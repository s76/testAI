using UnityEngine;
using System.Collections.Generic;

public class HexCenter : MonoBehaviour {
	Dictionary<HexIndex,HexCell> list;

	public int hex_size;
	public float size=1;

	public void Awake () {
		list = new Dictionary<HexIndex, HexCell> (new HexIndexComparer ());

		// test 
		for (int y = -hex_size +1; y < hex_size ; y ++) {
			for (int x = -hex_size + 1- (y < 0 ? y : 0); x < hex_size - (y > 0 ? y : 0); x ++) {
				var id = new HexIndex(x,y);
				var h = new HexCell(id,size);
				//Debug.Log(id + " h= " + h.relative_center);
				//Debug.Log ("revers = " + ToHexIndex (transform.position + h.relative_center));
				list.Add(id,h);
			}
		}

		Debug.Log(cube_to_hex(new Vector3(0,0,0)));
		Debug.Log(cube_to_hex(new Vector3(1,0,0)));
		Debug.Log(cube_to_hex(new Vector3(0,1,0)));
		Debug.Log(cube_to_hex(new Vector3(0,0,1)));
		Debug.Log(cube_to_hex(new Vector3(1,0,1)));
		Debug.Log(cube_to_hex(new Vector3(1,1,1)));
	          
	}
	
	static float sqrt3 = Mathf.Sqrt (3f) ;

	//!!! assuming that center of hexagon is object itself
	public HexIndex ToHexIndex ( Vector3 position ) {
		int q = (int)((position.x * sqrt3 / 3f - position.z / 3f) / size);
		int r = (int)(position.z * 2f / 3f / size);
		return hex_round(new HexIndex(q, r));	
	}

	void OnDrawGizmos () {
		if (list == null)
			return;
		foreach (var r in list.Values) {
			r.Draw(transform.position);
		}
		Gizmos.color = Color.red;
		Gizmos.DrawSphere (transform.position, 0.05f);
	}


	public bool ValidIndex (HexIndex index ) {
		return Mathf.Abs (index.q) < size - 1 & Mathf.Abs (index.r) < size - 1;
	}

	public HexCell RegisterCell ( HexIndex index ) {
		HexCell cell = new HexCell (index, size);
		list.Add (index, cell);
		return cell;
	}

	public HexCell GetCell ( HexIndex index ) {
		HexCell cell = null;
		list.TryGetValue (index, out cell);
		return cell;
	}
	#region hide
	HexIndex cube_to_hex(Vector3 h) { // axial
		var q = (int)h.x;
		var r = (int)h.z;
		return new HexIndex(q, r);
	}
			
	Vector3 hex_to_cube(HexIndex h) { // # axial
		var x = h.q;
		var z = h.r;
		var y = -x-z;
		return new Vector3(x, y, z);
	}

	HexIndex hex_round (HexIndex h) {
		return cube_to_hex (cube_round (hex_to_cube (h)));
	}

	Vector3 cube_round(Vector3 h) {
		var rx = Mathf.Round(h.x);
		var ry = Mathf.Round (h.y);
		var rz = Mathf.Round (h.z);

		var x_diff = Mathf.Abs (rx - h.x);
		var y_diff = Mathf.Abs (ry - h.y);
		var z_diff = Mathf.Abs (rz - h.z);

		if ( x_diff > y_diff & x_diff > z_diff ) {
			rx = -ry-rz;
		}
		else if (y_diff > z_diff ) {
			ry = -rx-rz;
		}
		else {
			rz = -rx-ry;
		}
		return new Vector3(rx, ry, rz);
	}


	Vector3 hex_to_pixel(HexIndex hex) {
		var x = size * sqrt3 * (hex.q + hex.r / 2);
		var y = size * 3/2 * hex.r;
		return new Vector3(x, y);
	}
	

	Vector3 hex_corner(Vector3 center,int  size,int conner_index ) {
		var angle_deg = 60 * conner_index   + 30;
		var angle_rad = Mathf.Deg2Rad * angle_deg;
		return new Vector3(center.x + size * Mathf.Cos(angle_rad),0, center.z + size * Mathf.Sin(angle_rad) );
	}

	#endregion
}
