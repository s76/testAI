using UnityEngine;
using System.Collections;
using React;

//This is a little shortcut which creates an alias for a type, and makes out Action methods look much nicer.
using  Action = System.Collections.Generic.IEnumerator<React.NodeResult>;


public class UnitAI : MonoBehaviour {
	public bool pritnLog;
	static int obstacles_mask = LayerMask.GetMask("obstacles");

	public UnitCore core { get; private set; }
	Reactor react;
	int enemy_layer;

	void Start () {
		core = GetComponent<UnitCore> ();
		react = GetComponent<Reactor> ();
		enemy_layer = LayerMask.GetMask (core.side == UnitSide.Side01 ? "units-side02" : "units-side01"); 
		UnitGlobalController.Instance.Register(this);
	}

	float distance_tolerance = 5;
	
	public enum AS { Move, Attack, Approach }
	AS actionState = AS.Move;

	public bool isMoving () {
		return actionState == AS.Move;
	}

	public bool isAttacking () {
		return actionState == AS.Attack;
	}

	public bool isApproaching () {
		return actionState == AS.Approach;
	}

	UnitAI GetClosetInSightEnemy () {
		var enemies = UnitGlobalController.Instance.GetEnemies (this);
		float min_distance = float.MaxValue;
		UnitAI closest_enemy = null;
		foreach (var e in enemies) {
			if ( !e.isActiveAndEnabled  ) continue;
			// ################# need optimization ############################
			// ## if sight_angle = 360 -> use vector3.magnitude instead of raycast

			var t = e.transform.position - transform.position;

			Ray r = new Ray(transform.position,t.normalized);
			RaycastHit hitinfo;
			if ( Physics.Raycast(r, out hitinfo, maxDistance:core.parameters.sight_range, layerMask: enemy_layer)) {
				if ( Physics.Linecast(r.origin,hitinfo.point,obstacles_mask) ) continue;
				if ( hitinfo.distance < min_distance ) {
					closest_enemy = e;
					min_distance = hitinfo.distance;
				}
			}
		}
		
		return closest_enemy; 
	}

	bool IsInAttackRange (UnitAI enemy ) {
		return (enemy.transform.position - transform.position).magnitude <= core.parameters.attack_range;
	}

	UnitAI _current_enemy;

	Node[] last_node_choices;
	int last_node_choice_index = 0;


	public Action MoveAction () {
		if (pritnLog)
			Debug.Log ("MoveAction");
		var e = GetClosetInSightEnemy ();

		if (e != null) {
			if (e != _current_enemy) _current_enemy = e;
		} 

		if ((transform.position - core.currentNode.real_transform.position).magnitude < distance_tolerance) {
			
			if (pritnLog)
				Debug.Log ("Arrived to current node");
			
			last_node_choices = core.side == UnitSide.Side01 ? core.currentNode.connectedTo : core.currentNode.connectedFrom;
			if ( last_node_choices.Length > 0 ) {
				
				var mut = UnitGlobalController.Instance.mutationTables;
				
				last_node_choice_index = mut[core.id%mut.Length ] % last_node_choices.Length;
				core.last_node = core.currentNode;

				core.currentNode = last_node_choices[last_node_choice_index];

				core.agent.SetDestination (core.currentNode.real_transform.position);
			}
		}
		
		if (!core.agent.pathPending) {
			if (core.agent.pathStatus == NavMeshPathStatus.PathPartial) {
				
				Debug.Log ("NavMeshPathStatus.PathPartial");
				
				last_node_choice_index ++;
				core.last_node = core.currentNode;
				
				last_node_choices = core.side == UnitSide.Side01 ? core.currentNode.connectedTo : core.currentNode.connectedFrom;
				core.currentNode = last_node_choices [last_node_choice_index % last_node_choices.Length];
				core.agent.SetDestination (core.currentNode.real_transform.position);
				
			}
		} 
			
		if ( IsEnemyValid (_current_enemy) ) {
			if (pritnLog)
				Debug.Log ("IsEnemyValid");
			if (IsInAttackRange (_current_enemy)) {
				if (pritnLog)
					Debug.Log ("IsInAttackRange -> Attack");
				_attack_cd_timer = 0;
				core.agent.Stop ();
				actionState = AS.Attack;
				yield return NodeResult.Failure;
			} else {
				if (pritnLog)
					Debug.Log ("! IsInAttackRange -> Approach");
				actionState = AS.Approach;
				yield return NodeResult.Failure;
			}
		}
	
		//core.agent.Resume ();


		yield return NodeResult.Success;
	}

	bool IsEnemyValid (UnitAI enemy ) {
		if (enemy == null)
			return false;
		return enemy.isActiveAndEnabled;
	}

	float _attack_cd_timer;

	void DealDamage (UnitAI enemy ) {
		if (enemy._current_enemy == null) {
			enemy._current_enemy = this;
		}

		enemy.core.parameters.hp_current -= core.parameters.attack_damage;

		Debug.DrawLine (transform.position, enemy.transform.position, core.side == UnitSide.Side01? Color.yellow : Color.white);

		if (enemy.core.parameters.hp_current <= 0) {
	
			core.agent.SetDestination (core.currentNode.real_transform.position);
			core.agent.Resume();
			actionState = AS.Move;
			_current_enemy = null;

			enemy.core.Deactivate();
		}
	}

	public Action AttackAction () {
		if (IsEnemyValid (_current_enemy)) {
			if ( IsInAttackRange(_current_enemy ) ) {
				_attack_cd_timer += react.TickDuration;
				if (_attack_cd_timer > core.parameters.attack_cd) {
					_attack_cd_timer -= core.parameters.attack_cd;
					DealDamage(_current_enemy);
				}
				yield return NodeResult.Success;
			} else {
				core.agent.Resume();
				actionState = AS.Approach;
				yield return NodeResult.Failure;
			}
		} else {
			core.agent.Resume();
			actionState = AS.Move;
			yield return NodeResult.Failure;
		}
	}

	public Action ApproachAction () {
		if (IsEnemyValid (_current_enemy)) {
			var e = GetClosetInSightEnemy ();
			if ( IsEnemyValid (e ) ) {
				if ( e != _current_enemy ) {
					_current_enemy = e;
				}
			}

			if ( IsInAttackRange(_current_enemy ) ) {
				actionState = AS.Attack;
				core.agent.Stop();
				yield return NodeResult.Success;
			}

			core.agent.SetDestination(_current_enemy.transform.position);

			yield return NodeResult.Success;
		} else {
			core.agent.Resume();
			actionState = AS.Move;
			yield return NodeResult.Failure;
		}
	}

}
