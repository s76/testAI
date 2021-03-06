using UnityEngine;
using System.Collections;

public enum UnitType { Melee, Ranger } 

public class UnitState : IPoolable {
	public SquadBehavior squad;

	public UnitType type;
	public UnitSide side;
	public UnitParameters parameters;
	public float waypoint_path_tolerance=5;
	public float barier_hide_position_tolerance=1;
	public float enemy_range_penalty_factor; // 0.0 - 1.0; enemy_true_range = enemy_range * ( 1- enemy_range_penalty_factor)

	public UnitState tracked_enemy;   // !read-only 
	
	public Node2 current_node;

	public NavMeshAgent agent { get; private set; }
	
	static int global_id_counter = 0;
	static GameController game_controller;

	public int id { get; private set; }
	Color debug_attack_color;

	void Awake () {
		id = global_id_counter ++;			
		agent = GetComponent<NavMeshAgent> ();
		debug_attack_color = GetComponent<MeshRenderer> ().material.color;
	}

	void Start () {
		if ( game_controller == null ) game_controller = GameObject.FindObjectOfType<GameController> ();
		game_controller.RegisterUnit (this);
	}

	public override void Activate ()
	{
		base.Activate ();
		tracked_enemy = null;
		current_node = null;
		enemy_range_penalty_factor = 0;
		squad = null;
		id = global_id_counter ++;
		parameters.hp_current = parameters.hp_max;
	}

	public void SetSpawner (Spawner2 spawner ) {
		current_node = spawner.startNode;
		agent.Warp (spawner.startNode.real_transform.position);
		agent.ResetPath ();
		agent.SetDestination (spawner.startNode.real_transform.position);
	}

	public void MoveToNextNode () {
		var available_nodes = current_node [side];
		if (available_nodes.Length > 0) {  
			current_node = available_nodes [game_controller.mutationTables [id % game_controller.mutationTables.Length] % available_nodes.Length];
			agent.SetDestination(current_node.real_transform.position);
		} else {
			current_node = null;
			agent.Stop();
		}
	}

	public void DealDamageToTrackedEnemy () {
		tracked_enemy.parameters.hp_current -= parameters.attack_damage;

		Debug.DrawLine (transform.position, tracked_enemy.transform.position, debug_attack_color);
		if (tracked_enemy.parameters.hp_current <= 0) {
			tracked_enemy.Deactivate();
			tracked_enemy = null;
		}
	}

	void OnDrawGizmosSelected () {
		UnityEditor.Handles.color = Color.green;
		UnityEditor.Handles.DrawWireDisc (transform.position, transform.up, parameters.sight_range);

		UnityEditor.Handles.color = Color.magenta;
		UnityEditor.Handles.DrawWireDisc (transform.position, transform.up, tracked_enemy == null ? parameters.attack_range : parameters.attack_range * (1 - tracked_enemy.enemy_range_penalty_factor ));

		UnityEditor.Handles.color = Color.cyan;
		UnityEditor.Handles.DrawWireDisc (transform.position, transform.up, parameters.barier_search_range);

		
		UnityEditor.Handles.color = Color.yellow;
		UnityEditor.Handles.DrawWireDisc (transform.position, transform.up, waypoint_path_tolerance);

		UnityEditor.Handles.color = Color.Lerp(Color.green, Color.red, 0.5f);
		UnityEditor.Handles.DrawWireDisc (transform.position, transform.up, waypoint_path_tolerance);

		if (current_node != null) {
			Gizmos.color = Color.white;
			Gizmos.DrawLine (transform.position, current_node.real_transform.position);
		}
		
		if (tracked_enemy != null) {
			Gizmos.color = Color.gray;
			Gizmos.DrawLine (transform.position, tracked_enemy.transform.position);
		}

	}
}
