using UnityEngine;
using System.Collections;

public class Node : MonoBehaviour {
	public Transform real_transform;
	public Node[] connectedTo;
	public Node[] connectedFrom;

	void Awake () {
		if (real_transform == null) {
			real_transform = transform;
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.magenta;
		if ( real_transform != null ) Gizmos.DrawSphere(real_transform.position, 0.5f);
		foreach ( var r in connectedTo ) {
			Gizmos.DrawLine(real_transform.position, r.real_transform.position);
		}
	}
}
