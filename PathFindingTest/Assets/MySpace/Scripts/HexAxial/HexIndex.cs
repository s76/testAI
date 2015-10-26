using UnityEngine;
using System.Collections.Generic;

public class HexIndex  {
	public int q,r;
	public HexIndex ( int _q, int _r ) {
		q = _q;
		r = _r;
	}

	public override string ToString ()
	{
		return "HexIndex= " + q + ", " + r;
	}
}

public class HexIndexComparer : IEqualityComparer<HexIndex> {
	public bool Equals (HexIndex _this, HexIndex another)
	{ 
		return (another.q == _this.q ) & (another.r == _this.r);
	}
	
	public int GetHashCode(HexIndex x)
	{
		return x.r.GetHashCode () + x.q.GetHashCode ();
	}
}