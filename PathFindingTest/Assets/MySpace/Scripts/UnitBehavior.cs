using UnityEngine;
using System.Collections;
using React;

using  Action = System.Collections.Generic.IEnumerator<React.NodeResult>;

public enum BehaviorState { MoveAlongPath, Attack, Reposition, WaitInPosition, Approach }

public class UnitBehavior : MonoBehaviour {
	[SerializeField] BehaviorState current_behavior_state = BehaviorState.MoveAlongPath;


	Barier selected_barier;

	[SerializeField] Reactable unitReactable;
	Reactor reactor;
	UnitState unit_state;

	static GameController game_controller;

	
	static int walls_mask = LayerMask.GetMask("walls");
	static int bariers_mask = LayerMask.GetMask("bariers");


	int tracked_enemy_id;

	/* ########################## core mono functions ######################## */
	void Awake () {
		reactor = GetComponent<Reactor> ();
		unit_state = GetComponent<UnitState> ();
	}

	void Start () {
		if ( game_controller == null ) game_controller = GameObject.FindObjectOfType<GameController> ();
	}

	/* ########################## State queries ############################ */
	public bool IsMovingAlongPath () {
		return current_behavior_state == BehaviorState.MoveAlongPath;
	}

	public bool IsAttaking () {
		return current_behavior_state == BehaviorState.Attack;
	}

	public bool IsRepositioning () {
		return current_behavior_state == BehaviorState.Reposition;
	}

	public bool IsWaitingInPosition () {
		return current_behavior_state == BehaviorState.WaitInPosition;
	}

	public bool IsAppoaching () {
		return current_behavior_state == BehaviorState.Approach;
	}

	/* ########################## Situational judgment tests ############################ */
	public bool HasTrackedEnemy () {
		if (unit_state.tracked_enemy == null) 
			return false;
		if (!unit_state.tracked_enemy.isActiveAndEnabled) {
			unit_state.tracked_enemy = null;
			return false;
		}

		if (unit_state.tracked_enemy.id != tracked_enemy_id ) {
			unit_state.tracked_enemy = null;

			return false;
		}

		return true;
	}
	
	public bool IsTrackedEnemyInRange () {
		if (HasTrackedEnemy ()) {	
			return !Physics.Linecast(unit_state.tracked_enemy.transform.position, transform.position,walls_mask) &&
				(unit_state.tracked_enemy.transform.position - transform.position).magnitude <= unit_state.parameters.attack_range  * ( 1- unit_state.tracked_enemy.enemy_range_penalty_factor);
		}
		return false;
	}

	public bool IsEnemyInSight() {
		var e = GetClosestActiveEnemy ();
		if (e != null) {
			if ( !HasTrackedEnemy () ) {
				unit_state.tracked_enemy = e;
				tracked_enemy_id = e.id;
			}
			return true;
		} 
		return false;
	}

	public bool NeedRepositioning () {	
		if (unit_state.type == UnitType.Melee)
			return false;

		if (HasTrackedEnemy ()) {

		}

		if (flock_force != Vector3.zero) {
			//flock_pos = transform.position + flock_force;
			//flock_force = Vector3.zero;

			//unit_state.agent.Resume();
			//unit_state.agent.SetDestination(flock_pos);
		}
		return false;
	}

	public bool TryGettingAvailableBarier () {
		var b = GetClosestBarier ();
		if (b != null) {
			selected_barier = b;

		}
		return false;
	}

	public bool IsCurrentHidePosOk () {
		return false;
	}


	/* ########################## State defining actions ############################ */
	public Action ApproachEnemy () {
		unit_state.agent.avoidancePriority = 20;
		current_behavior_state = BehaviorState.Approach;
		unit_state.agent.SetDestination (unit_state.tracked_enemy.transform.position);
		yield return NodeResult.Success;
	}

	/*
	public Action TryFindingBetterCombatPosition () {
		// get potential good combat position
		// if ( found ) return Failure
		yield return NodeResult.Success;
	}
	*/

	public Action Attack () { 
		unit_state.agent.Stop ();
		unit_state.agent.velocity = Vector3.zero;
		
		if (unit_state.type == UnitType.Melee)
			unit_state.agent.avoidancePriority = 6;
		if (unit_state.type == UnitType.Ranger)
			unit_state.agent.avoidancePriority = 10;

		//GetComponent<NavMeshObstacle> ().enabled = true;
		current_behavior_state = BehaviorState.Attack;
		yield return NodeResult.Success;
		
	}

	public Action MoveAlongPath () {
		if (unit_state.type == UnitType.Melee)
			unit_state.agent.avoidancePriority = 4;
		if (unit_state.type == UnitType.Ranger)
			unit_state.agent.avoidancePriority = 5;

		current_behavior_state = BehaviorState.MoveAlongPath;
		if ( unit_state.current_node != null ) unit_state.agent.SetDestination (unit_state.current_node.real_transform.position);
		unit_state.agent.Resume ();
		yield return NodeResult.Success;
	}

	public Action Reposition () {
		current_behavior_state = BehaviorState.Reposition;
		yield return NodeResult.Success;

	}

	public Action WaitInPosition () {
		current_behavior_state = BehaviorState.WaitInPosition;
		yield return NodeResult.Success;
	}
	
