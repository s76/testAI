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
	[ReactNode]
	public class SendMessage : LeafNode
	{
		[ReactVar]
		public string methodName;
		
		[ReactVar]
		public string pathToGameObject;
		
		[ReactVar]
		public SendMessageOptions messageOptions;
		
		GameObject target;
		
		public override IEnumerator<NodeResult> NodeTask ()
		{
			if (target == null) {
				target = GameObject.Find(pathToGameObject);
				if(target == null) {
					Debug.LogError("Could not find GameObject: " + pathToGameObject);	
				}
			}
			if(target != null) {
				target.SendMessage(methodName, messageOptions);	
			} else {
				yield return NodeResult.Failure;	
			}
			yield return NodeResult.Success;
		}
		
		public override string ToString ()
		{
			return string.Format ("SendMessage - {0}", methodName);
		}
	
	
#if UNITY_EDITOR
		public override void DrawSpecialValueWidget (System.Reflection.FieldInfo f)
		{
			if(f.Name == "messageOptions") {
				f.SetValue(this, (SendMessageOptions)EditorGUILayout.EnumPopup((SendMessageOptions)f.GetValue(this)));	
			}
		}
		
        public new static string GetHelpText ()
		{
			return "Sends a message to a gameobject.";
		}
		
		
#endif
	}

}







