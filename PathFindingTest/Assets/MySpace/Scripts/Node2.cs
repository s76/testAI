using UnityEngine;
using System.Collections;

public class Node2 : MonoBehaviour {
	public Transform real_transform;
	[SerializeField] Node2[] connectionsToSide01;
	[SerializeField] Node2[] connectionsToSide02;


	Node2[][] connections;

	public Node2[] this[UnitSide side] {
		get {
			return connections [(int)side];
		}
	}

	void Awake () {
		if (real_transform == null) {
			real_transform = transform;
		}
		connections = new Node2[System.Enum.GetNames(typeof(UnitSide)).Length][];
		connections [(int)UnitSide.Side01] = connectionsToSide02;
		connections [(int)UnitSide.Side02] = connectionsToSide01;
	}

	GameController gc;

	void Start () {
		gc = GameObject.FindObjectOfType<GameController> ();
	}
	void OnDrawGizmosSelected () {
		if (gc == null) {
			Gizmos.color = Color.magenta;
			foreach (var r in connectionsToSide02) {
				Gizmos.DrawLine (real_transform.position, r.real_transform.position);
			}
			Gizmos.color = Color.cyan;
			if (real_transform != null)
				Gizmos.DrawSphere (real_transform.position, 0.2f);
			return;
		}
		if (!gc.node_display_fromSide02) {
			Gizmos.color = Color.magenta;
			foreach (var r in connectionsToSide02) {
				Gizmos.DrawLine (real_transform.position, r.real_transform.position);
			}
			Gizmos.color = Color.cyan;
			if (real_transform != null)
				Gizmos.DrawSphere (real_transform.position, 0.2f);
		} else {
			Gizmos.color = Color.cyan;
			foreach (var r in connectionsToSide01) {
				Gizmos.DrawLine (real_transform.position, r.real_transform.position);
			}
			Gizmos.color = Color.magenta;
			if (real_transform != null)
				Gizmos.DrawSphere (real_transform.position, 0.2f);
		}
	}
}
