using UnityEngine;
using System.Collections;

public class ControlPointController : MonoBehaviour {

	[System.Serializable] 
	public class ControlLayer {
		public Transform[] controlPoints;
	}

	public ControlLayer[] layers;
}
