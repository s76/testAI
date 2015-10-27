using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using React;

using Action = System.Collections.Generic.IEnumerator<React.NodeResult>;

public class TestNavMove : MonoBehaviour {
	public Vector3 off;
	Transform target;
	NavMeshAgent agent;
	Reactor reactor;

	void Awake () {
		agent = GetComponent<NavMeshAgent> ();
		reactor = GetComponent<Reactor> ();
		target = GameObject.Find ("target").transform;
	}


	void Start () {
		agent.SetDestination (target.position);
	}

	public enum MState { MoveToTarget, MoveAround }
	public MState state;

	public void MoveToTarget () {
		agent.SetDestination (target.position);
		state = MState.MoveToTarget;
	}

	public void MoveAround () {
		NavMeshPath path = new NavMeshPath();
		agent.CalculatePath(temp_des, path);
	
		if (path.status == NavMeshPathStatus.PathPartial) {
			MoveToTarget ();
			Debug.Log("MoveAround1 ");
		} else {
			Debug.Log("MoveAround2");
			agent.SetPath (path);
		}

		state = MState.MoveAround;
	}

	public bool IsMoveToTarget () {
		return state == MState.MoveToTarget;
	}

	public bool IsMoveAround () {
		return state == MState.MoveAround;
	}

	public float check_range = 1f;
	float timer = 0;

	public float des_tolerance = 1f;
	Vector3 temp_des;

	public float move_around_len = 2f;

	public bool IsArrivedAtDes () {
	//	if ( IsMoveAround () ) { 

		if ((transform.position - temp_des).sqrMagnitude > des_tolerance) {
			Debug.Log("move to tmp");
			return true;
		} else {
			Debug.Log("Reached temp des");
			agent.SetDestination(target.position);
			return false;
		}
		//}
	}

	enum Dir { left, right , back, forw}
	Dir lastDir; 

	public float check_freq;

	public Action Evaluate () {
		yield return NodeResult.Continue;
		Debug.Log ("Eval");
		timer += reactor.TickDuration;
		if (timer > check_freq) {
			timer = 0; 
			Vector3 fw = transform.forward;
			Vector3 left = Quaternion.Euler(0, -60, 0) * fw;
			Vector3 right = Quaternion.Euler(0, 60, 0) * fw;

			bool _forw=false, _left=false , _right=false;
			
			RaycastHit hit_left, hit_right, hit_front ;
			if (_forw = Physics.Raycast(transform.position, fw,out hit_front , check_range ) ) {
				Debug.Log("hit front");
				_left = Physics.Raycast(transform.position, left, out hit_left, check_range *0.5f );
				_right = Physics.Raycast(transform.position, right,  out hit_right, check_range *0.5f);
			}

			if ( _left & _right ) {
				
				Debug.Log("should go back now");
				temp_des = transform.position +  -(hit_front.transform.position - transform.position).normalized*move_around_len;
				lastDir = Dir.back;
			} else if ( _left ) {
				Debug.Log("should turn right now");
				temp_des = transform.position + Quaternion.Euler(0, 90, 0) * (hit_left.transform.position - transform.position).normalized * move_around_len;
				lastDir = Dir.right;
			} else if ( _right ) {
				Debug.Log("should turn left now");
				temp_des = transform.position + Quaternion.Euler(0, -90, 0) * (hit_right.transform.position - transform.position).normalized * move_around_len;
				lastDir = Dir.left;
			} 

			if ( _forw & ( !_left & !_right ) ) { 
				if ( lastDir == Dir.left ) {
					temp_des = transform.position + Quaternion.Euler(0, -90, 0) * (hit_front.transform.position - transform.position).normalized * move_around_len;
				}
				if ( lastDir == Dir.right ) {
					temp_des = transform.position + Quaternion.Euler(0, 90, 0) * (hit_front.transform.position - transform.position).normalized * move_around_len;
				}
				if ( lastDir == Dir.back ) {
					temp_des = transform.position + -(hit_front.transform.position - transform.position).normalized*move_around_len;
				}

			}

			if ( _forw ) {
				MoveAround();
			} else {
				MoveToTarget();
			}

		}
		yield return NodeResult.Continue;
	}

	void OnDrawGizmos () {
		if (IsMoveAround ()) {
			Gizmos.DrawSphere( temp_des,0.2f);
		}
		UnityEditor.Handles.color = Color.cyan;
		UnityEditor.Handles.DrawWireDisc (transform.position, transform.up, des_tolerance);

		Vector3 fw = transform.forward;
		Vector3 left = Quaternion.Euler(0, -60, 0) * fw;
		Vector3 right = Quaternion.Euler(0, 60, 0) * fw;

		Gizmos.color = Color.yellow;
		Gizmos.DrawLine ( transform.position, transform.position + fw* check_range);
		Gizmos.color = Color.green;
		Gizmos.DrawLine ( transform.position, transform.position + left* check_range *0.5f);
		Gizmos.color = Color.magenta;
		Gizmos.DrawLine ( transform.position, transform.position + right* check_range *0.5f);

		if (agent != null) {
			if (agent.hasPath) {
				foreach ( var r in agent.path.corners ) {
					
					Gizmos.color = Color.yellow;
					Gizmos.DrawSphere ( r,0.15f);
				}
			}
		}
	} 
}
