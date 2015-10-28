using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using React;

using Action = System.Collections.Generic.IEnumerator<React.NodeResult>;

public class TestNavMove : MonoBehaviour {
	Transform target;
	NavMeshAgent agent;

	void Awake () {
		agent = GetComponent<NavMeshAgent> ();
		target = GameObject.Find ("target").transform;
	}


	void Start () {
		agent.SetDestination (target.position);
	}

	void Update () {
		Debug.Log ("agent.pathPending=" + agent.pathPending);
		Debug.Log ("agent.pathStatus=" + agent.pathStatus);
		Debug.Log ("agent.isPathStale=" + agent.isPathStale);
		Debug.Log ("agent.hasPath=" + agent.hasPath);
		Debug.Log ("agent.pathEndPosition=" + agent.pathEndPosition);
		Debug.Log ("agent.destination=" + agent.destination);
	}

	void OnDrawGizmos () {

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
