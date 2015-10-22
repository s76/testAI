using UnityEngine;
using System.Collections;
using System.Linq;
using React;
using  Action = System.Collections.Generic.IEnumerator<React.NodeResult>;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPatroller : MonoBehaviour {

    /// <summary>
    /// The distance between random positions.
    /// </summary>
    public float waypointDistance = 20;

    /// <summary>
    /// How close the player must be in order to be noticed.
    /// </summary>
    public float visibilityDistance = 5;

    /// <summary>
    /// How close the player must be in order to attack the player.
    /// </summary>
    public float attackDistance = 1;

    /// <summary>
    /// The transform of the player GameObject.
    /// </summary>
    public Transform player;


    NavMeshAgent agent;
    Vector3 destination;
    Animator animator;

    void Start ()
    {
        agent = GetComponent<NavMeshAgent> ();      
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Is the player within the visibilityDistance of this gameObject?
    /// </summary>
    /// <returns><c>true</c> if player near; otherwise, <c>false</c>.</returns>
    public bool IsPlayerInVisibleRange() {
        var hits = Physics.OverlapSphere(transform.position, visibilityDistance);
        foreach(var h in hits) {
            if(h.transform == player) return true;
        }
        return false;
    }

    /// <summary>
    /// Is the player within the attackDistance of this gameObject?
    /// </summary>
    /// <returns><c>true</c> if player near; otherwise, <c>false</c>.</returns>
    public bool IsPlayerInAttackRange() {
        var hits = Physics.OverlapSphere(transform.position, attackDistance);
        foreach(var h in hits) {
            if(h.transform == player) return true;
        }
        return false;
    }

    /// <summary>
    /// Chooses a new random position, sets the destination field.
    /// </summary>
    public void ChooseNewRandomPosition() {
        destination = transform.position + new Vector3 (Random.Range (-waypointDistance, waypointDistance), 0, Random.Range (-waypointDistance, waypointDistance));
    }

    /// <summary>
    /// Chooses a new position which moves towards the player.
    /// </summary>
    public void ChoosePositionTowardsPlayer() {
        destination = Vector3.Lerp(transform.position, player.transform.position, 0.5f);
    }

    /// <summary>
    /// Attacks the player.
    /// </summary>
    public Action AttackPlayer() {
        yield return NodeResult.Success;
    }

    /// <summary>
    /// This actions moves the Agent to the position specified in the destination field.
    /// </summary>
    public Action MoveToPosition ()
    {
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
	
}
