using UnityEngine;
using System.Collections;

public class SidePoint {
	public Vector3 value;
	public Vector3[] closest_conner_points;
}

public class Barier : MonoBehaviour {
	[SerializeField] SidePointEditor[] _side_points;

	public SidePoint[] side_points { get; private set; }
	
	static int walls_mask = LayerMask.GetMask("walls");

	void Awake () {
		var l = transform.GetComponentsInChildren<SidePointEditor> ();
		side_points = new SidePoint[l.Length];
		for (int i = 0; i < side_points.Length; i ++) {
			var sp = new SidePoint();
			sp.value = l[i].transform.position;
			sp.closest_conner_points = new Vector3[l[i].closest_conner_points.Length];
			for(int j = 0 ; j < l[i].closest_conner_points.Length; j ++ ) {
				sp.closest_conner_points[j] = l[i].closest_conner_points[j].transform.position;
			}
			side_points[i] = sp;
		}

		var m = transform.GetComponentsInChildren<Transform> ();
		foreach (var _m in m) {
			if ( _m == transform ) continue;
			DestroyObject(_m.gameObject);
		}
		_side_points = null;
	}

	public void TestIfBarierSuitableToHide ( UnitCore unit, Vector3 precalculated_toEnemyVector, ref float global_min, ref Barier barier ) {
		var unit_position = unit.transform.position;



		//!!! CAUTION !!!
		// only work with 4 side points and 4 conner points !!!!
		float local_min = global_min;
		bool min_changed = false;
		Vector3 closest = Vector3.zero;
		Vector3 tmp;
		tmp = side_points[0].value - unit_position;
		if ( tmp.magnitude < global_min ) { 
			closest = side_points[0].value;
			global_min = tmp.magnitude;
			min_changed = true;
		}
		foreach (var r in side_points[0].closest_conner_points) {
			tmp = r - unit_position;
			if ( tmp.magnitude < local_min ) { 
				closest = r;
				local_min = tmp.magnitude;
				min_changed = true;
			}
		}

		tmp = side_points[2].value - unit_position;
		if ( tmp.magnitude < local_min ) { 
			closest = side_points[2].value;
			local_min = tmp.magnitude;
			min_changed = true;
		}
		foreach (var r in side_points[2].closest_conner_points) {
			tmp = r - unit_position;
			if ( tmp.magnitude < local_min ) { 
				closest = r;
				local_min = tmp.magnitude;
				min_changed = true;
			}
		}
		//!!! END CAUTION !!!

		if (min_changed) {
			if ( Physics.Linecast(closest, unit.transform.position, walls_mask) ) return;

			// if we must run over enemy to reach the barier then no
			if (Vector3.Angle (precalculated_toEnemyVector, closest -unit_position) > 50f) {
				barier = this;
				global_min = local_min;
			}
		}
	}

	void OnDrawGizmosSelected () {
		if (side_points != null) {
			foreach (var sp in side_points) {
				Gizmos.color = Color.white;
				foreach (var cp in sp.closest_conner_points) {
					Gizmos.DrawLine (sp.value, cp);
				}
				Gizmos.DrawSphere (sp.value, 0.1f);

			}
		}

		if (_side_points != null) {
			foreach (var sp in _side_points) {
				Gizmos.color = Color.white;
				if ( sp.closest_conner_points != null) {
					foreach (var cp in sp.closest_conner_points) {
						Gizmos.DrawLine (sp.transform.position, cp.position);
					}
				}
				Gizmos.DrawSphere (sp.transform.position, 0.05f);
				
			}
		}
	}
}
