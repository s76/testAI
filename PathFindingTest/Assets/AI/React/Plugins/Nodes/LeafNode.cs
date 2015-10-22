using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace React
{

	
	public class LeafNode : BaseNode
	{
		public override IEnumerable<BaseNode> Children ()
		{
			return new BaseNode[]{};
		}
#if UNITY_EDITOR
		public override Color NodeColor ()
		{
			return Color.grey;
		}
#endif
	}
	
	


}







