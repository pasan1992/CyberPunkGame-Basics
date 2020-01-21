using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CoverPointBasedCombatStage : BasicMovmentCombatStage
{
    private ICyberAgent m_target;
    private CoverPoint m_currentCoverPoint;
    private CoverPoint m_ownedCoverPoint;

    private GameEnums.Cover_based_combat_stages m_coverBasedStages;

    float fireRangeDistance = 15;
    int m_maxTimeLimitAtCover = 20;
    int m_currentTimeAtCover;
    int m_noOfIteractions =4;


    private Collider[] targetLocations;
    private Transform targetLocation;
    private Vector3 randomOffset;
    private float suppriceFactor = 0;

    private bool damageAlert = false;
    public override Vector3 CenteredPosition 
    { get => centeredPosition; 
     set
     {
         centeredPosition = value;
         m_ownedCoverPoint.transform.position = value;
     }
    }


    public override GameEnums.MovmentBehaviorType CurrentMovmentType 
    {
         get { return m_currentMovmentType;}
         set 
         {
             m_currentMovmentType = value;
             switch (m_currentMovmentType)
             {
                 case GameEnums.MovmentBehaviorType.FIXED_POSITION:
                    m_currentCoverPoint.setOccupent(null);
                    m_currentCoverPoint = null;
                    if(m_ownedCoverPoint == null)
                    {
                        Debug.LogError("Need a owned coverpoint for fixed postion movment type");
                    }
                 break;
                 case GameEnums.MovmentBehaviorType.FREE:
                    
                    CenteredPosition = Vector3.zero;
                 break;
                 case GameEnums.MovmentBehaviorType.NEAR_POINT:
                 break;
             }
         }
    }

    public CoverPointBasedCombatStage(ICyberAgent selfAgent, NavMeshAgent agent,GameEnums.MovmentBehaviorType behaviorType, CoverPoint coverPoint) : base(selfAgent, agent)
    {
        m_ownedCoverPoint = coverPoint;
        m_ownedCoverPoint.setOccupent(selfAgent);
        m_currentMovmentType = behaviorType;
    }

    public CoverPointBasedCombatStage(ICyberAgent selfAgent, NavMeshAgent agent,GameEnums.MovmentBehaviorType behaviorType) : base(selfAgent, agent)
    {
        m_currentMovmentType = behaviorType;
    }

    public override void endStage()
    {
        base.endStage();
    }

    public override void initalizeStage()
    {
        base.initalizeStage();
       ((HumanoidMovingAgent)m_selfAgent).togglepSecondaryWeapon();
       m_currentMovmentBehaviorStage = GameEnums.MovmentBehaviorStage.CALULATING_NEXT_POINT;
    }

    public override void setTargets(ICyberAgent target)
    {
        if(target !=null &&  m_target != target)
        {
            m_target = target;
            targetLocations = m_target.getTransfrom().gameObject.GetComponentsInChildren<Collider>();
            suppriceFactor = 1.5f;

            if(m_ownedCoverPoint != null)
            {
                m_ownedCoverPoint.setTargetToCover(target);
            }
            
            // If new target is found, new cover position is needed
           // m_currentMovmentBehaviorStage = GameEnums.MovmentBehaviorStage.CALULATING_NEXT_POINT;
        }
    }

    public override void stopStageBehavior()
    {
        base.stopStageBehavior();
    }
    protected override void stepUpdate()
    {
        base.stepUpdate();
    }

    protected override void updateFreePositionMovment()
    {
        // When centered positon is zero, center point is not concidered when calculating coverpoints
        // m_centeredPoint is set as zero from the super class setter for m_currentMovmentType variable.
        updateGeneralMovment();
        //updateCombatBehavior();
    }

    public override void updateStage()
    {
        base.updateStage();
        aimAtTarget();
    }

    protected override void updateCombatBehavior()
    {
        
        findTargetLocationToFire();
        StateShower.Instance.setText(m_currentMovmentBehaviorStage.ToString() + "-"+ m_coverBasedStages.ToString() + ":"+ m_noOfIteractions,m_selfAgent);
        //Debug.Log(m_currentMovmentBehaviorStage);
       
        switch (m_currentMovmentBehaviorStage)
        {
            case GameEnums.MovmentBehaviorStage.AT_POINT:

                if(m_target != null)
                {
                    coverBasedCombat();                
                }
                else
                {
                    Debug.LogError("No Target For" + m_selfAgent.getTransfrom().name);
                }
                
            break;
            case GameEnums.MovmentBehaviorStage.MOVING_TO_POINT:
                if (m_target !=null && Vector3.Distance(m_selfAgent.getCurrentPosition(), m_target.getCurrentPosition()) < fireRangeDistance)
                {
                    m_selfAgent.aimWeapon();
                    m_enableRun = false;
                    m_selfAgent.weaponFireForAI();
                    setStepIntervalSize(0.3f);
                }
                else
                {
                    m_selfAgent.stopAiming();
                    m_enableRun = true;
                    
                    getUpFromCover();
                }

                if(damageAlert)
                {
                    m_selfAgent.dodgeAttack(m_navMeshAgent.desiredVelocity);
                    damageAlert = false;
                }
            break;
            case GameEnums.MovmentBehaviorStage.CALULATING_NEXT_POINT:
                getUpFromCover();
            break;
        }
        damageAlert = false;
        
    }

    private void coverBasedCombat()
    {
        switch (m_coverBasedStages)
        {
            case GameEnums.Cover_based_combat_stages.IN_COVER:

                 m_noOfIteractions --;

                 // If Not safe from current target get up and shoot
                 if( (m_currentCoverPoint != null && !m_currentCoverPoint.isSafeFromTarget()) || (m_ownedCoverPoint !=null && !m_ownedCoverPoint.isSafeFromTarget()) )
                 {
                    m_selfAgent.aimWeapon();
                    m_noOfIteractions = (int)Random.Range(4,10);
                    getUpFromCover();
                    m_coverBasedStages = GameEnums.Cover_based_combat_stages.SHOOT;
                    break;    
                 }

                // After staying at cover, peek and shoot
                if(m_noOfIteractions <= 0)
                 {
                    if(Vector3.Distance(m_selfAgent.getCurrentPosition(), m_target.getCurrentPosition()) < fireRangeDistance)
                    {
                        m_selfAgent.aimWeapon();
                        m_noOfIteractions = (int)Random.Range(4,10);
                        m_coverBasedStages = GameEnums.Cover_based_combat_stages.SHOOT;
                    }
                    else
                    {
                        //Debug.LogError("Fire Range Not enough for" + m_selfAgent.getTransfrom().name);

                        m_noOfIteractions = 0;
                    }

                    break;
                 }


                // When in cover, hide and do no aim
                if(m_selfAgent.isAimed())
                {
                    m_selfAgent.stopAiming();
                }

                if(!m_selfAgent.isHidden())
                {
                    m_selfAgent.toggleHide();
                }
                 
            break;
            case GameEnums.Cover_based_combat_stages.SHOOT:   

                // When firing is done, return to cover
                if(m_noOfIteractions <= 0)
                {
                    m_noOfIteractions = (int)Random.Range(2,5);
                    m_coverBasedStages = GameEnums.Cover_based_combat_stages.IN_COVER;
                    break;
                }

                if((m_currentCoverPoint != null && !m_currentCoverPoint.isSafeFromTarget()) || (m_ownedCoverPoint != null && !m_ownedCoverPoint.isSafeFromTarget()))
                {
                    getUpFromCover();
                }
                else
                {
                    getCover();
                }

                if(damageAlert)
                {
                    damageAlert = false;

                    if(m_noOfIteractions > 3)
                    {
                        m_noOfIteractions -=3;
                    }
                    else
                    {
                        m_noOfIteractions = (int)Random.Range(4,6);
                        m_coverBasedStages = GameEnums.Cover_based_combat_stages.IN_COVER;
                    }          
                }

                // Shoot 
                m_noOfIteractions --;
                m_selfAgent.weaponFireForAI();
            break;
        }
    }

    private void updateGeneralMovment()
    {
        switch (m_currentMovmentBehaviorStage)
        {
        case GameEnums.MovmentBehaviorStage.CALULATING_NEXT_POINT:
            
            CoverPoint tempCurrentCoverPoint =  CoverPointsManager.getNearCoverObject(m_selfAgent,m_target,fireRangeDistance,true,CenteredPosition,MaxDistnaceFromCenteredPoint);
            
            //Stop the moving agent in case status change from moving - calculate next point
            m_navMeshAgent.velocity = Vector3.zero;
            m_navMeshAgent.isStopped = true;

            if(tempCurrentCoverPoint != null)
            {
                if(m_currentCoverPoint != tempCurrentCoverPoint)
                {
                    // Hande CoverPoint
                    if (m_currentCoverPoint)
                    {
                        m_currentCoverPoint.setOccupent(null);
                    }
                    m_currentCoverPoint = tempCurrentCoverPoint;
                    m_currentCoverPoint.setOccupent(m_selfAgent);

                    m_navMeshAgent.SetDestination(m_currentCoverPoint.getPosition());

                    // Get up and move
                    getUpFromCover();
                    m_currentMovmentBehaviorStage = GameEnums.MovmentBehaviorStage.MOVING_TO_POINT;
                    m_navMeshAgent.isStopped = false;
                }
                else
                {
                    Debug.Log("Same Cover Point");
                }

            }
            // else
            // {
            //     Debug.LogError("Agent "+ m_selfAgent.getGameObject().name + " Cannot find cover points" );
            // }
        break;
        case GameEnums.MovmentBehaviorStage.MOVING_TO_POINT:

            if(CommonFunctions.checkDestniationReached(m_navMeshAgent) || m_navMeshAgent.remainingDistance < 0.3f)
            {
                m_currentMovmentBehaviorStage = GameEnums.MovmentBehaviorStage.AT_POINT;
                m_currentTimeAtCover = 0;
                m_navMeshAgent.velocity = Vector3.zero;
                m_navMeshAgent.isStopped = true;
            }

        break;
        case GameEnums.MovmentBehaviorStage.AT_POINT:
            if(m_maxTimeLimitAtCover < m_currentTimeAtCover)
            {
               m_currentMovmentBehaviorStage = GameEnums.MovmentBehaviorStage.CALULATING_NEXT_POINT;
            }
            m_currentTimeAtCover++;
        break;     
        }
    }

    protected override void updateNearPointPositonMovment()
    {
        updateGeneralMovment();
        //updateCombatBehavior();
    }

    protected override void updateFixedPositionMovment()
    {
        base.updateFixedPositionMovment();
        //updateCombatBehavior();
    }

    private void getUpFromCover()
    {
        if(m_selfAgent.isHidden())
        {
            m_selfAgent.toggleHide();
        }
    }

    private void getCover()
    {
        if(!m_selfAgent.isHidden())
        {
            m_selfAgent.toggleHide();
        }
    }

    public void alrtDamage()
    {
        damageAlert = true;
    }

    private void findTargetLocationToFire()
    {
        if(m_target !=null)
        {
            HumanoidMovingAgent humanoidOpponent = m_target as HumanoidMovingAgent;

            if(humanoidOpponent != null && humanoidOpponent.isCrouched() && humanoidOpponent.isAimed())
            {
                targetLocation = humanoidOpponent.getHeadTransfrom();
            }
            else if (targetLocations !=null && targetLocations.Length !=0)
            {
                int randomIndex = Random.Range(0, targetLocations.Length - 1);
                targetLocation = targetLocations[randomIndex].transform;
            }
            else
            {
                targetLocation = m_target.getTransfrom();
            }

            if(Random.value + suppriceFactor > m_selfAgent.getSkill())
            {
                randomOffset = Random.insideUnitSphere * 0.8f;
            }
            else
            {
                randomOffset = Vector3.zero;
            }

            if(suppriceFactor > 0)
            {
                suppriceFactor -=0.25f;
            }
        }
    }

    
    private void aimAtTarget()
    {
        if(m_target !=null)
        {
            // When self is in cover
            if(m_coverBasedStages.Equals(GameEnums.Cover_based_combat_stages.IN_COVER))
            {
                // If target is hidden aim for the head
                if(m_target.isHidden())
                {
                    // When target is peeking from cover
                    if(m_target.isAimed())
                    {
                        m_selfAgent.setTargetPoint(m_target.getTransfrom().position + new Vector3(0, 1.1f, 0) + randomOffset + m_target.getCurrentVelocity()*0);
                    }
                    // When target is hidden in cover - supress fire
                    else
                    {
                        m_selfAgent.setTargetPoint(m_target.getTransfrom().position + new Vector3(0, 0.4f, 0) + randomOffset + m_target.getCurrentVelocity()*0);
                    }
                }
                // Target is open 
                else
                {
                    m_selfAgent.setTargetPoint(m_target.getTransfrom().position + new Vector3(0, 1.25f, 0) + randomOffset + m_target.getCurrentVelocity()*0);
                }
            }
            else
            {
                m_selfAgent.setTargetPoint(targetLocation.position + randomOffset + m_target.getCurrentVelocity()*0);
            }
        }
    }
}
