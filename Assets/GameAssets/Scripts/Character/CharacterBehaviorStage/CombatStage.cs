using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CombatStage : BasicMovmentStage
{
    private ICyberAgent opponent;
    private Collider[] targetLocations;
    private Transform targetLocation;
    private CoverPoint[] coverPoints;
    private CoverPoint currentCoverPoint;
    private Vector3 randomOffset = Vector3.zero;

    public enum CombatSubStages { LookingForCover,MovingToCover, InCover }
    private CombatSubStages currentCombatSubStage = CombatSubStages.LookingForCover;

    public enum CoverShootingSubStages { Cover,Peek,Shoot};
    private CoverShootingSubStages currentCoverShootingSubStage;

    // Parameters
    float fireRangeDistance = 14;
    int shotsFromCover = 3;
    int currentShotsFromCover = 0;

    #region initalize
    public CombatStage(ICyberAgent selfAgent,ICyberAgent target,NavMeshAgent navMeshAgent) :base(selfAgent,navMeshAgent)
    {
        this.opponent = target;
        coverPoints = GameObject.FindObjectsOfType<CoverPoint>();
        selfAgent.toggleHide();
        selfAgent.aimWeapon();
        selfAgent.togglepSecondaryWeapon();
        targetLocations = opponent.getTransfrom().gameObject.GetComponentsInChildren<Collider>();
        findTargetLocationToFire();
    }
    #endregion

    #region update

    // Main Update function that get called for every frame.
    public override void updateStage()
    {
        // This is needed for calling step update.
        base.updateStage();

        if(currentCombatSubStage.Equals(CombatSubStages.InCover))
        {
            m_selfAgent.setTargetPoint(opponent.getTransfrom().position + new Vector3(0, 1.1f, 0)+ randomOffset);
        }
        else
        {
            m_selfAgent.setTargetPoint(targetLocation.position + randomOffset);
        }

    }

    // Update function that get called for every 1 second.
    protected override void stepUpdate()
    {
        updateSubStages();
        updateTarget();
    }

    private void updateSubStages()
    {
        findTargetLocationToFire();

        switch (currentCombatSubStage)
        {
            case CombatSubStages.InCover:

                //Debug.Log("In Cover");

                m_selfAgent.lookAtTarget();

                if (currentCoverPoint.isSafeFromTarget() && currentCoverPoint.canFireToTarget(fireRangeDistance))
                {
                    switch(currentCoverShootingSubStage)
                    {
                        case CoverShootingSubStages.Cover:

                            m_selfAgent.aimWeapon();
                            currentCoverShootingSubStage = CoverShootingSubStages.Peek;

                            shotsFromCover = (int)(Random.value * 5);
                            //Debug.Log(shotsFromCover);

                            break;
                        case CoverShootingSubStages.Peek:

                            m_selfAgent.weaponFireForAI();
                            currentCoverShootingSubStage = CoverShootingSubStages.Shoot;
                            setStepIntervalSize(0.3f);

                            break;
                        case CoverShootingSubStages.Shoot:


                            currentShotsFromCover++;
                            
                            if(currentShotsFromCover > shotsFromCover)
                            {
                                currentCoverShootingSubStage = CoverShootingSubStages.Cover;
                                currentShotsFromCover = 0;
                                m_selfAgent.stopAiming();
                                setStepIntervalSize(0.8f);
                            }
                            else
                            {
                                currentCoverShootingSubStage = CoverShootingSubStages.Peek;
                            }
                            


                            break;
                    }
                }
                else
                {
                    //Debug.Log("Cover is not safe or cannot fire to target");
                    currentCombatSubStage = CombatSubStages.LookingForCover;
                    setStepIntervalSize(1);
                }
                break;
            case CombatSubStages.LookingForCover:
                //Debug.Log("Looking for cover");


                CoverPoint tempCurrentCoverPoint = closestCombatLocationAvaialbe();

                if(tempCurrentCoverPoint != null)
                {

                    if (currentCoverPoint)
                    {
                        //currentCoverPoint.stPointOccupentsName("");
                        currentCoverPoint.setOccupent(null);
                    }

                    currentCoverPoint = tempCurrentCoverPoint;
                    m_navMeshAgent.SetDestination(currentCoverPoint.getPosition());
                    currentCombatSubStage = CombatSubStages.MovingToCover;

                    // Get up and move
                    m_selfAgent.toggleHide();
                    m_selfAgent.aimWeapon();
                }

                break;

            case CombatSubStages.MovingToCover:
                //Debug.Log("Moving to cover");
                if (!m_navMeshAgent.pathPending && m_navMeshAgent.remainingDistance >1)
                {

                    if(m_navMeshAgent.remainingDistance > 3 && Vector3.Distance(m_selfAgent.getCurrentPosition(),opponent.getCurrentPosition()) > fireRangeDistance)
                    {
                        m_enableRun = true;
                        m_selfAgent.stopAiming();
                    }
                    else
                    {
                        if (Vector3.Distance(m_selfAgent.getCurrentPosition(), opponent.getCurrentPosition()) < fireRangeDistance)
                        {
                            m_selfAgent.aimWeapon();
                            m_enableRun = false;
                            m_selfAgent.weaponFireForAI();
                        }
                    }



                }
                else
                {
                    m_selfAgent.lookAtTarget();
                    currentCombatSubStage = CombatSubStages.InCover;

                    // Get down on cover
                    m_selfAgent.toggleHide();
                    m_selfAgent.stopAiming();
                    setStepIntervalSize(0.8f);
                    m_navMeshAgent.velocity = Vector3.zero;

                }
                break;
        }
    }

    public void updateTarget()
    {
        float minimumDistance = Vector3.Distance(m_selfAgent.getCurrentPosition(), opponent.getCurrentPosition());

        if (opponent == null || !opponent.IsFunctional())
        {
            minimumDistance = 9999;
            AgentController[] allAgents = GameObject.FindObjectsOfType<AgentController>();
            foreach (AgentController agent in allAgents)
            {
                ICyberAgent cyberAgent = agent.getICyberAgent();
                float distance = Vector3.Distance(m_selfAgent.getCurrentPosition(), cyberAgent.getCurrentPosition());
                if (cyberAgent.IsFunctional() && cyberAgent.getFaction() != m_selfAgent.getFaction() && !cyberAgent.getFaction().Equals(AgentController.AgentFaction.Player))
                {
                    if (distance < minimumDistance)
                    {
                        minimumDistance = distance;
                        opponent = cyberAgent;
                    }
                }
            }
        }
    }
    #endregion

    #region Utility

    private CoverPoint closestCombatLocationAvaialbe()
    {
        float minimumDistanceToIdealCoverPoint = 999;
        float minimumDistanceToSafeCoverPoint = 999;

        CoverPoint tempIDealCoverPoint = null;
        CoverPoint tempSafeCoverPoint = null;


        foreach (CoverPoint point in coverPoints)
        {
            if (!point.isOccupied())
            {
                point.setTargetToCover(opponent);
                if(point.isSafeFromTarget())
                {

                    // Find the safe cover point.
                   if(minimumDistanceToSafeCoverPoint > point.distanceTo(m_selfAgent.getCurrentPosition()))
                    {
                        minimumDistanceToSafeCoverPoint = point.distanceTo(m_selfAgent.getCurrentPosition());
                        tempSafeCoverPoint = point;
                    }

                   // Find the ideal closest cover point.
                   if(point.canFireToTarget(fireRangeDistance))
                    {
                        if (minimumDistanceToIdealCoverPoint > point.distanceTo(m_selfAgent.getCurrentPosition()))
                        {
                            minimumDistanceToIdealCoverPoint = point.distanceTo(m_selfAgent.getCurrentPosition());
                            tempIDealCoverPoint = point;
                        }
                    }

                }
            }
        }

        if(tempIDealCoverPoint !=null)
        {
            //tempIDealCoverPoint.stPointOccupentsName(selfAgent.getName());
            tempIDealCoverPoint.setOccupent(m_selfAgent);
            return tempIDealCoverPoint;
        }
        else if(tempSafeCoverPoint != null)
        {
            //tempSafeCoverPoint.stPointOccupentsName(selfAgent.getName());
            tempSafeCoverPoint.setOccupent(m_selfAgent);
            return tempSafeCoverPoint;
        }

        return null;
    }
    
    private void findTargetLocationToFire()
    {

        MovingAgent humanoidOpponent = opponent as MovingAgent;

        if(humanoidOpponent != null && humanoidOpponent.isCrouched())
        {
            targetLocation = humanoidOpponent.getHeadTransfrom();
        }
        else
        {
            int randomIndex = Random.Range(0, targetLocations.Length - 1);
            targetLocation = targetLocations[randomIndex].transform;
        }



        if(Random.value > m_selfAgent.getSkill())
        {
            randomOffset = Random.insideUnitSphere * 2;
        }
        else if(m_navMeshAgent.remainingDistance > 9)
        {
            randomOffset = Random.insideUnitSphere * 0.7f;
        }
        else
        {
            randomOffset = Vector3.zero;
        }

    }
    #endregion

    #region Getters and Setters

    public override void setWeaponFireCapability(bool enabled)
    {
        m_selfAgent.setWeponFireCapability(enabled);
    }

    public override void setTargets(ICyberAgent target)
    {
        this.opponent = target;
    }

    private bool isCoverPointUsable(CoverPoint point)
    {
        return point.isSafeFromTarget() && point.canFireToTarget(fireRangeDistance);
    }

    #endregion


}
