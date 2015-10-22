using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using React;
using System.Linq;

//This is a little shortcut which creates an alias for a type, and makes out Action methods look much nicer.
using  Action = System.Collections.Generic.IEnumerator<React.NodeResult>;
	
/// <summary>
/// This class demonstrates how to write routines which work with React.
/// It is a simple agent type script which can move a gameobject around a scene using a navmesh.
/// </summary>
public class SimpleEnemy : MonoBehaviour
{
	public float turnSpeed = 45;
	public Transform player;
	NavMeshAgent agent;
	
	void Start ()
	{
		agent = GetComponent<NavMeshAgent> ();		
	}
	
    /// <summary>
    /// This is an example of a Condition method which is used in React. Notice is is a public method that takes
    /// no arguments, and returns a bool. If it returns true, the condition is considered to true. If it returns
    /// false, the condition is considered false.
    /// 
    /// This particular method performs a raycast from the gameObject to the player object to determine if the
    /// player can see this gameObject.
    /// </summary>
	public bool CanISeePlayer ()
	{
		var playerDirection = player.position - transform.position;
		var ray = new Ray (transform.position, playerDirection);
        var inFOV = true; // Vector3.Angle(transform.forward, playerDirection) < 45; //90 degree field of fiew
		if (inFOV) {	
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 1000)) {
				return hit.collider.transform == player;
			} 		
		}
		return false;
	}
	
    /// <summary>
    /// This is an example of a React Action. Notice it is a public method, which returns an Action. (If you see the above
    /// alias, you will see that "Action" is an alias for System.Collections.Generic.IEnumerator<React.NodeResult>;
    /// The method works very much like a Unity cortouine. If you yield NodeResult.Continue, the action will continue in 
    /// the next frame. If you yield NodeResult.Success or NodeResult.Failure, the Action will stop and return this result
    /// to it's parent node in the behaviour tree.
    /// 
    /// This particular method just makes the enemy AI turn left slowly to 45 degrees.
    /// </summary>
	public Action TurnLeft() {
		var forward = transform.forward;
		while(true) {
			yield return NodeResult.Continue;
			transform.Rotate(0, Time.deltaTime*turnSpeed, 0);
			if(Vector3.Angle(forward, transform.forward) >= 45) {
				yield return NodeResult.Success;	
			}
		}
	}

	/// <summary>
    /// This is another Actin method.  This particular methods tries to calculate a path to a random position.
    /// </summary>
    /// <returns>The to random position.</returns>
	public Action MoveToRandomPosition ()
	{
		var destination = transform.position + new Vector3 (UnityEngine.Random.Range (-20, 20), 0, UnityEngine.Random.Range (-20, 20));
		agent.destination = destination;
		while (true) {
			if (agent.pathPending) {
				yield return NodeResult.Continue;	
			} else {
				if (agent.pathStatus == NavMeshPathStatus.PathInvalid) {
					yield return NodeResult.Failure;	
				}
				if (agent.pathStatus == NavMeshPathStatus.PathPartial) {
					yield return NodeResult.Continue;	
				}
				if (agent.pathStatus == NavMeshPathStatus.PathComplete) {
					while (true) {
                        agent.Resume();
						if (agent.remainingDistance < 2) {
							yield return NodeResult.Success;
						} else {
							yield return NodeResult.Continue;	
						}
					}
				}
			}
		}
	}

    public Action Wait() {
        agent.Stop();
        yield return NodeResult.Success;
    }

    public Action PushClosestEnemy() {
        Func<SimpleEnemy, SimpleEnemy, SimpleEnemy> closer =
            (e1, e2) => (e1.transform.position - transform.position).sqrMagnitude < (e2.transform.position - transform.position).sqrMagnitude ? e1 : e2;
        SimpleEnemy closestEnemy = FindObjectsOfType<SimpleEnemy>().Where(e => e.CanISeePlayer())
            .Aggregate((SimpleEnemy)null, (closestE, e) => (closestE == null || closer(closestE, e) == e) ? e : closestE);
        if (closestEnemy == null) yield return NodeResult.Failure;
        while (closestEnemy.CanISeePlayer() == false) {
            agent.destination = closestEnemy.transform.position;
            while (agent.pathPending) yield return NodeResult.Continue;
            if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
                yield return NodeResult.Failure;
            agent.Resume();
        }
        yield return NodeResult.Success;
    }
	
}
