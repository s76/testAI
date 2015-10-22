using UnityEngine;
using System.Collections;

public class SimplePlayerControl : MonoBehaviour {
	public float moveSpeed = 0.1f;
	public float turnSpeed = 45f;
	
	
	CharacterController controller;
	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
	}
	
	
	void Update () {
		
		transform.Rotate(0, Input.GetAxis ("Horizontal") * turnSpeed, 0);
		var forward = transform.forward;
		var curSpeed = moveSpeed * Input.GetAxis ("Vertical");

		controller.SimpleMove(forward * curSpeed);
	}
}
