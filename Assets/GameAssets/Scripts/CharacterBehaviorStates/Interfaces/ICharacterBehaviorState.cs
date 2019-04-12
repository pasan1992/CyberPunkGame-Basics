using UnityEngine.AI;
using UnityEngine;

public interface ICharacterBehaviorState 
{
    void updateStage();

    void setNavMeshAgent(NavMeshAgent navMeshAgent);

    void setTargets(ICyberAgent target);

    void setStepIntervalSize(float timeInSeconds);
}
