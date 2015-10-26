using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	List<UnitState> attackers = new List<UnitState> ();

	public HexCenter hex_center { get; private set; }
	public HexCell current_cell { get; private set; }


	void Awake () {
		id = global_id_counter ++;			
		agent = GetComponent<NavMeshAgent> ();
		debug_attack_color = GetComponent<MeshRenderer> ().material.color;
		hex_center = GetComponent<HexCenter> ();
	}

	void Start () {
		if ( game_controller == null ) game_controller = GameObject.FindObjectOfType<GameController> ();
		game_controller.RegisterUnit (this);
	}


	public override void Activate ()
	{
		base.Activate ();
		current_cell = null;

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

	public void OnBeingSetAsAttackTarget (UnitState attacker ) {
		if (!attackers.Contains (attacker)) {	
			attackers.Add(attacker);
		}
		foreach (var a in attackers) {
			a.RearrangeAttackPosition(hex_center);		
		}
	}


	public void RearrangeAttackPosition (HexCenter hexcenter ) {

		var i = hexcenter.ToHexIndex (transform.position);
		if (hexcenter.ValidIndex (i)) {
			var cell = hex_center.GetCell(i);

			if ( cell == null ) {
				cell = hex_center.RegisterCell(i);
				cell.unit = this;
				current_cell = cell;
				return;
			}
			
			if ( cell.unit != null ) {
				cell = cell.FreeNeighbCell (hex_center);
				if ( cell != null ) {
					current_cell = cell;
					cell.unit = this;
					return;
				}
				else {
					Debug.LogError("shititititisi ");
				}
			}
		} else {
			throw new UnityException("Hex index is not valid");
		}
	}

	public void OnNotBeingSetAsAttackTarget (UnitState attacker ) {
		if (attackers.Contains (attacker)) {	
			attackers.Remove(attacker);
		}
	}

	public void DealDamageToTrackedEnemy () {
		tracked_enemy.parameters.hp_current -= parameters.attack_damage;

		Debug.DrawLine (transform.position, tracked_enemy.transform.position, debug_attack_color);
		if (tracked_enemy.parameters.hp_current <= 0) {
			tracked_enemy.Deactivate();

			tracked_enemy.attackers.Clear();
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
