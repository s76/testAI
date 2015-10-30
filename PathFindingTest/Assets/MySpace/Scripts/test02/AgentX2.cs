using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(NavMeshAgent))]
public class AgentX2 : IPoolable {
	static int walls_mask = LayerMask.GetMask("walls");
	static int bariers_mask = LayerMask.GetMask("bariers");
	static GameController game_controller;
	public UnitSide side;
	public UnitType type;

	public UnitParameters2 parameters;

	public AgentX2 tracked_enemy ;
	
	public void SetDestination (Vector3 v) {
		stop = false;
		target_reached = false;
		index = 0;
		agent.CalculatePath (v, path);
	}
	
	public void Stop () {
		if (showLog)
			Debug.Log ("stop --");
		stop = true;
	}

	public void SetSpawner (Spawner2 spawner, Vector3 offset ) {
		transform.position = spawner.transform.position + offset;
		current_node = spawner.startNode;
		SetDestination (spawner.startNode.real_transform.position);
	}
	
	bool stop;
	Vector3 target;
	
	int index;

	NavMeshAgent agent;
	NavMeshPath path;
	AgentBehavior react;

	
	public Node2 current_node;
	public int id { get; private set; }
	static int global_id_counter = 0;
	
	Color debug_attack_color;	

	public override void Activate ()
	{
		base.Activate ();
		
		tracked_enemy = null;
		current_node = null;
		id = global_id_counter ++;
		parameters.hp_current = parameters.hp_max;
	}


	// ####################### core mono ################################### //
	void Awake () {
		arounds_soft = new Collider[0];
		arounds_hard = new Collider[0];
		
		agent = GetComponent<NavMeshAgent> ();
		path  = new NavMeshPath();
		react = GetComponent<AgentBehavior> ();

		id = global_id_counter ++;			
		debug_attack_color = GetComponent<MeshRenderer> ().material.color;
	}	

	void Start () {
		if ( game_controller == null ) game_controller = GameObject.FindObjectOfType<GameController> ();
	}

	//# public f
	public void IgnoreCurrentEnemy () {
		tracked_enemy = null;
	}

	public void ReturnToCurrentNode () {
		if ( current_node != null ) {
			SetDestination (current_node.real_transform.position);
		}
	}
	public void ApproachEnemy () {
		SetDestination (tracked_enemy.transform.position);
	}

	public void DealDamageToTrackedEnemy () {
		tracked_enemy.parameters.hp_current -= parameters.attack_damage;
		
		Debug.DrawLine (transform.position, tracked_enemy.transform.position, debug_attack_color);
		if (tracked_enemy.parameters.hp_current <= 0) {
			tracked_enemy.Deactivate();
			tracked_enemy = null;
		}
	}
	
	public bool ArrivedAtCurrentNode (){
		if (showLog)
			Debug.Log ("ArrivedAtCurrentNode=" + ((transform.position - current_node.real_transform.position).magnitude > reach_distance));
		if ((transform.position - current_node.real_transform.position).magnitude > reach_distance) return false;
		return true;
	}
	

	public void MoveToNextNode () {
		var available_nodes = current_node [side];
		if (available_nodes.Length > 0) {  
			current_node = available_nodes [game_controller.mutationTables [id % game_controller.mutationTables.Length] % available_nodes.Length];
			SetDestination(current_node.real_transform.position);
		} else {
			current_node = null;
			Stop();
		}
	}

	int last_tracked_enemy_id = -1;
	
	public bool HasEnemyAround () {
		if (!ValidEnemy() ) {
			tracked_enemy = GetClosestActiveEnemy();
			if ( tracked_enemy != null ) last_tracked_enemy_id = tracked_enemy.id;
		}
		return tracked_enemy != null;
	}

	public bool ValidEnemy () {
		if (tracked_enemy == null) 
			return false;
		if (!tracked_enemy.isActiveAndEnabled | tracked_enemy.id != last_tracked_enemy_id) {
			tracked_enemy = null;
			return false;
		}
		return true;
	}

	public bool IsTrackedEnemyInRange () {
		if (HasEnemyAround ()) {	
			return !Physics.Linecast(tracked_enemy.transform.position, transform.position,walls_mask) &&
				(tracked_enemy.transform.position - transform.position).magnitude <= parameters.attack_range ;
		}
		return false;
	}

