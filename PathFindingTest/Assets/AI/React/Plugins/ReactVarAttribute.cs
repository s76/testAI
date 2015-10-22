using System;
using System.Collections.Generic;
using System.Linq;

namespace React
{
	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class ReactVarAttribute : System.Attribute
	{
		public string displayName = null;
		public int priority = 0;
		
		public ReactVarAttribute ()
		{
		}

		public ReactVarAttribute (string displayName)
		{
			this.displayName = displayName;
		}
		
		public ReactVarAttribute (int priority)
		{
			this.priority = priority;	
		}
		
	}
}