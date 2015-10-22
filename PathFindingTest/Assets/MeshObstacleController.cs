using UnityEngine;
using System.Collections;

public class MeshObstacleController : MonoBehaviour {
	NavMeshObstacle obs;

	void Awake () {
		obs = GetComponent<NavMeshObstacle> ();
	}
	void Update () {

		transform.position = new Vector3 (transform.position.x, transform.position.y, 8 * Mathf.Sin (Time.time));
	}
}