	AgentX2 GetClosestActiveEnemy () {
		var arounds = Physics.OverlapSphere (transform.position, parameters.sight_range);
		AgentX2 closest_enemy = null;
		float min_dis = float.MaxValue;

		foreach (var c in arounds) {
			if (c.transform == transform)
				continue;

			var k = c.GetComponent<AgentX2>();
			if ( k != null ) {
				if ( k.side == side ) continue;
				var p = c.ClosestPointOnBounds (transform.position);
				var d = (transform.position - p).magnitude;
				if (d  < min_dis ) {
					min_dis = d;
					closest_enemy = k;
				}
			}
		}
		return closest_enemy; 
	}

	//#### steering
	public float soft_steer_range;
	public float hard_steer_range;

	public float stop_distance;
	public float reach_distance;


	float speed = 1f;
	float turn_speed = 0.4f;
	
	Vector3 steering;
	
	public bool showLog;

	bool target_reached;
	Collider[] arounds_hard;
	Collider[] arounds_soft;

	void OnDrawGizmosSelected () {
		
		Gizmos.color = Color.white;
		
		foreach (var r in path.corners) {
			
			Gizmos.DrawSphere(r,0.08f);
		}
		
		UnityEditor.Handles.color = Color.red;
		UnityEditor.Handles.DrawWireDisc (transform.position, transform.up, parameters.sight_range);
		UnityEditor.Handles.color = Color.green;
		UnityEditor.Handles.DrawWireDisc (transform.position, transform.up, soft_steer_range);
		UnityEditor.Handles.color = Color.yellow;
		UnityEditor.Handles.DrawWireDisc (transform.position, transform.up, hard_steer_range);
		//UnityEditor.Handles.color = Color.red;
		//UnityEditor.Handles.DrawWireDisc (transform.position, transform.up, stop_distance);
		//UnityEditor.Handles.color = Color.blue;
		//UnityEditor.Handles.DrawWireDisc (transform.position, transform.up, reach_distance);
		
	}

	void ReselectTarget () {
		var r = GetClosestActiveEnemy ();
		if (r != null) {
			tracked_enemy = r;
			last_tracked_enemy_id = r.id;
		}
	}
	void Update () {
		// set destination
		if ( showLog ) Debug.Log("Update");
		if (target_reached) {
			if ( showLog ) Debug.Log("target_reached");
			if ( index < path.corners.Length -1 ) {
				index ++;
				target_reached = false;
			} 
		}
		Vector3 to_target= Vector3.zero;

		try { 
			to_target = path.corners [index] - transform.position;
			if ( showLog ) Debug.Log("to_target = " + to_target);
		} catch ( System.IndexOutOfRangeException e ) {
			Debug.Log("path.coner = " + path.corners.Length + " index = " + index);
			Debug.LogError(e.StackTrace);
		}

		if (type == UnitType.Melee & !react.IsAttaking () ) {
			RaycastHit shit;
			if (Physics.SphereCast (new Ray(transform.position, path.corners [index]), hard_steer_range*0.75f, out shit)) {
				AgentX2 k = shit.collider.GetComponent<AgentX2> ();
				if (k != null) {
					if (k.side != side) {
						ReselectTarget ();
					} else if (k.react.IsAttaking ()) {
						Process (transform, Time.deltaTime);
						return;
					}
				}
			}

		}

		steering = Vector3.zero;
		if (hard_steer_range > 0.1f) {
			// calculate hard-steering
			arounds_hard = Physics.OverlapSphere (transform.position, hard_steer_range);

			foreach (var c in arounds_hard) {
				if (c.transform == transform)
					continue;

				var p = c.ClosestPointOnBounds (transform.position);


				var u = transform.position - p;
				var len = Mathf.Clamp (hard_steer_range - u.magnitude, 0, hard_steer_range);
				var k = c.GetComponent<AgentX2>();
				if ( k != null ) {
					if ( k.type == UnitType.Melee ) len *=2f;
					if ( type == UnitType.Ranger & react.IsAttaking() ) len*=3;
				}
				steering += u.normalized * len;
			}
		}
 
		if (arounds_hard.Length <= 1 & !react.IsAttaking())  {
			if ( showLog ) Debug.Log("soft_steer");
			steering = Vector3.zero;

			arounds_soft = Physics.OverlapSphere (transform.position, soft_steer_range);
			foreach (var c in arounds_soft) {
				if (c.transform == transform)
					continue;

				var k = c.GetComponent<AgentX2>();
				if ( k != null ) {
					if ( k.side != side ) continue;
				}

				var p = c.ClosestPointOnBounds (transform.position);
				var u = transform.position - p;
				var len = Mathf.Clamp (soft_steer_range - u.magnitude, 0, soft_steer_range) ;
				steering += u.normalized * len;
			}
		}



		if (to_target.magnitude < stop_distance) {
			if ( showLog ) Debug.Log("magnitude < stop_distance");
			target_reached = true;
		}
		
		if (stop) {
			if ( showLog ) Debug.Log("stop");
			to_target = Vector3.zero;
		}

		Vector3 total;
		if (arounds_hard.Length <= 1)
			total = (steering.normalized + to_target.normalized).normalized;
		else
			total = steering.normalized;

		if ( showLog) Debug.DrawLine (transform.position, transform.position + to_target.normalized*2, Color.red);
		if ( showLog) Debug.DrawLine (transform.position, transform.position + steering.normalized, Color.green);
		if (showLog) {
			Debug.Log("total ="+total);
			Debug.DrawLine (transform.position, transform.position + total, Color.magenta);
		}
		
		if (total != Vector3.zero) {
			total.y = 0;
			var q = Quaternion.LookRotation (total);
			
			transform.rotation = Quaternion.Lerp (transform.rotation, q, turn_speed * Time.deltaTime);
			transform.position += total * speed * Time.deltaTime;
		}
	}
	
