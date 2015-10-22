using UnityEngine;
using System.Collections;
using React;
using  Action = System.Collections.Generic.IEnumerator<React.NodeResult>;

public class TestReactAI : MonoBehaviour {

	public bool if01 () {
		return true;
	}

	public bool if02 () {
		return false;
	}

	public Action ac01 () {
		Debug.Log (nb + "\tframe before failure");
		yield return NodeResult.Failure;
		Debug.Log (nb + "\tframe after failure");
	}

	public Action ac02 () {
		Debug.Log (nb + "\tframe before Success");
		yield return NodeResult.Success;
		Debug.Log (nb + "\tframe after Success");
	}

	public Action ac03 () {
		Debug.Log (nb + "\tframe before Continue");
		yield return NodeResult.Continue;
		Debug.Log (nb + "\tframe after Continue");
	}

	public Action loop01 () {
		for (;;) {
			Debug.Log (nb + "\tframe before failure");
			yield return NodeResult.Failure;
			Debug.Log (nb + "\tframe after failure");
		}
	}
	
	public Action loop02 () {
		for (;;) {
			Debug.Log (nb + "\tframe before Success");
			yield return NodeResult.Success;
			Debug.Log (nb + "\tframe after Success");
		}
	}
	
	public Action loop03 () {
		for (;;) {
			Debug.Log (nb + "\tframe before Continue");
			yield return NodeResult.Continue;
			Debug.Log (nb + "\tframe after Continue");
		}
	}
	

	public bool nOnS () {
		Debug.Log ("Non Sense");
		return false;
	}
	public Action end () {
	
		Debug.Log (nb +" End");
		yield return NodeResult.Success;
	}

	int nb = 0;

	void Update () {
		nb ++;
	}

}
