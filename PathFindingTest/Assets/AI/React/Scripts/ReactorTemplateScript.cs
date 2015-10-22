using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using React;

public class ReactorTemplateScript : MonoBehaviour {

	public void ThisIsAFunction() {
	}
	
	public bool ThisIsACondition() {
		return false;
	}
	
	public IEnumerator<React.NodeResult> ThisIsAnAction() {
		//This allows the action to continue in the next tick.
		Debug.Log("Running an Action");
		yield return NodeResult.Continue;
		Debug.Log("Still running an Action");
		//This tells the reactor that this action failed.
		yield return NodeResult.Failure;
		
		//This tells the reactor that this action succeeded;
		yield return NodeResult.Success;
	}
		
	
}
