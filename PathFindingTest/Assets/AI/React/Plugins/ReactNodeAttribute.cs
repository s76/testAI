using System;
using System.Collections.Generic;
using System.Linq;

namespace React
{
	[System.AttributeUsage(System.AttributeTargets.Class)]
	public class ReactNodeAttribute : System.Attribute
	{
		
		
		public ReactNodeAttribute ()
		{
		}
		
		static public Type[] GetTypes ()
		{
			var typesWithMyAttribute =
		    from a in AppDomain.CurrentDomain.GetAssemblies ()
		    from t in a.GetTypes ()
		    let attributes = t.GetCustomAttributes (typeof(ReactNodeAttribute), true)
		    where attributes != null && attributes.Length > 0
		    select t;	
			return typesWithMyAttribute.ToArray();
		}
	}
	
	
	public class ReactDocAttribute : System.Attribute {
		public string docText;
		public ReactDocAttribute(string docText) {
			this.docText = docText;
		}

	}
}