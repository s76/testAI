using UnityEngine;
using System.Collections;

public class TestCollider : MonoBehaviour {
	Vector3[] p = new Vector3[0];
	public float radius;

	void OnDrawGizmos () {
		UnityEditor.Handles.color = Color.red;
		UnityEditor.Handles.DrawWireDisc (transform.position, transform.up, radius);
		foreach (var k in p) {
			Gizmos.color = Color.green;
			Gizmos.DrawLine (transform.position, k);
		}
	}

	void OnGUI () {
		if ( GUILayout.Button ("Cast")){
			var u = Physics.OverlapSphere(transform.position,radius);
			p = new Vector3[u.Length];
			for(int i=0; i < u.Length; i ++ ) {
				p[i] = u[i].ClosestPointOnBounds(transform.position);
				
			}
		}
	}
}
