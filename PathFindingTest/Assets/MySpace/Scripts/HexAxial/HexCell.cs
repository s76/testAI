using UnityEngine;
using System.Collections.Generic;

public class Int2 { 
	public int x,y;
	public Int2 (int _x, int _y ) {
		x = _x;
		y = _y;
	}
}

public class HexCell  {
	public HexIndex hex_index { get; private set; }
	public HexCenter hex_center { get; private set; }

	Vector3 relative_center;
	public UnitState unit;

	public Vector3 position {
		get {
			return hex_center.transform.position + relative_center;
		}
	}
	static Int2[] directions = {
			new Int2 (-1, 1), new Int2 (0, 1),
		new Int2 (-1, 0), 			new Int2 (1, 0),
			new Int2 (0, -1), new Int2 (1, -1)
	};

	int[] best_moves;
	int[] normal_moves;
	int[] bad_moves;

	public HexCell ( HexIndex _index, HexCenter _center ) {
		hex_center = _center;
		relative_center = hex_to_pixel (_index);
		hex_index = _index;

		var abs_center = this.position;
		if( last_world_pos != abs_center) {
			last_world_pos = abs_center;
			RecalculateConners ();
		}

	}
	//!! for optimization : table cos and sin for each conners

	static float sqrt3 = Mathf.Sqrt (3f) ;

	Vector3 hex_to_pixel(HexIndex hex) {
		var x = hex_center.size * sqrt3 * (hex.q + hex.r / 2f);
		var z = hex_center.size * 3f/2f * hex.r;
		return new Vector3(x, 0, z);
	}

	public HexCell GetNearestEmptyCell () {
		foreach (var d in directions) {
			HexIndex newindex = new HexIndex(hex_index.q + d.x , hex_index.r + d.y);
			if ( hex_center.ValidIndex(newindex) ) {
				HexCell c = hex_center.GetCell(newindex);
				
			}
		}
		return null;
	}
	
	Vector3 last_world_pos = Vector3.zero;
	Vector3[] conners = new Vector3[6];

	void RecalculateConners () {
		for(int i=0; i < 6; i ++ ) {
			conners[i] = hex_corner (last_world_pos, hex_center.size, i);
		}
	}

	public void DrawGizmos () {
		var abs_center = this.position;
		if( last_world_pos != abs_center) {
			last_world_pos = abs_center;
			RecalculateConners ();
		}

		for (int i=0; i < conners.Length; i ++) {
			Gizmos.color = Color.grey;
			Gizmos.DrawLine (conners [i], conners [(i + 1) % conners.Length]);
		}
	}
	
	Vector3 hex_corner(Vector3 center,float  size,int conner_index ) {
		var angle_deg = 60 * conner_index   + 30;
		var angle_rad = Mathf.Deg2Rad * angle_deg;
		return new Vector3(center.x + size * Mathf.Cos(angle_rad),0, center.z + size * Mathf.Sin(angle_rad) );
	}
}
