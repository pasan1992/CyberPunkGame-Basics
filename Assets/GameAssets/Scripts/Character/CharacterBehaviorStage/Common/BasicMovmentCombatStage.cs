using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicMovmentCombatStage : BasicMovmentStage
{
    protected GameEnums.MovmentBehaviorType m_currentMovmentType;
    protected GameEnums.MovmentBehaviorStage m_currentMovmentBehaviorStage;
    protected float maxDistnaceFromCenteredPoint = 10;
    protected Vector3 centeredPosition = Vector3.zero;

    public BasicMovmentCombatStage(ICyberAgent selfAgent,NavMeshAgent agent):base(selfAgent,agent)
    {

    }
    public GameEnums.MovmentBehaviorType CurrentMovmentState 
    {
         get { return m_currentMovmentType;}
         set 
         {
             m_currentMovmentType = value;
             switch (m_currentMovmentType)
             {
                 case GameEnums.MovmentBehaviorType.FIXED_POSITION:
                 break;
                 case GameEnums.MovmentBehaviorType.FREE:
                    // When centered positon is zero, center point is not concidered when calculating coverpoints
                    CenteredPosition = Vector3.zero;
                 break;
                 case GameEnums.MovmentBehaviorType.NEAR_POINT:
                 break;
             }
         }
    }
    protected GameEnums.MovmentBehaviorStage CurrentMovmentBehaviorStage { get => m_currentMovmentBehaviorStage; set => m_currentMovmentBehaviorStage = value; }
    public Vector3 CenteredPosition { get => centeredPosition; set => centeredPosition = value; }
    public float MaxDistnaceFromCenteredPoint { get => maxDistnaceFromCenteredPoint; set => maxDistnaceFromCenteredPoint = value; }

    public override void setTargets(ICyberAgent target)
    {
        throw new System.NotImplementedException();
    }

    protected override void stepUpdate()
    {
        switch (m_currentMovmentType)
        {
            case GameEnums.MovmentBehaviorType.FIXED_POSITION:
                updateFixedPositionMovment();
            break;
            case GameEnums.MovmentBehaviorType.FREE:
                updateFreePositionMovment();
            break;
            case GameEnums.MovmentBehaviorType.NEAR_POINT:
                updateNearPointPositonMovment();
            break;
        }
    }

    protected virtual void updateFixedPositionMovment()
    {
        switch (m_currentMovmentBehaviorStage)
        {
           case GameEnums.MovmentBehaviorStage.CALULATING_NEXT_POINT:
           break;
           case GameEnums.MovmentBehaviorStage.MOVING_TO_POINT:
                
           break;
           case GameEnums.MovmentBehaviorStage.AT_POINT:

                // If current distination is different from the new fixed position
                if( m_navMeshAgent.destination != null && Vector3.Distance(m_navMeshAgent.destination,centeredPosition) > 2)
                {
                    m_navMeshAgent.destination = centeredPosition;
                    m_currentMovmentBehaviorStage = GameEnums.MovmentBehaviorStage.MOVING_TO_POINT;
                }

           break;     
        }
    }
    protected virtual void updateFreePositionMovment()
    {
        Debug.LogError("Not Implemented");
        switch (m_currentMovmentBehaviorStage)
        {
           case GameEnums.MovmentBehaviorStage.CALULATING_NEXT_POINT:
           break;
           case GameEnums.MovmentBehaviorStage.MOVING_TO_POINT:
           break;
           case GameEnums.MovmentBehaviorStage.AT_POINT:
           break;     
        }
    }
    protected virtual void updateNearPointPositonMovment()
    {
        Debug.LogError("Not Implemented");
        switch (m_currentMovmentBehaviorStage)
        {
           case GameEnums.MovmentBehaviorStage.CALULATING_NEXT_POINT:
           break;
           case GameEnums.MovmentBehaviorStage.MOVING_TO_POINT:
           break;
           case GameEnums.MovmentBehaviorStage.AT_POINT:
           break;     
        }
    }
}
