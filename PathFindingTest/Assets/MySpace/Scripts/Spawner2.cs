using UnityEngine;
using System.Collections.Generic;

public class Spawner2 : MonoBehaviour {
	Dictionary<UnitType, UnitPool > dict;
	GameController gameController;

	public Node2 startNode;
	int index;

	Vector3[,] start_pos;

	public float offset;

	void Awake () {
		dict = new Dictionary<UnitType, UnitPool> (2);

		var l = transform.GetComponentsInChildren<UnitPool> ();
		foreach (var k in l) {
			dict.Add(k.prefab.type, k);
		}	

		start_pos = new Vector3[11,5];

		for (int i = 0; i < 11; i ++) {
			for(int j = 0; j < 5; j ++ ) {
				start_pos[i,j] = new Vector3((j-2 )*offset, 0, (i-5)*offset);
			}
		}
		index = 0;
	}

	void Start () {
		gameController = GameObject.FindObjectOfType<GameController> ();
	}

	public AgentX2 SpawnUnit ( UnitType t ) {
		UnitPool p = null;
		dict.TryGetValue (t, out p);
		var g = p.Get ();
		int i = index % 11;
		int j = (index - ( (int)(index / 11) * 11) )% 5;

		try { 
			g.SetSpawner (this, start_pos[i,j]);
			index ++;
		} catch (System.IndexOutOfRangeException e ) {
			Debug.Log("index = " + index + " i,j= " + i + ","+ j );
		}
		return g;
	}
}
