using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentX : MonoBehaviour {
	public void SetDestination (Vector3 v) {
		stop = false;
		target_reached = false;
		index = 0;
		agent.CalculatePath (v, path);
	}

	public void Stop () {
		stop = true;
	}

	bool stop;
	Vector3 target;
	
	int index;
	
	
	NavMeshAgent agent;
	NavMeshPath path;
	UnitCore core;

	void Awake () {
		arounds = new Collider[0];
		
		agent = GetComponent<NavMeshAgent> ();
		path  = new NavMeshPath();
		core = GetComponent<UnitCore> ();
	}	
	
	public void SetTarget (Vector3 _target ) {
		target = _target;
	}
	
	public float radius;
	public float speed;
	public float min_steering_strength;
	public float turn_speed;
	public float stop_distance;
	public float reach_distance;
	
	int last_stuck_around_len;
	
	Vector3 steering;
	Vector3 total = Vector3.zero;
	
	public bool showLog;
	bool target_reached;
	Collider[] arounds;

	void OnDrawGizmosSelected () {

		Gizmos.color = Color.white;

		foreach (var r in path.corners) {

			Gizmos.DrawSphere(r,0.2f);
		}

		UnityEditor.Handles.color = Color.green;
		UnityEditor.Handles.DrawWireDisc (transform.position, transform.up, radius);
		UnityEditor.Handles.color = Color.red;
		UnityEditor.Handles.DrawWireDisc (transform.position, transform.up, stop_distance);

	}

	void Update () {
		if ( showLog ) Debug.Log("Update");
		if (target_reached) {
			if ( showLog ) Debug.Log("target_reached");
			if ( index < path.corners.Length -1 ) {
				index ++;
				target_reached = false;
			}
		}
		
		Vector3 to_target = path.corners [index] - transform.position;

		if (showLog) {
	//		Debug.DrawLine (transform.position, path.corners [index], Color.magenta);
		}
		if (to_target.magnitude < stop_distance) {
			if ( showLog ) Debug.Log("magnitude < stop_distance");
			target_reached = true;
			return;
		}

		if (stop) {
			if ( showLog ) Debug.Log("stop");
			to_target = Vector3.zero;
		}

//		if (to_target.magnitude < reach_distance) {
//			if ( showLog ) Debug.Log("to_target.magnitude < reach_distance");
//			target_reached = true;
//			to_target = Vector3.zero;
//		}
//		
		
		steering = Vector3.zero;
		
		arounds = Physics.OverlapSphere (transform.position, radius);
		foreach (var c in arounds) {
			if ( c.transform == transform ) continue;
			var _core = c.GetComponent<UnitCore>();
			if ( _core != null ) {
				if ( _core.side != core.side ) {
				}
			}
			var u = transform.position - c.transform.position;
			var len =  Mathf.Clamp(radius - u.magnitude,0,radius);
			//Debug.Log("len = " + len);
			
			steering += u.normalized * len;
		}
		
		
		Vector3 _total;
		
		_total = (steering.normalized+ to_target.normalized).normalized;

		total = _total;

		
		if ( showLog) Debug.DrawLine (transform.position, transform.position + to_target.normalized*2, Color.red);
		if ( showLog) Debug.DrawLine (transform.position, transform.position + steering.normalized, Color.green);
		if ( showLog) Debug.DrawLine (transform.position, transform.position + total, Color.magenta);
		
		if (total != Vector3.zero) {
			total.y = 0;
			var q = Quaternion.LookRotation (total);
			
			transform.rotation = Quaternion.Lerp (transform.rotation, q, turn_speed * Time.deltaTime);
			transform.position += total * speed * Time.deltaTime;
		}
	}

	
}
