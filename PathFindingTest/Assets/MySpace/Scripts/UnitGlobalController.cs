using UnityEngine;
using System.Collections.Generic;

public class UnitGlobalController : MonoBehaviour {
	public int randomSeed = 0;

	Dictionary<UnitSide, List<UnitAI>> units = new Dictionary<UnitSide, List<UnitAI>>();

	void Awake () {
		units.Add (UnitSide.Side01, new List<UnitAI> ());
		units.Add (UnitSide.Side02, new List<UnitAI> ());
	}

	public Spawner base01 { get; private set; } 
	public Spawner base02 { get; private set; }

	public int[] mutationTables { get; private set; }

	void Start () {
		var r = GameObject.FindObjectsOfType<Spawner> ();
		base01 = r [0].side == UnitSide.Side01 ? r [0] : r [1];
		base02 = r [1].side == UnitSide.Side02 ? r [1] : r [0];
		Random.seed = randomSeed;
		mutationTables = new int[128];
		for (int i=0; i < 128; i ++) {
			mutationTables[i] = Random.Range(0,1000);
		}
	}

	public void Register ( UnitAI unit ) {
		if ( unit == null) throw new UnityException ("Unit is null");
		List<UnitAI> r = null;
		units.TryGetValue (unit.core.side, out r);

		if (r.Contains (unit)) {
			Debug.LogError ("Unit is already in list");
			return;
		}

		r.Add (unit);
	}

	public List<UnitAI> GetAlliens ( UnitAI request_source ) {
		List<UnitAI> r = null;
		units.TryGetValue (request_source.core.side, out r);
		return r;
	}

	public List<UnitAI> GetEnemies ( UnitAI request_source ) {
		List<UnitAI> r = null;
		units.TryGetValue (request_source.core.side == UnitSide.Side01? UnitSide.Side02 : UnitSide.Side01, out r);
		return r;
	}

	public float interval = 2;
	public int k;
	float timer = 0;
	void Update () {
		timer -= Time.deltaTime;

		if (timer < 0) {
			timer += interval;
			for(int i=0; i < k ; i ++ ) {
				base01.SpawnMelee();
			}
			for(int i=0; i < k ; i ++ ) {
				
				base01.SpawnRanger();
			}

			for(int i=0; i < k ; i ++ ) {
				base02.SpawnMelee();
			}
			for(int i=0; i < k ; i ++ ) {
				
				base02.SpawnRanger();
			}
		} 

	}

	static UnitGlobalController _instance;

	public static UnitGlobalController Instance {
		get {
			if ( _instance == null ) {
				_instance = GameObject.FindObjectOfType<UnitGlobalController>();
			}
			return _instance;
		}
	}

}
