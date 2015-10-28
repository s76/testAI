using UnityEngine;
using System.Collections;
using React;

using  Action = System.Collections.Generic.IEnumerator<React.NodeResult>;

public class UnitReact : MonoBehaviour
{
	static GameController game_controller;
	static int walls_mask = LayerMask.GetMask("walls");
	static int bariers_mask = LayerMask.GetMask("bariers");

	[SerializeField] BehaviorState current_behavior_state = BehaviorState.MoveAlongPath;

	Reactor reactor;
	UnitCore core;
	
	int tracked_enemy_id;

	/* ########################## core mono functions ######################## */
	void Awake () {
		reactor = GetComponent<Reactor> ();
		core = GetComponent<UnitCore> ();
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
		return core.HasEnemy();
	}
	
	public bool IsTrackedEnemyInRange () {
		return core.EnemyInRange();
	}
	
	public bool IsEnemyInSight() {
		return core.EnemyInSight ();
	}
	
	public bool NeedRepositioning () {	
		if (core.type == UnitType.Melee)
			return false;
		
		if (core.HasEnemy()) {
			
		}
		return false;
	}
	
	public bool TryGettingAvailableBarier () {
		var b = core.GetClosestBarier ();

		return false;
	}
	
	public bool IsCurrentHidePosOk () {
		return false;
	}
	
	
	/* ########################## State defining actions ############################ */
	public Action ApproachEnemy () {
		current_behavior_state = BehaviorState.Approach;
		core.agent.avoidancePriority = 20;
		core.agent.SetDestination (core.tracked_enemy.transform.position);
		yield return NodeResult.Success;
	}
	
	
	public Action Attack () {
		core.agent.Stop ();
		core.agent.velocity = Vector3.zero;
		
		if (core.type == UnitType.Melee)
			core.agent.avoidancePriority = 6;
		if (core.type == UnitType.Ranger)
			core.agent.avoidancePriority = 10;
		
		current_behavior_state = BehaviorState.Attack;
		yield return NodeResult.Success;
	}
	
	
	public Action MoveAlongPath () {
		if (core.type == UnitType.Melee)
			core.agent.avoidancePriority = 4;
		if (core.type == UnitType.Ranger)
			core.agent.avoidancePriority = 5;
		
		current_behavior_state = BehaviorState.MoveAlongPath;
		core.ReturnToCurrentNode();
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
	public Action DoApproachEnemy () {
		var dif = last_approach_pos - core.tracked_enemy.transform.position;
		if (dif.magnitude > core.parameters.attack_range) {
			last_approach_pos = core.tracked_enemy.transform.position;
			core.agent.SetDestination (last_approach_pos);
		} 
		yield return NodeResult.Success;
	}

	double last_attack_timer = 0.0;
	
	public Action LaunchAttack () {
		if (game_controller.timer - last_attack_timer > core.parameters.attack_cd) {
			last_attack_timer = game_controller.timer;
			core.DealDamageToTrackedEnemy ();
		} 
		yield return NodeResult.Success;
	}
	
	public Action DoRepositioning () {
		
		yield return NodeResult.Success;
	}
	
	public Action Move () {
		if ( core.current_node == null ) yield return NodeResult.Success;
		
		if (core.ArrivedAtCurrentNode() ){
			core.MoveToNextNode();
		}
		yield return NodeResult.Success;
	}
}

