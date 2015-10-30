using UnityEngine;
using System.Collections;
using React;

using  Action = System.Collections.Generic.IEnumerator<React.NodeResult>;

public class AgentBehavior : MonoBehaviour {	
	static GameController game_controller;
	static int walls_mask = LayerMask.GetMask("walls");
	static int bariers_mask = LayerMask.GetMask("bariers");

	DebugX debug = new DebugX();
	public bool showDebug;

	[SerializeField] AgentState current_agent_state;
	
	Reactor reactor;
	AgentX2 core;
	int tracked_enemy_id;

	/* ########################## core mono functions ######################## */
	void Awake () {
		reactor = GetComponent<Reactor> ();
		core = GetComponent<AgentX2> ();
	}
	
	void Start () {
		if ( game_controller == null ) game_controller = GameObject.FindObjectOfType<GameController> ();
	}
	

	/* ########################## State queries ############################ */
	public bool IsMoving () {
		debug.Log (showDebug, 1, "IsMoving", current_agent_state == AgentState.Move);
		return current_agent_state == AgentState.Move;
	}
	
	public bool IsAttaking () {
		debug.Log (showDebug, 2, "IsAttaking", current_agent_state == AgentState.Attack);
		return current_agent_state == AgentState.Attack;
	}

	
	public bool IsApproaching () {
		debug.Log (showDebug, 3, "IsApproaching", current_agent_state == AgentState.Approach);
		return current_agent_state == AgentState.Approach;
	}

	/* ########################## Situational judgment tests ############################ */


	// ####
	

	public Action Move () { //!!!
		current_agent_state = AgentState.Move;
		debug.Log (showDebug, 23, "Move");
		core.IgnoreCurrentEnemy ();
		core.ReturnToCurrentNode();
		yield return NodeResult.Success;
	}

	public Action Attack () { //!!
		current_agent_state = AgentState.Attack;
		core.Stop ();

		debug.Log (showDebug, 20, "Attack");
	
		yield return NodeResult.Success;
	}

	public Action Approach () { //!!
		current_agent_state = AgentState.Approach;
		debug.Log (showDebug, 4365623, "Approach");
		yield return NodeResult.Success;
	}

	/* ########################## action functions ############################ */

	double last_attack_timer = 0.0;
	
	public Action LaunchAttack () {
		debug.Log (showDebug,12121, "LaunchAttack");
		if (game_controller.timer - last_attack_timer > core.parameters.attack_cd) {
			last_attack_timer = game_controller.timer;
			core.DealDamageToTrackedEnemy ();
		} 
		yield return NodeResult.Success;
	}

	public Action DoMove () {
		debug.Log (showDebug,22121, "DoMove");
		if ( core.current_node == null ) yield return NodeResult.Success;
		
		if (core.ArrivedAtCurrentNode() ){
			core.MoveToNextNode();
		}
		yield return NodeResult.Success;
	}

	public Action DoApproach () {
		
		debug.Log (showDebug,44666756, "DoApproach");
		
		core.ApproachEnemy ();
		yield return NodeResult.Success;
	}
}
