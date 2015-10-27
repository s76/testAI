using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	public Spawner2[] spawners { get; private set; }
	public Barier[] bariers { get; private set; }
	public int[] mutationTables { get; private set; }

	public int game_seed;

	Dictionary<UnitSide, List<UnitState> > db;
	List<UnitState> side01;
	List<UnitState> side02;

	public bool node_display_fromSide02=false;

	void Awake () {
		side01 = new List<UnitState> ();
		side02 = new List<UnitState> ();

		Random.seed = game_seed;
		mutationTables = new int[128];
		for (int i=0; i < 128; i ++) {
			mutationTables[i] = Random.Range(0,1000);
		}
	}

	void Start () {
		bariers = GameObject.FindObjectsOfType<Barier> ();

		spawners = GameObject.FindObjectsOfType<Spawner2> ();
		StartCoroutine (SpawnUnit ());
	}

	public double timer { get; private set; } 

	void Update () {
		timer += Time.deltaTime;
	}

	public float spawnInterval=10f;
	public int spawnAmountPerUnitType = 3;

	IEnumerator SpawnUnit () {
		while (true) {
			foreach ( var s in spawners ) {
				for(int i=0; i < spawnAmountPerUnitType; i ++ ) {
					s.SpawnUnit(UnitType.Melee);
				//	s.SpawnUnit(UnitType.Ranger);
				}
			}
			yield return new WaitForSeconds(spawnInterval);
		}
	}

	public void RegisterUnit ( UnitState unit ) {
		var l = unit.side == UnitSide.Side01 ? side01 : side02; 
		if ( !l.Contains(unit) ) l.Add (unit);
	}

	public List<UnitState> GetUnitListOfOppositeSide ( UnitSide side ) {
		return side == UnitSide.Side01? side02 : side01;
	}
}
