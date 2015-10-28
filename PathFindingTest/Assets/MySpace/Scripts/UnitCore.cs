using UnityEngine;
using System.Collections.Generic;

public class UnitCore : IPoolable {
	static int global_id_counter = 0;
	static GameController game_controller;
	static int walls_mask = LayerMask.GetMask("walls");

	public UnitType type;
	public UnitSide side;
	public UnitParameters2 parameters;

	public float waypoint_path_tolerance=5;
	public float barier_hide_position_tolerance=1;
	public float enemy_range_penalty_factor; // 0.0 - 1.0; enemy_true_range = enemy_range * ( 1- enemy_range_penalty_factor)
	
	public UnitCore tracked_enemy;   // !read-only 
	
	public Node2 current_node;
	public NavMeshAgent agent { get; private set; }
	public int id { get; private set; }

	
	Color debug_attack_color;	

	// ####################### core mono ################################### //
	
	void Awake () {
		id = global_id_counter ++;			
		agent = GetComponent<NavMeshAgent> ();
		debug_attack_color = GetComponent<MeshRenderer> ().material.color;
	}
	
	void Start () {
		if ( game_controller == null ) game_controller = GameObject.FindObjectOfType<GameController> ();
		game_controller.RegisterUnit (this);
	}
	
	// ####################### ipoolable functions ################################### //
	public override void Activate ()
	{
		base.Activate ();
		
		tracked_enemy = null;
		current_node = null;
		enemy_range_penalty_factor = 0;
		id = global_id_counter ++;
		parameters.hp_current = parameters.hp_max;
	}

	
	// ####################### core functions ################################### //
	int last_tracked_enemy_id = -1;

	public bool HasEnemy () {
		if (tracked_enemy == null) 
			return false;
		if (!tracked_enemy.isActiveAndEnabled) {
			tracked_enemy = null;
			return false;
		}
		if ( tracked_enemy.id != last_tracked_enemy_id) {
			tracked_enemy = null;
			return false;
		}
		return true;
	}

	public bool EnemyInSight () {
		var e = GetClosestActiveEnemy ();
		if (e != null) {
			if ( !HasEnemy () ) {
				tracked_enemy = e;
				last_tracked_enemy_id = e.id;
			}
			return true;
		} 
		return false;
	}

	public bool EnemyInRange () {
		if (HasEnemy ()) {	
			return !Physics.Linecast(tracked_enemy.transform.position, transform.position,walls_mask) &&
				(tracked_enemy.transform.position - transform.position).magnitude <= parameters.attack_range  * ( 1- tracked_enemy.enemy_range_penalty_factor);
		}
		return false;
	}

	public bool ArrivedAtCurrentNode (){
		if ((transform.position - current_node.real_transform.position).magnitude > waypoint_path_tolerance) return false;
		return true;
	}

	public void ReturnToCurrentNode () {
		if ( current_node != null ) {
			agent.SetDestination (current_node.real_transform.position);
		}
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

	public Barier GetClosestBarier () {
		throw new UnityException("unimplemented");
		
		if ( !HasEnemy() ) return null;
		
		//		var bariers = game_controller.bariers;
		//		var vector_to_enemy = tracked_enemy.transform.position - transform.position;
		//		float min_distance = parameters.barier_search_range;
		//		
		//		Barier closest_barier = null;
		//		
		//		foreach (var b in bariers) {
		//			b.TestIfBarierSuitableToHide(core, vector_to_enemy, ref min_distance, ref closest_barier );
		//		}
		//		
		return null; 
	}
	// ####################### debug functions ################################### //

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

	
	// ####################### background/help/ functions ################################### //

	
	UnitCore GetClosestActiveEnemy () {
		var enemies = game_controller.GetUnitListOfOppositeSide (side);
		float min_distance = float.MaxValue;
		UnitCore closest_enemy = null;
		
		foreach (var e in enemies) {
			if ( !e.isActiveAndEnabled  ) continue;
			
			var diff = e.transform.position - transform.position;
			float distance = diff.magnitude;
			
			if ( distance > parameters.sight_range ) continue;
			
			if ( Physics.Linecast(transform.position, e.transform.position,walls_mask) ) continue;
			
			if ( distance < min_distance ) {
				closest_enemy = e;
				min_distance = distance;
			}
		}
		
		return closest_enemy; 
	}

}