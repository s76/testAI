using UnityEngine;
using System.Collections.Generic;

// customized pool
public class UnitPool : MonoBehaviour {

	public AgentX2 prefab;
	public int initCap;

	List<AgentX2> list;
	
	void Awake () {
		if ( initCap < 0 ) throw new UnityException("initCap < 0 ");

		list = new List<AgentX2>(initCap);
		for ( int i =0; i < initCap; i ++ ) {
			AgentX2 g = Instantiate<AgentX2>(prefab);
			g.transform.SetParent(transform);
			g.Deactivate();
			list.Add(g);
		}
	}
	
	public AgentX2 Get () {
		foreach ( var o in list ) {
			if ( o.IsFree() ) {
				o.Activate();
				return o;
			}
		}
		
		AgentX2 g = Instantiate<AgentX2>(prefab);
		g.transform.SetParent(transform);
		g.Activate();
		list.Add(g);
		
		return g;
	}
}
