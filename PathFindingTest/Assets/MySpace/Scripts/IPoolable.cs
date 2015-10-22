using UnityEngine;
using System.Collections;

public abstract class IPoolable : MonoBehaviour {
	virtual public void Deactivate ()
	{
		gameObject.SetActive (false);
	}

	virtual public void Activate () {
		gameObject.SetActive (true);
	}

	virtual public bool IsFree () {
		return !isActiveAndEnabled;
	}
}
