using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using React;
using Action = System.Collections.Generic.IEnumerator<React.NodeResult>;


public class TurnBasedUnit : MonoBehaviour
{

    public bool ItIsMyTurn ()
    {
        return false;
    }

    public bool HealthIsCritical ()
    {
        return false;
    }

    public bool HealthIsAlmostFull ()
    {
        return false;
    }

    public bool UnderAttack ()
    {
        return false;
    }

    public bool SupportIsNearby ()
    {
        return false;
    }

    public bool SurroundedByEnemy ()
    {
        return false;
    }

    public bool CanSeeEnemy ()
    {
        return false;
    }

    public bool EnemyInRange ()
    {
        return false;
    }

    public bool AllyUnderAttack ()
    {
        return false;
    }


    public Action TargetNearestEnemy ()
    {
        yield return NodeResult.Failure;
    }

    public Action FindNearestAlly ()
    {
        yield return NodeResult.Failure;
    }

    public Action Attack ()
    {
        yield return NodeResult.Failure;
    }

    public Action Defend ()
    {
        yield return NodeResult.Failure;
    }

    public Action FindSafeMoveLocation ()
    {
        yield return NodeResult.Failure;
    }

    public Action FindAggressiveMoveLocation ()
    {
        yield return NodeResult.Failure;
    }

    public Action FindSupportiveMoveLocation ()
    {
        yield return NodeResult.Failure;
    }

    public Action Move ()
    {
        yield return NodeResult.Failure;
    }

    public Action Panic ()
    {
        yield return NodeResult.Failure;
    }

    public Action Die ()
    {
        yield return NodeResult.Failure;
    }

    public Action Retreat ()
    {
        yield return NodeResult.Failure;
    }
	

}
