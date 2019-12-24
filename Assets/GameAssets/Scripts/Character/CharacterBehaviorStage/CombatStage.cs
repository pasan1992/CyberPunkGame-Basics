using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CombatStage : BasicCombatStage
{
    //private ICyberAgent opponent;
    private Collider[] targetLocations;
    private Transform targetLocation;
    private CoverPoint[] coverPoints;
    private CoverPoint currentCoverPoint;
    private Vector3 randomOffset = Vector3.zero;
    private static int COVER_POINT_MIN_DISTANCE = 15;

    public enum CombatSubStages { LookingForCover,MovingToCover, InCover }
    private CombatSubStages currentCombatSubStage = CombatSubStages.LookingForCover;

    public enum CoverShootingSubStages { Cover,Peek,Shoot};
    private CoverShootingSubStages currentCoverShootingSubStage;

    // Parameters
    float fireRangeDistance = 15;
    int shotsFromCover = 3;
    int currentShotsFromCover = 0;

    int coverIteractions = 0;
    int maxCoverIteractions = 20;

    private CombatSubStages loggerStage = CombatSubStages.InCover;
    private CoverShootingSubStages logShootingStage;

    private bool playitSafe;

    #region initalize
    public CombatStage(ICyberAgent selfAgent,ICyberAgent target,NavMeshAgent navMeshAgent) :base(selfAgent,target,navMeshAgent)
    {
        this.opponent = target;
        coverPoints = GameObject.FindObjectsOfType<CoverPoint>();
        playitSafe = false;

        if(target !=null)
        {
            targetLocations = opponent.getTransfrom().gameObject.GetComponentsInChildren<Collider>();
            findTargetLocationToFire();
        }
    }

    public override void initalizeStage()
    {
        HumanoidMovingAgent humnaoidAgent = ((HumanoidMovingAgent)m_selfAgent);

        if(humnaoidAgent.isInteracting())
        {
            ((HumanoidMovingAgent)m_selfAgent).cancleInteraction();
        }

        
      // m_selfAgent.toggleHide();
       m_selfAgent.aimWeapon();
        
        if(Random.value > 0.5)
        {
            humnaoidAgent.togglePrimaryWeapon();
        }
        else
        {
            humnaoidAgent.togglepSecondaryWeapon();
        }      
        
        m_navMeshAgent.enabled = true;   
        m_navMeshAgent.isStopped = false; 

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
            if(opponent.isHidden())
            {
                if(opponent.isAimed())
                {
                    m_selfAgent.setTargetPoint(opponent.getTransfrom().position + new Vector3(0, 1.1f, 0) + randomOffset + opponent.getCurrentVelocity()*0);
                }
                else
                {
                    m_selfAgent.setTargetPoint(opponent.getTransfrom().position + new Vector3(0, 0.4f, 0) + randomOffset + opponent.getCurrentVelocity()*0);
                }
            }
            else
            {
                m_selfAgent.setTargetPoint(opponent.getTransfrom().position + new Vector3(0, 1.25f, 0) + randomOffset + opponent.getCurrentVelocity()*0);
            }


            //m_selfAgent.setTargetPoint(opponent.getTransfrom().position  + randomOffset);
        }
        else
        {
            m_selfAgent.setTargetPoint(targetLocation.position + randomOffset + opponent.getCurrentVelocity()*0);
        }

    }

    // Update function that get called for every 1 second.
    protected override void stepUpdate()
    {
        updateSubStages();
        //updateTarget();
    }

    private void updateSubStages()
    {
        findTargetLocationToFire();
        //logStage();
        //logSubStage();

        switch (currentCombatSubStage)
        {
            case CombatSubStages.InCover:
                m_navMeshAgent.isStopped = true;
                
                //Debug.Log("In Cover");

                //m_selfAgent.lookAtTarget();

                if ( (coverIteractions < maxCoverIteractions) && (currentCoverPoint.isSafeFromTarget() || (currentCoverPoint.canFireToTarget(fireRangeDistance,true)) ) )
                {
                    // if(coverIteractions < maxCoverIteractions && currentCoverPoint.isSafeFromTarget())
                    // {
                    //        // Debug.Log("Is safe and max iteractions");
                    // }

                    // if(currentCoverPoint.canFireToTarget(fireRangeDistance))
                    // {
                    //    // Debug.Log("Can fire from target");
                    // }
                    
                    switch(currentCoverShootingSubStage)
                    {
                        case CoverShootingSubStages.Cover:

                            //m_selfAgent.aimWeapon();
                            m_selfAgent.stopAiming();
                            currentCoverShootingSubStage = CoverShootingSubStages.Peek;

                            shotsFromCover = (int)(Random.value * 5 + 1);
                            //Debug.Log(shotsFromCover);

                            break;
                        case CoverShootingSubStages.Peek:
                            m_selfAgent.aimWeapon();
                            m_selfAgent.weaponFireForAI();
                            currentCoverShootingSubStage = CoverShootingSubStages.Shoot;
                            setStepIntervalSize(0.1f);

                            break;
                        case CoverShootingSubStages.Shoot:

                            coverIteractions ++;
                            currentShotsFromCover++;
                            
                            if(currentShotsFromCover > shotsFromCover)
                            {
                                if(currentCoverPoint.isSafeFromTarget() && isTargetFar())
                                {
                                    currentCoverShootingSubStage = CoverShootingSubStages.Cover;
                                    currentShotsFromCover = 0;
                                    m_selfAgent.stopAiming();
                                    setStepIntervalSize(Random.value*1.2f);
                                }
                                else
                                {
                                    currentShotsFromCover = 0;
                                    currentCoverShootingSubStage = CoverShootingSubStages.Peek;
                                }

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
                    //Debug.Log("looking for cover");
                    //Debug.Log("Cover is not safe or cannot fire to target");
                    currentCombatSubStage = CombatSubStages.LookingForCover;
                    m_navMeshAgent.isStopped = false;
                    setStepIntervalSize(1);
                    coverIteractions = 0;
                }
                break;
            case CombatSubStages.LookingForCover:
                //logStage(CombatSubStages.LookingForCover);
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
                    currentCoverPoint.setOccupent(m_selfAgent);

                    // Get up and move
                    if(m_selfAgent.isHidden())
                    {
                         m_selfAgent.toggleHide();
                    }
                    m_selfAgent.aimWeapon();
                }

                break;

            case CombatSubStages.MovingToCover:
                //logStage(CombatSubStages.MovingToCover);
                //Debug.Log("Moving to cover");
                //if (!m_navMeshAgent.pathPending && m_navMeshAgent.remainingDistance >1)
                if(!checkDestniationReached())
                {

                    if(m_navMeshAgent.remainingDistance > 3 && Vector3.Distance(m_selfAgent.getCurrentPosition(),opponent.getCurrentPosition()) > fireRangeDistance*0.55f)
                    {
                        m_enableRun = true;
                        m_selfAgent.stopAiming();
                        setStepIntervalSize(1f);
                    }
                    else
                    {
                        if (Vector3.Distance(m_selfAgent.getCurrentPosition(), opponent.getCurrentPosition()) < fireRangeDistance)
                        {
                            m_selfAgent.aimWeapon();
                            m_enableRun = false;
                            m_selfAgent.weaponFireForAI();
                            setStepIntervalSize(0.3f);
                        }
                    }



                }
                else
                {
                    m_selfAgent.lookAtTarget();
                    currentCombatSubStage = CombatSubStages.InCover;

                    // Get down on cover
                    if(currentCoverPoint.isSafeFromTarget() && !m_selfAgent.isHidden())
                    {
                        m_selfAgent.toggleHide();
                    }
                    

                    //m_selfAgent.stopAiming();
                    maxCoverIteractions = Random.Range(10,30);
                    setStepIntervalSize(0.1f);
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
                if (cyberAgent.IsFunctional() && cyberAgent.getFaction() != m_selfAgent.getFaction() && !cyberAgent.getFaction().Equals(AgentBasicData.AgentFaction.Player))
                {
                    if (distance < minimumDistance)
                    {
                        minimumDistance = distance;
                        opponent = cyberAgent;
                         targetLocations = opponent.getTransfrom().gameObject.GetComponentsInChildren<Collider>();
                    }
                }
            }
        }
    }
    #endregion

    #region Utility

    private void takeCover()
    {
        m_selfAgent.stopAiming();
    }

    private CoverPoint closestCombatLocationAvaialbe()
    {
        float minimumDistanceToIdealCoverPoint = 999;
        float minimumDistanceToSafeCoverPoint = 999;
        float maximumDistanceToRiskyCoverPoint = 0;
        float distanceToClosestCoverPoint = 99999;

        CoverPoint tempIDealCoverPoint = null;
        CoverPoint tempSafeCoverPoint = null;
        CoverPoint tempRiskyCoverPoint = null;


        foreach (CoverPoint point in coverPoints)
        {
            if (!point.isOccupied())
            {
                point.setTargetToCover(opponent);

                if(point.isSafeFromTarget())
                {

                    // Find the safe cover point.
                   if(minimumDistanceToSafeCoverPoint > point.distanceTo(opponent.getCurrentPosition()))
                    {
                        minimumDistanceToSafeCoverPoint = point.distanceTo(opponent.getCurrentPosition());
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
                
                if(playitSafe)
                {
                    // Find the safe cover point.
                    float distanceFromRiskyPoint = point.distanceTo(opponent.getCurrentPosition());
                    if (maximumDistanceToRiskyCoverPoint < distanceFromRiskyPoint && distanceFromRiskyPoint < COVER_POINT_MIN_DISTANCE)
                    {
                        maximumDistanceToRiskyCoverPoint = point.distanceTo(opponent.getCurrentPosition());
                        tempRiskyCoverPoint = point;
                    }
                }
                else
                {
                    float distanceFromPoint = point.distanceTo(opponent.getCurrentPosition());
                    if(distanceToClosestCoverPoint > distanceFromPoint && point.canFireToTarget(fireRangeDistance) && distanceFromPoint > 5)
                    {
                        distanceToClosestCoverPoint = distanceFromPoint;
                        tempRiskyCoverPoint = point;
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
        else if(tempSafeCoverPoint != null && Vector2.Distance(opponent.getCurrentPosition(), tempSafeCoverPoint.transform.position) <= COVER_POINT_MIN_DISTANCE)
        {
            //tempSafeCoverPoint.stPointOccupentsName(selfAgent.getName());
            tempSafeCoverPoint.setOccupent(m_selfAgent);
            return tempSafeCoverPoint;
        }
        else if(tempRiskyCoverPoint !=null)
        {
            return tempRiskyCoverPoint;
        }

        return null;
    }
    
    private void findNearOpponent()
    {
        Collider[] hitColliders = Physics.OverlapSphere(m_selfAgent.getCurrentPosition(), 15);

        float minDistance = 99999;
        HumanoidMovingAgent closetsAgent = null;

        foreach(Collider hitCollider in hitColliders)
        {
            if(hitCollider.tag == "Player")
            {
                HumanoidMovingAgent mAgent = hitCollider.transform.root.GetComponent<HumanoidMovingAgent>();

                if(mAgent && !m_selfAgent.Equals(mAgent) && !m_selfAgent.getFaction().Equals(mAgent.getFaction()) && mAgent.IsFunctional())
                {
                    float distance = Vector3.Distance(m_selfAgent.getCurrentPosition(),mAgent.getCurrentPosition());

                    if(distance < minDistance)
                    {
                        closetsAgent = mAgent;
                        minDistance = distance;
                    }
                }

            }
        }


        if(closetsAgent)
        {
            opponent = closetsAgent;
        }
    }

    private void findTargetLocationToFire()
    {

        //findNearOpponent();
        HumanoidMovingAgent humanoidOpponent = opponent as HumanoidMovingAgent;

        if(humanoidOpponent != null && humanoidOpponent.isCrouched() && humanoidOpponent.isAimed())
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
        else
        {
            randomOffset = Vector3.zero;
        }

    }

    private bool isTargetFar()
    {
        return (Vector3.Distance(targetLocation.position,this.m_selfAgent.getCurrentPosition()) > 5);
    }
    #endregion

    #region Getters and Setters

    public override void setWeaponFireCapability(bool enabled)
    {
        m_selfAgent.setWeponFireCapability(enabled);
    }

    public override void setTargets(ICyberAgent target)
    {
        if(opponent == null || target != this.opponent)
        {
            this.opponent = target; 
            targetLocations = opponent.getTransfrom().gameObject.GetComponentsInChildren<Collider>();
        }
         //targetLocations = opponent.getTransfrom().gameObject.GetComponentsInChildren<Collider>();
         
        findTargetLocationToFire();
    }

    private bool isCoverPointUsable(CoverPoint point)
    {
        return point.isSafeFromTarget() && point.canFireToTarget(fireRangeDistance);
    }

    private void logStage()
    {
        if(loggerStage != currentCombatSubStage)
        {
            loggerStage = currentCombatSubStage;
            Debug.Log("Stage" + m_selfAgent.getGameObject().name + currentCombatSubStage);
        }
    }

    private void logSubStage()
    {
        if(logShootingStage != currentCoverShootingSubStage)
        {
            logShootingStage = currentCoverShootingSubStage;
            Debug.Log("Sub Stage " + m_selfAgent.getGameObject().name + currentCoverShootingSubStage);
        }
    }

    public override void endStage()
    {
        if(currentCoverPoint != null)
        {
            currentCoverPoint.setOccupent(null);
        }
    }

    #endregion


}
