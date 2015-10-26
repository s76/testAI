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

	int tracked_enemy_id;
	
	static int walls_mask = LayerMask.GetMask("walls");
	static int bariers_mask = LayerMask.GetMask("bariers");

	
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
		if ( tracked_enemy_id != unit_state.tracked_enemy.id ) {
			unit_state.tracked_enemy = null;
			return false;
		}
		return true;
	}
	
	public bool IsTrackedEnemyInRange () {
		if (HasTrackedEnemy ()) {
			return Physics.Linecast(transform.position,unit_state.tracked_enemy.transform.position,bariers_mask)
				&& (unit_state.tracked_enemy.transform.position - transform.position).magnitude <= unit_state.parameters.attack_range  * ( 1- unit_state.tracked_enemy.enemy_range_penalty_factor);
		}
		return false;
	}

	public bool IsEnemyInSight() {
		var e = GetClosestActiveEnemy ();
		if (e != null) {
			if ( !HasTrackedEnemy () ) {
				unit_state.tracked_enemy = e;
				tracked_enemy_id = unit_state.tracked_enemy.id;
				e.OnBeingSetAsAttackTarget(unit_state);
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
		current_behavior_state = BehaviorState.Attack;
		unit_state.tracked_enemy.RearrangeAttackersPosition();
		yield return NodeResult.Success;
	}


	public Action MoveAlongPath () {
		current_behavior_state = BehaviorState.MoveAlongPath;
		unit_state.agent.SetDestination (unit_state.current_node.real_transform.position);
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

	double last_attack_timer = 0.0;

	public Action LaunchAttack () {
		if (game_controller.timer - last_attack_timer > unit_state.parameters.attack_cd) {
			last_attack_timer = game_controller.timer;
			unit_state.DealDamageToTrackedEnemy ();
		} else {
			if ( unit_state.current_cell != null ) {
				unit_state.agent.SetDestination(unit_state.current_cell.position );
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
