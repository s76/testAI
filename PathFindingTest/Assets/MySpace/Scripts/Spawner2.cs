using UnityEngine;
using System.Collections.Generic;

public class Spawner2 : MonoBehaviour {
	Dictionary<UnitType, UnitPool > dict;
	GameController gameController;

	public Node2 startNode;
	
	void Awake () {
		dict = new Dictionary<UnitType, UnitPool> (2);

		var l = transform.GetComponentsInChildren<UnitPool> ();
		foreach (var k in l) {
			dict.Add(k.prefab.type, k);
		}	
	}

	void Start () {
		gameController = GameObject.FindObjectOfType<GameController> ();
	}

	public UnitCore SpawnUnit ( UnitType t ) {
		UnitPool p = null;
		dict.TryGetValue (t, out p);
		var g = p.Get ();
		g.SetSpawner (this);
		return g;
	}
}