	static Vector3 Vec90 ( Vector3 first , Vector3 normal ) {
		Vector3 left90 = Quaternion.Euler (0, -90, 0) * first;
		left90.Normalize ();
		if (Vector3.Dot (left90, normal) > 0)
			return left90;
		return (Quaternion.Euler (0, 90, 0) * first).normalized;
	}

	Vector3 turn_side;
	bool keep_side;
	
	public float strength = 0.5f;

	public void Process (Transform unit, float delta ) {
		var dir = unit.forward;
		
		Vector3 left45 = Quaternion.Euler(0, -50, 0) * dir* soft_steer_range*0.75f;
		Vector3 right45 = Quaternion.Euler(0, 50, 0) * dir* soft_steer_range*0.75f;
		
		
		Vector3 detect_ray = dir * soft_steer_range;
		
		RaycastHit hit, left_hit, right_hit;
		
		Vector3 main_steer = Vector3.zero;
		Vector3 left_steer = Vector3.zero;
		Vector3 right_steer = Vector3.zero;
		
		turn_side = unit.right;
		
		bool r = false, l= false;
		if (l = Physics.SphereCast ( new Ray (unit.position, unit.position + left45), hard_steer_range * 0.4f, out left_hit)) {
			if (!keep_side) { 
				keep_side = true;
				turn_side = unit.right;
			}
			
			left_steer = Vec90(left_hit.point - unit.position,unit.forward) * 0.75f * strength ;
			
		} else if ( r = Physics.SphereCast ( new Ray (unit.position, unit.position + right45), hard_steer_range * 0.4f, out right_hit)) {
			if (!keep_side) {
				keep_side = true;
				turn_side = -unit.right;	
			}
			right_steer = ReflectLineCast(right_hit.point - unit.position,unit.forward).normalized * 0.75f * strength ;
		} 
		
		if (Physics.SphereCast (new Ray ( unit.position, unit.position + detect_ray) , hard_steer_range * 0.4f, out hit)) {
			main_steer = turn_side;
		} 
		
		if ( !r & !l) {
			keep_side = false;
		}
		
		
		dir = (dir+ main_steer + left_steer + right_steer).normalized;
		
		var q = Quaternion.LookRotation(dir);

		unit.rotation = Quaternion.Slerp (unit.rotation, q, turn_speed * Time.deltaTime);
		unit.position += dir * speed/3 * Time.deltaTime;
		return;
	}
	
	static Vector3 ReflectLineCast (Vector3 first , Vector3 normal ) {
		var u = first.normalized;
		if ( Vector3.Dot(u,normal ) == 1 ) return normal;
		var v1 = new Vector3(u.x,-u.y);
		float k = Vector3.Dot(v1,normal);
		if ( k > 0 ) return v1;
		else return new Vector3(-u.x,first.y,u.y);
	}
}
