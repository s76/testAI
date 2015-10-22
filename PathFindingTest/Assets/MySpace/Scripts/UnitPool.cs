using UnityEngine;
using System.Collections.Generic;

// customized pool
public class UnitPool : MonoBehaviour {

	public UnitState prefab;
	public int initCap;

	List<UnitState> list;
	
	public void Awake () {
		if ( initCap < 0 ) throw new UnityException("initCap < 0 ");

		list = new List<UnitState>(initCap);
		for ( int i =0; i < initCap; i ++ ) {
			UnitState g = Instantiate<UnitState>(prefab);
			g.transform.SetParent(transform);
			g.Deactivate();
			list.Add(g);
		}
	}
	
	public UnitState Get () {
		foreach ( var o in list ) {
			if ( o.IsFree() ) {
				o.Activate();
				return o;
			}
		}
		
		UnitState g = Instantiate<UnitState>(prefab);
		g.transform.SetParent(transform);
		g.Activate();
		list.Add(g);
		
		return g;
	}
}
