using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(HumanoidMovingAgent))]
public class FSMAutoCharacterController : AgentController
{
    public HumanoidMovingAgent target;
    protected HumanoidMovingAgent m_movingAgent;
    protected ICharacterBehaviorState m_currentState;
    protected ICharacterBehaviorState m_combatStage;

    protected ICharacterBehaviorState m_iteractionStage;

    protected HumanoidAgentBasicVisualSensor m_visualSensor;

    protected NavMeshAgent m_navMeshAgent;
    //public float health;

    public BasicWaypoint[] basicWaypoints;

    public WaypointRutine rutine;

    private float timeFromLastSwitch;

    private float MaxomumWaitTimeToSwitch  = 2;

    #region initaialize

    private void Awake()
    {
        m_movingAgent = this.GetComponent<HumanoidMovingAgent>();
    }

    void Start()
    {
        m_navMeshAgent = this.GetComponent<NavMeshAgent>();
        m_combatStage = new CombatStage(m_movingAgent, target,m_navMeshAgent);

        if(rutine == null)
        {
            m_iteractionStage = new IteractionStage(m_movingAgent,m_navMeshAgent,basicWaypoints);
        }
        else
        {
            m_iteractionStage = new IteractionStage(m_movingAgent,m_navMeshAgent,rutine.m_wayPoints.ToArray());
        }
        

        m_visualSensor = new HumanoidAgentBasicVisualSensor(m_movingAgent);
        m_visualSensor.setOnEnemyDetectionEvent(onEnemyDetection);
        m_visualSensor.setOnAllClear(onAllClear);
        m_movingAgent.setWeponFireCapability(true);
        intializeAgentCallbacks(m_movingAgent);
        m_movingAgent.enableTranslateMovment(false);
        m_movingAgent.setOnDamagedCallback(onDamaged);
       // m_currentState = m_combatStage;
        m_currentState = m_iteractionStage;
       //switchToCombatStage();
    }
    #endregion

    #region update
    public void Update()
    {
        timeFromLastSwitch += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if(m_currentState != null && m_movingAgent.IsFunctional() && !m_movingAgent.isDisabled() & isInUse())
        {
            m_currentState.updateStage();
            m_visualSensor.UpdateSensor();
        }
    }
    #endregion

    #region events

    void OnBecameVisible()
    {
        //m_currentState.setWeaponFireCapability(true);
    }

    void OnBecameInvisible()
    {
        //m_currentState.setWeaponFireCapability(false);
    }

    public void onDamaged()
    {
        if(m_currentState != m_combatStage)
        {
            switchToCombatStage();
        }
    }

    public override void OnAgentDestroy()
    {
        base.OnAgentDestroy();
        m_navMeshAgent.enabled = false;

        if(m_currentState == m_combatStage)
        {
            m_combatStage.endStage();
        }
        
        // reset character
        Invoke("reUseCharacter", m_timeToReset);
    }

    private void reUseCharacter()
    {
        setInUse(false);
    }

    public override void resetCharacher()
    {
        if (m_navMeshAgent != null)
        {
            m_navMeshAgent.enabled = true;
        }

        if (m_movingAgent != null)
        {
        }
    }

    public override void onAgentDisable()
    {
        m_navMeshAgent.enabled = false;
    }

    public override void onAgentEnable()
    {
        m_navMeshAgent.enabled = true;
    }

    #endregion

    #region getters and setters

    public override float getSkill()
    {
        return m_movingAgent.AgentData.Skill;
    }

    public override void setMovableAgent(ICyberAgent agent)
    {
        m_movingAgent = (HumanoidMovingAgent)agent;
    }

    public override ICyberAgent getICyberAgent()
    {
        return m_movingAgent;
    }

    public override void setPosition(Vector3 postion)
    {
        m_navMeshAgent.Warp(postion);
    }

    private void switchToCombatStage()
    {
        if(timeFromLastSwitch > MaxomumWaitTimeToSwitch)
        {
            if(m_currentState != m_combatStage)
            {
                timeFromLastSwitch = 0;
                ((CombatStage)m_combatStage).initalizeStage();
                m_currentState = m_combatStage;
            }
        }
    }

    private void swithtoIteractionStage()
    {
        m_currentState = m_iteractionStage;
    }

    public void onEnemyDetection(ICyberAgent opponent)
    {
        m_combatStage.setTargets(opponent);
        if(m_currentState != m_combatStage)
        {
            switchToCombatStage();
        }
    }

    public void onAllClear()
    {
        if(timeFromLastSwitch > MaxomumWaitTimeToSwitch)
        {
            
            if(m_currentState != m_iteractionStage)
            {
                m_combatStage.endStage();
                timeFromLastSwitch = 0;
                m_currentState = m_iteractionStage;
                m_iteractionStage.initalizeStage();
               // Debug.Log(m_movingAgent.name);

                if(m_movingAgent.isHidden())
                {
                    m_movingAgent.toggleHide();
                }

                if(m_movingAgent.isAimed())
                {
                    m_movingAgent.stopAiming();
                }

                m_movingAgent.hosterWeapon();
            }
        }
    }
    #endregion
}
