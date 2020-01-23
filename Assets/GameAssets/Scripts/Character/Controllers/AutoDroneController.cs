using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(FlyingAgent))]
public class AutoDroneController :  AgentController
{
    // Start is called before the first frame update
    public string enemyTag;

    private FlyingAgent m_selfAgent;
    private NavMeshAgent m_navMeshAgent;
    private ICharacterBehaviorState m_currentBehaviorState;
    private ICharacterBehaviorState m_combatState;
    private ICharacterBehaviorState m_itearationState;
    private HumanoidAgentBasicVisualSensor m_visualSensor;
    private bool inStateTransaction = false;

    #region initalize
    void Awake()
    {
        m_navMeshAgent = this.GetComponent<NavMeshAgent>();
        m_navMeshAgent.updateRotation = false;    
        m_selfAgent = this.GetComponent<FlyingAgent>();

        m_itearationState = new IteractionStage(m_selfAgent,m_navMeshAgent,m_selfAgent.getGameObject().GetComponent<WaypointRutine>().m_wayPoints.ToArray());
        m_combatState = new DroneCombatStage(m_selfAgent,m_navMeshAgent,FindObjectOfType<PlayerController>().GetComponent<HumanoidMovingAgent>());
        m_currentBehaviorState  = m_itearationState;

        m_visualSensor = new HumanoidAgentBasicVisualSensor(m_selfAgent);
    }

    private void Start()
    {
        intializeAgentCallbacks(m_selfAgent);
        m_selfAgent.setOnDamagedCallback(onDamaged);

        m_visualSensor.setOnEnemyDetectionEvent(onEnemyDetection);
        m_visualSensor.setOnAllClear(onAllClear);
        
        EnvironmentSound.Instance.listenToSound(onSoundAlert);  
    }

    #endregion

    #region update

    // Update is called once per frame
    void Update()
    {
        if(m_selfAgent.IsFunctional() && !m_selfAgent.isDisabled() && isInUse())
        {
            m_currentBehaviorState.updateStage();

            if(!m_selfAgent.isInteracting())
            {
                m_visualSensor.UpdateSensor();
            }
            
        }
    }

    private void droneUpdate()
    {
    }

    #endregion

    #region commands
    #endregion

    #region getters and setters

    public override void setMovableAgent(ICyberAgent agent)
    {
        m_selfAgent = (FlyingAgent)agent;
    }

    public override float getSkill()
    {
        return m_selfAgent.GetAgentData().Skill;
    }

    public override ICyberAgent getICyberAgent()
    {
        return m_selfAgent;
    }

    private void switchToCombatStage()
    {
        if(m_currentBehaviorState !=m_combatState && !inStateTransaction)
        {
            m_selfAgent.cancleInteraction();
            inStateTransaction = true;
            StartCoroutine(waitTillInteractionStopAndSwitchToCombat());
        }  
    }

    #endregion

    #region events

    public void onDamaged()
    {
        switchToCombatStage();
    }
    public void onSoundAlert(Vector3 position, AgentBasicData.AgentFaction faction)
    {
        if( m_currentBehaviorState != m_combatState)
        {
            m_visualSensor.forceUpdateSneosr();
        }     

        // If sound is comming from a enemy agent, force guess the location of the agent
        if(!faction.Equals(m_selfAgent.GetAgentData().m_agentFaction))
        {
            m_visualSensor.forceGussedTargetLocation(position);
        }
     }

    public void onEnemyDetection(ICyberAgent opponent)
    {
         m_combatState.setTargets(opponent);
        switchToCombatStage();
    }

    IEnumerator waitTillInteractionStopAndSwitchToCombat()
    {
        yield return m_selfAgent.waitTilInteractionOver();
        m_currentBehaviorState = m_combatState;
        inStateTransaction = false;
        m_navMeshAgent.enabled =true;
    }

    public void onAllClear()
    {
        if(m_currentBehaviorState != m_itearationState)
        {
            m_currentBehaviorState =m_itearationState;
        }
    }
    public override void OnAgentDestroy()
    {
        //TEst
        base.OnAgentDestroy();
        if(m_navMeshAgent.isOnNavMesh && m_navMeshAgent.isStopped)
        {
            m_navMeshAgent.isStopped = true;
        }

        m_navMeshAgent.enabled = false;

        this.transform.position = new Vector3(3.18f, 3.27f, 52.93f);
        setInUse(false);
    }


    public override void onAgentDisable()
    {
        m_navMeshAgent.isStopped = true;
        m_navMeshAgent.velocity = Vector3.zero;
    }

    public override void onAgentEnable()
    {
        //m_navMeshAgent.enabled = true;
        this.transform.position = m_selfAgent.transform.position;
        m_selfAgent.transform.parent = this.transform;

        if(m_navMeshAgent.isOnNavMesh && m_navMeshAgent.isStopped)
        {
            m_navMeshAgent.isStopped = false;
        }

    }

    public override void resetCharacher()
    {
        m_selfAgent.resetAgent();

        if(m_navMeshAgent.isOnNavMesh && m_navMeshAgent.isStopped)
        {
            m_navMeshAgent.isStopped = false;
        }

        m_navMeshAgent.enabled = true;
        this.gameObject.SetActive(true);
    }

    public override void setPosition(Vector3 postion)
    {
        m_navMeshAgent.Warp(postion);
    }

    #endregion

}
