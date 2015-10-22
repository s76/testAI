using UnityEngine;
using System.Collections;

public class UnitCore : IPoolable {

	public UnitSide side;
	public UnitParameters2 parameters;

	public NavMeshAgent agent { get; private set; }
	public int id=0;

	Reactor reactor;

	public Node currentNode;
	public Node last_node; // for test

	void Awake () {
		reactor = GetComponent<Reactor> ();
		agent = GetComponent<NavMeshAgent> ();
	}

	public override void Activate ()
	{
		base.Activate ();
		parameters.hp_current = parameters.hp_max; 
		reactor.enabled = true;

	}

	public override void Deactivate ()
	{
		reactor.enabled = false;
		agent.ResetPath ();
		currentNode = null;
		base.Deactivate ();
	}

	public override bool IsFree ()
	{
		return base.IsFree () & currentNode == null;
	}
}

public enum UnitSide : int { 
	Side01 = 0, 
	Side02 = 1 
}
