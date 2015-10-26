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
	public HexCenter hex_center;
	public HexIndex index { get; private set; }

	public Vector3 postion {
		get {
			return 
		}
	}
	public Vector3 relative_center;
	public UnitState unit;

	float size;
	
	static Int2[] directions = {
			new Int2 (-1, 1), new Int2 (0, 1),
		new Int2 (-1, 0), 			new Int2 (1, 0),
			new Int2 (0, -1), new Int2 (1, -1)
	};

	int[] best_moves;
	int[] normal_moves;
	int[] bad_moves;

	public HexCell ( HexIndex _index, float _size ) {
		size = _size;
		relative_center = hex_to_pixel (_index);
		index = _index;
	}
	//!! for optimization : table cos and sin for each conners

	static float sqrt3 = Mathf.Sqrt (3f) ;

	Vector3 hex_to_pixel(HexIndex hex) {
		var x = size * sqrt3 * (hex.q + hex.r / 2f);
		var z = size * 3f/2f * hex.r;
		return new Vector3(x, 0, z);
	}

	public HexCell FreeNeighbCell (HexCenter hex_center ) {
		foreach (var d in directions) {
			HexIndex newindex = new HexIndex(index.q + d.x , index.r + d.y);
			if ( hex_center.ValidIndex(newindex) ) {
				HexCell c = hex_center.GetCell(newindex);
				if ( c == null ) {
					c = hex_center.RegisterCell(newindex);
					return c;
				}
				else if ( c.unit == null ) return c;
			}
		}
		return null;
	}


	public void Draw (Vector3 hex_center) {
		var abs_center = hex_center + relative_center;
		Vector3[] conners = {
			hex_corner (abs_center, size, 0),
			hex_corner (abs_center, size, 1),
			hex_corner (abs_center, size, 2),
			hex_corner (abs_center, size, 3),
			hex_corner (abs_center, size, 4),
			hex_corner (abs_center, size, 5)
		};
		for (int i=0; i < conners.Length; i ++) {
			Gizmos.color = Color.grey;
			Gizmos.DrawLine (conners [i], conners [(i + 1) % conners.Length]);
		}
	
		Gizmos.DrawSphere (abs_center, 0.05f);

	}
	
	Vector3 hex_corner(Vector3 center,float  size,int conner_index ) {
		var angle_deg = 60 * conner_index   + 30;
		var angle_rad = Mathf.Deg2Rad * angle_deg;
		return new Vector3(center.x + size * Mathf.Cos(angle_rad),0, center.z + size * Mathf.Sin(angle_rad) );
	}
}
