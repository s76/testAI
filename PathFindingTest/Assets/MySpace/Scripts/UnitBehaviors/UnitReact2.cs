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
	
	public bool showDebug;
	
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
		debug.Log (showDebug, 1, "IsMovingAlongPath", current_behavior_state == BehaviorState.MoveAlongPath);
		return current_behavior_state == BehaviorState.MoveAlongPath;
	}
	
	public bool IsAttaking () {
		debug.Log (showDebug, 2, "IsAttaking", current_behavior_state == BehaviorState.Attack);
		return current_behavior_state == BehaviorState.Attack;
	}
	
	public bool IsTryingToMoveAround () {
		debug.Log (showDebug, 3, "IsTryingToMoveAround", current_behavior_state == BehaviorState.TryMoveAround);
		return current_behavior_state == BehaviorState.TryMoveAround;
	}
	
	public bool IsWaitingInPosition () {
		debug.Log (showDebug, 4, "IsWaitingInPosition", current_behavior_state == BehaviorState.WaitInPosition);
		return current_behavior_state == BehaviorState.WaitInPosition;
	}
	
	public bool IsAppoaching () {
		debug.Log (showDebug, 5, "IsAppoaching", current_behavior_state == BehaviorState.Approach);
		return current_behavior_state == BehaviorState.Approach;
	}
	
	/* ########################## Situational judgment tests ############################ */
	
	void OnDrawGizmosSelected () {
		
		if (showDebug) {
			UnityEditor.Handles.color = Color.magenta;
			UnityEditor.Handles.DrawWireDisc (transform.position, transform.up, steering.detect_range);
			
			UnityEditor.Handles.color = Color.green;
			UnityEditor.Handles.DrawLine (transform.position, transform.position + steering.dir);
		}
		
	}
	public float reach_check_range=0.6f;
	
	
	public bool CanReachTarget () {	
		var k = core.IsTrackedEnemyInRange ();
		if (!k) {
			debug.Log (showDebug, 101, "IsTrackedEnemyInRange = false");
			var to_target = ( core.tracked_enemy.transform.position - transform.position).normalized;
			Vector3 left25 = Quaternion.Euler(0, -25, 0) * to_target;
			Vector3 right25 = Quaternion.Euler(0, 25, 0) * to_target;
			
			RaycastHit hit;
			if ( Physics.Linecast(transform.position,transform.position + to_target * reach_check_range, out hit ) ) {
				debug.Log (showDebug, 102, "obstacle in front of me");
				if ( (hit.point - core.tracked_enemy.transform.position ).magnitude > core.parameters.attack_range * 0.8f ) {
					debug.Log (showDebug, 103, "obstacles len to neemy > attack range * 0.8");
					
					RaycastHit hit_r,hit_l;
					
					if ( Physics.Linecast(transform.position,transform.position + right25 * reach_check_range, out hit_r ) 
					    && Physics.Linecast(transform.position,transform.position + left25 * reach_check_range, out hit_l )
					    ) {
						debug.Log (showDebug, 10, "CanReachTarget = false");
						return false;
					}
				}
			}
		}
		
		debug.Log (showDebug, 10, "CanReachTarget = true");
		return true;
	}
	
	public bool TryGettingAvailableBarier () {
		var b = core.GetClosestBarier ();
		debug.Log (showDebug, 11, "TryGettingAvailableBarier");
		
		return false;
	}
	
	public bool IsCurrentHidePosOk () {
		debug.Log (showDebug, 12, "IsCurrentHidePosOk");
		return false;
	}
	
	public bool TriedToMoveAround () {
		debug.Log (showDebug, 13, "TriedToMoveAround");
		return core._tried_to_move_around;
	}
	
	public bool CanTakeControlFromManualSteering () {
		debug.Log (showDebug, 14, "CanTakeControlFromManualSteering", canTakeBackControlFromManualSteering);
		return canTakeBackControlFromManualSteering;
	}
	/* ########################## State defining actions ############################ */
	public Action ApproachEnemy () {
		debug.Log (showDebug, 20, "ApproachEnemy");
		OnSteering = false;
		CancelInvoke ();
		current_behavior_state = BehaviorState.Approach;
		if (core.HasTrackedEnemy ()) {
			//if ( !core.agent.isActiveAndEnabled ) core.agent.enabled = true;
			//core.agent.avoidancePriority = 20;
			core.agent.SetDestination (core.tracked_enemy.transform.position);
			//core.agent.Resume();
		}
		yield return NodeResult.Success;
	}
	
	public Action DoGhostJump () {
		debug.Log (showDebug, 21, "DoGhostJump");
		
		yield return NodeResult.Success;
	}
	
	
	public Action Attack () {
		debug.Log (showDebug, 22, "Attack");
		core.agent.Stop ();
		//core.agent.velocity = Vector3.zero;
		
		//if (core.type == UnitType.Melee)
		//core.agent.avoidancePriority = 6;
		//if (core.type == UnitType.Ranger)
		//core.agent.avoidancePriority = 10;
		
		current_behavior_state = BehaviorState.Attack;
		yield return NodeResult.Success;
	}
	
	
	public Action MoveAlongPath () {
		debug.Log (showDebug, 23, "MoveAlongPath");
		//	if (core.type == UnitType.Melee)
		//		core.agent.avoidancePriority = 4;
		//	if (core.type == UnitType.Ranger)
		//	core.agent.avoidancePriority = 5;
		
		//	if (!core.agent.isActiveAndEnabled)
		//	core.agent.enabled = true;
		current_behavior_state = BehaviorState.MoveAlongPath;
		core.IgnoreCurrentEnemy ();
		core.ReturnToCurrentNode();
		yield return NodeResult.Success;
	}
	
	DebugX debug = new DebugX();
	
	public Action TryMoveAround () {
		debug.Log (showDebug, 24, "TryMoveAround");
		core.agent.Stop ();
		//if ( core.agent.isActiveAndEnabled ) core.agent.enabled = false;
		current_behavior_state = BehaviorState.TryMoveAround;
		steering_timer = 0;
		canTakeBackControlFromManualSteering = false;
		InvokeRepeating ("ManualSteering", 0f, steering_update_freq);
		yield return NodeResult.Success;
	}
	
	public Action WaitInPosition () {
		debug.Log (showDebug, 25, "WaitInPosition");
		current_behavior_state = BehaviorState.WaitInPosition;
		yield return NodeResult.Success;
	}
	
	/* ########################## Try to move around - steering job ################### */
	
	Steering2 steering = new Steering2();
	public float steering_update_freq = 0.05f;
	public float max_time_steering = 6f;
	float steering_timer;
	bool canTakeBackControlFromManualSteering;
	
	public bool OnSteering;
	
	void ManualSteering () {
		OnSteering = true;
		steering.showDebug = showDebug;
		steering.Process (transform, steering_update_freq);
		steering_timer += steering_update_freq;
		if (steering_timer > max_time_steering)
			canTakeBackControlFromManualSteering = true;
	}
	
	/* ########################## action functions ############################ */
	Vector3 last_approach_pos;
	public Action DoApproachEnemy () {
		var dif = last_approach_pos - core.tracked_enemy.transform.position;
		if (dif.magnitude > core.parameters.attack_range) {
			last_approach_pos = core.tracked_enemy.transform.position;
			//core.agent.Resume();
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

