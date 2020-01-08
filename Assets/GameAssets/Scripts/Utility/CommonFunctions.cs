using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CommonFunctions
{
    public static bool isAllies(ICyberAgent agent1, ICyberAgent agent2)
    {
        return agent1.getFaction().Equals(agent2.getFaction());
    }

    public static bool isTargetVisibleToAgent(ICyberAgent agent, ICyberAgent target)
    {
        return true;
    }

    public static bool checkDestniationReached(NavMeshAgent navMeshAgent)
    {
        if(!navMeshAgent.pathPending && 
          navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && 
          (navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f))
        {
            return true;
        }
        return false;
    }
}
