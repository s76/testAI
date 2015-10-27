using UnityEngine;
using System.Collections;

public class TestNavMesh2 : MonoBehaviour {
	Transform tar;

	void Start () {
		tar = GameObject.Find ("target").transform;
	}

	// Update is called once per frame
	void Update () {
		GetComponent<NavMeshAgent> ().SetDestination (tar.position);
	}

	void OnGUI () {
		if ( GUILayout.Button("Resetpath ") ) {
			GetComponent<NavMeshAgent> ().ResetPath();
			GetComponent<NavMeshAgent> ().SetDestination (tar.position);
		}
	}
}
