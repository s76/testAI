using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ste01 : MonoBehaviour {
	Vector3 target;
	static List<Ste01 > list = new List<Ste01>();


	void Start () {
		list.Add (this);
		StartCoroutine (_sc=StuckCheck ());
		last_pos = transform.position;
		arounds = new Collider[0];
	}	

	public void SetTarget (Vector3 _target ) {
		target = _target;
	}

	Vector3 last_pos;
	bool stuck;
	public float min_remain_dis=0.2f;
	Vector3 last_stuck_steering;

	IEnumerator _sc;

	IEnumerator StuckCheck () {
		while (true) {
			yield return new WaitForSeconds (1.5f);
			if ( target_reached ) {
				stuck = false;
				break;
			}
			if (showLog ) Debug.Log("????");
			if ((transform.position - last_pos).magnitude < min_remain_dis) {
				if (showLog)
					Debug.Log ("STUCK !");
				stuck = true; 
				last_stuck_around_len = arounds.Length;
				break;
			}

			last_pos = transform.position;
		}

		Debug.Log ("stuck check ends");
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

	void Update () {
		Vector3 to_target = target - transform.position;

		if (to_target.magnitude < stop_distance)
			return;
		
		if (to_target.magnitude < reach_distance) {
			target_reached = true;
			to_target = Vector3.zero;
		}

		if (stuck) {
			to_target = Vector3.zero;
		}

		steering = Vector3.zero;

		arounds = Physics.OverlapSphere (transform.position, radius);
		foreach (var c in arounds) {
			if ( c.transform == transform ) continue;

			var u = transform.position - c.transform.position;
			var len =  Mathf.Clamp(radius - u.magnitude,0,radius);
			//Debug.Log("len = " + len);

			steering += u.normalized * len;
		}


		Vector3 _total;

		if (stuck & ( last_stuck_around_len != arounds.Length)) {
			stuck = false;
			if (_sc != null ) StopCoroutine(_sc);
			StartCoroutine(_sc = StuckCheck());
		}

		if (arounds.Length == 0) { 
			_total = to_target;
		} else {
			_total = (steering.normalized*1.3f + to_target.normalized).normalized;
			
			if ( showLog & stuck ) Debug.Log("_total =" + _total);
		}

		if ( showLog) Debug.DrawLine (transform.position, transform.position + to_target.normalized*2, Color.green);
		if ( showLog) Debug.DrawLine (transform.position, transform.position + steering.normalized*2, Color.red);

		if ( showLog) Debug.DrawLine (transform.position, transform.position + _total.normalized, Color.yellow);

		total = _total;

		if (total != Vector3.zero) {
			var q = Quaternion.LookRotation (total);
		
			transform.rotation = Quaternion.Lerp (transform.rotation, q, turn_speed * Time.deltaTime);
			transform.position += total * speed * Time.deltaTime;
		}
	}

	void OnDrawGizmosSelected () {
		UnityEditor.Handles.color = Color.green;
		UnityEditor.Handles.DrawWireDisc (transform.position, transform.up, reach_distance);
		UnityEditor.Handles.color = Color.red;
		UnityEditor.Handles.DrawWireDisc (transform.position, transform.up, stop_distance);
	}

}
