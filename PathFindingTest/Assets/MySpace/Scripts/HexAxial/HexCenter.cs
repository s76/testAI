using UnityEngine;
using System.Collections.Generic;

public class HexCenter : MonoBehaviour {
	Dictionary<HexIndex,HexCell> list;

	public int hex_size;
	public float size=1;
	
	static float sqrt3 = Mathf.Sqrt (3f) ;

	public void Awake () {
		list = new Dictionary<HexIndex, HexCell> (new HexIndexComparer ());

		for (int y = -hex_size +1; y < hex_size ; y ++) {
			for (int x = -hex_size + 1- (y < 0 ? y : 0); x < hex_size - (y > 0 ? y : 0); x ++) {
				var id = new HexIndex(x,y);
				var h = new HexCell(id,this);
				var reversed = ToHexIndex (h.position);
				list.Add(id,h);
			}
		}    
	}
	
	//##########################################################################

	//!!! assuming that center of hexagon is object itself
	public HexIndex ToHexIndex ( Vector3 position ) {
		float _q = (position.x * sqrt3 / 3f - position.z / 3f) / size;
		float _r = (position.z * 2f / 3f / size);
		return hex_round(new Vector2(_q,_r));	
	}


	public bool ValidIndex (HexIndex index ) {
		return Mathf.Abs (index.q) < size - 1 & Mathf.Abs (index.r) < size - 1;
	}

	public HexCell GetCell ( HexIndex index ) {
		HexCell cell = null;
		list.TryGetValue (index, out cell);
		return cell;
	}



	//##########################################################################
	#region hide
	HexIndex cube_to_hex(Vector3 h) { // axial
		var q = (int)h.x;
		var r = (int)h.z;
		return new HexIndex(q, r);
	}
			
	Vector3 hex_to_cube(Vector2 h) { // # axial
		var x = h.x;
		var z = h.y;
		var y = -x-z;
		return new Vector3(x, y, z);
	}

	HexIndex hex_round (Vector2 h) {
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
	#endregion

	public void DrawGizmos () {
		if (list == null)
			return;
		foreach (var r in list.Values) {
			r.DrawGizmos();
		}
		Gizmos.color = Color.red;
		Gizmos.DrawSphere (transform.position, 0.05f);
	}

}