	/* ########################## action functions ############################ */
	Vector3 last_approach_pos;
	float _appr_timer;
	public Action DoApproachEnemy () {
		var dif = last_approach_pos - unit_state.tracked_enemy.transform.position;
		if (dif.magnitude > unit_state.parameters.attack_range) {
			last_approach_pos = unit_state.tracked_enemy.transform.position;
			unit_state.agent.ResetPath();
			unit_state.agent.SetDestination (last_approach_pos);
		} else {
			_appr_timer += reactor.TickDuration;
			if ( _appr_timer > 2f ) {
				if ( !unit_state.agent.pathPending ) {
					if ( unit_state.agent.pathStatus == NavMeshPathStatus.PathComplete | unit_state.agent.pathStatus == NavMeshPathStatus.PathPartial ) {
						if (  !IsTrackedEnemyInRange()  ) {
							_appr_timer = 0;

							var e = GetClosestActiveEnemy ();

							if ( e == unit_state.tracked_enemy ) {
								e = GetAnotherCloseActiveEnemy(unit_state.tracked_enemy);
							}

							if (e != null) {
								unit_state.tracked_enemy = e;
								tracked_enemy_id = e.id;
								unit_state.agent.ResetPath();
								last_approach_pos = e.transform.position;
								unit_state.agent.SetDestination(last_approach_pos);
								unit_state.agent.avoidancePriority = 9;
							} else {
								//unit_state.agent.Stop();
								//unit_state.agent.velocity = Vector3.zero;
								//unit_state.agent.avoidancePriority = 60;
								current_behavior_state = BehaviorState.MoveAlongPath;
								if ( unit_state.current_node != null ) unit_state.agent.SetDestination (unit_state.current_node.real_transform.position);
								unit_state.agent.Resume ();
								unit_state.tracked_enemy = null;
							}
						}
					}
				}
			}
		}

		yield return NodeResult.Success;
	}



	double last_attack_timer = 0.0;
	float rearrange_timer = 0f;
	float flock_timer = 0f;
	Vector3 flock_force = Vector3.zero;
	public float flock_strength = 1;
	public float min_dis_ranger_attackers = 1.5f;
	Vector3 flock_pos;

	public Action LaunchAttack () {
		if (game_controller.timer - last_attack_timer > unit_state.parameters.attack_cd) {
			last_attack_timer = game_controller.timer;
			unit_state.DealDamageToTrackedEnemy ();
		} 
		if ( !HasTrackedEnemy() ) yield return NodeResult.Success;
		if ( unit_state.type == UnitType.Ranger ) {
			float sqrt_min_dis_ranger_attackers = min_dis_ranger_attackers*min_dis_ranger_attackers;
			rearrange_timer += reactor.TickDuration;
			if ( rearrange_timer > 1f ) {
				rearrange_timer = 0;
				var a = game_controller.GetUnitListOfOppositeSide(unit_state.tracked_enemy.side);
				foreach ( var l in a ) {
					if ( l.type == UnitType.Melee ) continue;
					if ( !l.isActiveAndEnabled ) continue; 
					var dif = (transform.position - l.transform.position );
					if ( dif.sqrMagnitude > sqrt_min_dis_ranger_attackers )  continue;
					flock_force += dif.normalized * flock_strength;
					//Debug.DrawLine(transform.position, l.transform.position,Color.white);
    			}
			}
		}

		yield return NodeResult.Success;
	}

	public Action DoRepositioning () {
		
		yield return NodeResult.Success;
	}

	public Action Move () {
		if ( unit_state.current_node == null ) yield return NodeResult.Success;

		if ((transform.position - unit_state.current_node.real_transform.position).magnitude < unit_state.waypoint_path_tolerance) {
			unit_state.MoveToNextNode();
		}
		yield return NodeResult.Success;
	}


	/* ########################## help functions ############################ */
	UnitState GetClosestActiveEnemy () {
		var enemies = game_controller.GetUnitListOfOppositeSide (unit_state.side);
		float min_distance = float.MaxValue;
		UnitState closest_enemy = null;

		foreach (var e in enemies) {
			if ( !e.isActiveAndEnabled  ) continue;

			var diff = e.transform.position - transform.position;
			float distance = diff.magnitude;

			if ( distance > unit_state.parameters.sight_range ) continue;

			if ( Physics.Linecast(transform.position, e.transform.position,walls_mask) ) continue;

			if ( distance < min_distance ) {
				closest_enemy = e;
				min_distance = distance;
			}
		}
		
		return closest_enemy; 
	}

	UnitState GetAnotherCloseActiveEnemy (UnitState enemy_unit) {
		var being_attaked = game_controller.GetUnitListOfOppositeSide (unit_state.side);

		float distance_to_enemy_unit = float.MaxValue;
		float distance_to_attacking_unit = float.MaxValue;

		UnitState closest_enemy = null;
		
		foreach (var e in being_attaked) {
			if ( e == enemy_unit ) continue;
			if ( !e.isActiveAndEnabled  ) continue;
			
			var diff = e.transform.position - enemy_unit.transform.position;
			float distance_to_em = diff.magnitude;
			
			var diff2 = e.transform.position - transform.position;
			float distance_to_attacker = diff2.magnitude;
			
			if ( distance_to_attacker > unit_state.parameters.sight_range ) continue;
			
			if ( Physics.Linecast(transform.position, e.transform.position,walls_mask) ) continue;
			
			if ( distance_to_em < distance_to_enemy_unit ) {
				closest_enemy = e;
				distance_to_enemy_unit = distance_to_em;
			}
		}
		
		return closest_enemy; 
	}

	Barier GetClosestBarier () {
		if ( !HasTrackedEnemy () ) return null;

		var bariers = game_controller.bariers;
		var vector_to_enemy = unit_state.tracked_enemy.transform.position - transform.position;
		float min_distance = unit_state.parameters.barier_search_range;

		Barier closest_barier = null;

		foreach (var b in bariers) {
			b.TestIfBarierSuitableToHide(unit_state, vector_to_enemy, ref min_distance, ref closest_barier );
		}
		
		return closest_barier; 
	}


	/* ########################## debug functions ############################ */




}
