using UnityEngine;
using System.Collections.Generic;

public class Pool : MonoBehaviour {

	[SerializeField] IPoolable prefab;
	public int initCap;
	
	List<IPoolable> list;
	
	void Awake () {
		if ( initCap < 0 ) throw new UnityException("initCap < 0 ");
		list = new List<IPoolable>(initCap);
		for ( int i =0; i < initCap; i ++ ) {
			GameObject g = Instantiate(prefab.gameObject);
			g.transform.SetParent(transform);
			var _p = g.GetComponent<IPoolable>();
			_p.Deactivate();
			list.Add(_p);
		}
	}
	
	public GameObject Get () {
		foreach ( var o in list ) {
			if ( o.IsFree() ) {
				o.Activate();
				return o.gameObject;
			}
		}
		
		GameObject g = Instantiate(prefab.gameObject);
		g.transform.SetParent(transform);
		var _p = g.GetComponent<IPoolable>();
		_p.Activate();
		list.Add(_p);
		
		return _p.gameObject;
	}
}
