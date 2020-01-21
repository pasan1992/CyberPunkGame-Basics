using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CompanionController : AgentController
{
    private ICyberAgent m_companionAgent;
    private Vector3 m_currentMovmentVector = Vector3.zero;
    private NavMeshAgent m_navMeshAgent;
    protected CoverPointBasedCombatStage m_combatStage;
    protected BasicMovmentCombatStage m_followStage;
    protected HumanoidAgentBasicVisualSensor m_visualSensor;
    private RaycastHit m_raycastHit;
    public HumanoidMovingAgent m_followingTarget;
    private GameObject m_selfCoverPoint;
    private ICharacterBehaviorState m_currentState;

    // Start is called before the first frame update
    void Awake()
    {
        GameObject coverpointprefab = Resources.Load<GameObject>("Prefab/CoverPoint");
        m_selfCoverPoint = GameObject.Instantiate(coverpointprefab);
        m_companionAgent = GetComponent<ICyberAgent>();
        m_navMeshAgent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        // Initialize navmesg agent
        m_navMeshAgent.updateRotation = false;

        // Intalize Stages
        m_combatStage = new CoverPointBasedCombatStage(m_companionAgent,m_navMeshAgent,GameEnums.MovmentBehaviorType.FIXED_POSITION, m_selfCoverPoint.GetComponent<CoverPoint>());
        m_followStage = new BasicMovmentCombatStage(m_companionAgent,m_navMeshAgent);
        m_followStage.CurrentMovmentType = GameEnums.MovmentBehaviorType.FIXED_POSITION;
        m_currentState = m_followStage;
        m_combatStage.CenteredPosition = m_followingTarget.getCurrentPosition();

        m_visualSensor = new HumanoidAgentBasicVisualSensor(m_companionAgent);

        // Register Events
        m_visualSensor.setOnEnemyDetectionEvent(onEnemyDetection);
        m_visualSensor.setOnAllClear(onAllClear);
        m_companionAgent.setOnDamagedCallback(onDamaged);
        
    }

    // Update is called once per frame
    void Update()
    {
       if(m_companionAgent.IsFunctional())
       {
            updatePlayerInput();

            m_visualSensor.UpdateSensor();

            m_followStage.CenteredPosition = m_followingTarget.getCurrentPosition() + Vector3.left*2;    

            if(m_currentState !=null)
            {
                m_currentState.updateStage();    
            }
            
       }
    }

    private void updatePlayerInput()
    {
        if(Input.GetMouseButtonUp(0) && !Input.GetMouseButton(1))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out m_raycastHit, 100,LayerMask.GetMask("Floor"))) 
            {
                m_combatStage.CenteredPosition = m_raycastHit.point;
                m_combatStage.CurrentMovmentBehaviorStage = GameEnums.MovmentBehaviorStage.CALULATING_NEXT_POINT;
            }
        }
    }
    public override ICyberAgent getICyberAgent()
    {
        return m_companionAgent;
    }

    public override float getSkill()
    {
        return m_companionAgent.getSkill();
    }

    public override void onAgentDisable()
    {
        throw new System.NotImplementedException();
    }

    public override void onAgentEnable()
    {
        throw new System.NotImplementedException();
    }

    public override void resetCharacher()
    {
        if (m_navMeshAgent != null)
        {
            m_navMeshAgent.enabled = true;
        }
    }

    public override void setMovableAgent(ICyberAgent agent)
    {
        m_companionAgent = agent;
    }

    public override void setPosition(Vector3 postion)
    {
        m_navMeshAgent.Warp(postion);
    }

    public void onEnemyDetection(ICyberAgent opponent)
    {
        m_combatStage.setTargets(opponent);

        if(m_currentState != m_combatStage)
        {
            switchToCombatStage();
        }
    }

    private void switchToCombatStage()
    {
        if(m_currentState != m_combatStage)
        {
            Debug.Log("Starting Combat Stage");
            m_companionAgent.cancleInteraction();
            m_combatStage.initalizeStage();
            m_currentState = m_combatStage;
        }
    }

    public void onDamaged()
    {
        m_combatStage.alrtDamage();
    }

    public void onAllClear()
    {
        if(!m_currentState.Equals(m_followStage))
        {
            StartCoroutine(switchFromCombatStageToFollowStage());
        }
    }

    private IEnumerator endCombatStage()
    {
        if(m_currentState.Equals(m_combatStage))
        {
            
            m_currentState.endStage();
            m_currentState = null;

            if(m_companionAgent.isHidden())
            {
                m_companionAgent.toggleHide();
            }

            if(m_companionAgent.isAimed())
            {
                m_companionAgent.stopAiming();
            }

           // This corutine will run until weapon is hosted
            yield return StartCoroutine(m_companionAgent.waitTillUnarmed());
        }
    }

    private IEnumerator switchFromCombatStageToFollowStage()
    {
        yield return StartCoroutine(endCombatStage());
        swithToFollowStage();
    }

    private void swithToFollowStage()
    {
        m_currentState = m_followStage;
        m_followStage.initalizeStage();
    }
}
