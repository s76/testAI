using UnityEngine;
using System.Collections.Generic;

// customized pool
public class UnitPool : MonoBehaviour {

	public UnitCore prefab;
	public int initCap;

	List<UnitCore> list;
	
	void Awake () {
		if ( initCap < 0 ) throw new UnityException("initCap < 0 ");

		list = new List<UnitCore>(initCap);
		for ( int i =0; i < initCap; i ++ ) {
			UnitCore g = Instantiate<UnitCore>(prefab);
			g.transform.SetParent(transform);
			g.Deactivate();
			list.Add(g);
		}
	}
	
	public UnitCore Get () {
		foreach ( var o in list ) {
			if ( o.IsFree() ) {
				o.Activate();
				return o;
			}
		}
		
		UnitCore g = Instantiate<UnitCore>(prefab);
		g.transform.SetParent(transform);
		g.Activate();
		list.Add(g);
		
		return g;
	}
}
