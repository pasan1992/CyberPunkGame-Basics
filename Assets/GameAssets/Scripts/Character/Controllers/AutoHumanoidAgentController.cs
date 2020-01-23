using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(HumanoidMovingAgent))]
public class AutoHumanoidAgentController :  AgentController
{
    public HumanoidMovingAgent target;

    // public HumanoidMovingAgent followingTarget;    
    protected HumanoidMovingAgent m_movingAgent;
    protected ICharacterBehaviorState m_currentState;
    protected ICharacterBehaviorState m_combatStage;
    protected ICharacterBehaviorState m_idleStage;
    protected HumanoidAgentBasicVisualSensor m_visualSensor;
    protected NavMeshAgent m_navMeshAgent;

    // private GameObject selfCoverPoint;
    //public float health;
    public WaypointRutine rutine;


    private float timeFromLastSwitch;
    private float MaxomumWaitTimeToSwitch  = 5;

    #region initaialize

    private void Awake()
    {
        m_movingAgent = this.GetComponent<HumanoidMovingAgent>();
        m_navMeshAgent = this.GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        inializeGuard();

        // Initalize Agent and sensors
        m_visualSensor = new HumanoidAgentBasicVisualSensor(m_movingAgent);
        m_visualSensor.setOnEnemyDetectionEvent(onEnemyDetection);
        m_visualSensor.setOnAllClear(onAllClear);
        m_movingAgent.setWeponFireCapability(true);
        intializeAgentCallbacks(m_movingAgent);
        m_movingAgent.enableTranslateMovment(false);
        m_movingAgent.setOnDamagedCallback(onDamaged);
        m_currentState = m_idleStage;
        EnvironmentSound.Instance.listenToSound(onSoundAlert);
    }

    private void inializeGuard()
    {
        m_combatStage = new CoverPointBasedCombatStage(m_movingAgent,m_navMeshAgent,GameEnums.MovmentBehaviorType.FREE);
        m_idleStage = new IteractionStage(m_movingAgent,m_navMeshAgent,rutine.m_wayPoints.ToArray());
    }
    #endregion

    #region update
    public void Update()
    {
        //timeFromLastSwitch += Time.deltaTime;
        
        if(m_currentState != null && m_movingAgent.IsFunctional() && !m_movingAgent.isDisabled() & isInUse())
        {
            //m_currentState.updateStage();
            m_visualSensor.UpdateSensor();
        }
    }

    void FixedUpdate()
    {
        if(m_currentState != null && m_movingAgent.IsFunctional() && !m_movingAgent.isDisabled() & isInUse())
        {
            m_currentState.updateStage();
        }
    }
    #endregion

    #region events

    public void onDamaged()
    {
        if(m_currentState != m_combatStage)
        {
            switchToCombatStage();
        }
        else
        {
            ((CoverPointBasedCombatStage)m_combatStage).alrtDamage();
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
        EnvironmentSound.Instance.removeListen(onSoundAlert);
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
    #endregion

    private void switchToCombatStage()
    {
        timeFromLastSwitch +=2;
        if(timeFromLastSwitch > MaxomumWaitTimeToSwitch)
        {
            if(m_currentState != m_combatStage)
            {
                m_movingAgent.cancleInteraction();
                timeFromLastSwitch = 0;
                m_combatStage.initalizeStage();
                m_currentState = m_combatStage;
            }
        }
    }

    private IEnumerator switchFromCombatStageToIteractionStage()
    {
        yield return StartCoroutine(endCombatStage());
        swithtoIteractionStage();
    }

    private IEnumerator endCombatStage()
    {
        if(m_currentState.Equals(m_combatStage))
        {
            
            m_currentState.endStage();
            m_currentState = null;

            if(m_movingAgent.isHidden())
            {
                m_movingAgent.toggleHide();
            }

            if(m_movingAgent.isAimed())
            {
                m_movingAgent.stopAiming();
            }
           // This corutine will run until weapon is hosted
            yield return StartCoroutine(m_movingAgent.waitTillUnarmed());
        }
    }

    private void swithtoIteractionStage()
    {
        m_currentState = m_idleStage;
        m_idleStage.initalizeStage();
    }
    

    #region Events
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
        timeFromLastSwitch++;
        if(timeFromLastSwitch > MaxomumWaitTimeToSwitch)
        {
            timeFromLastSwitch = 0;

            if(!m_currentState.Equals(m_idleStage))
            {
                StartCoroutine(switchFromCombatStageToIteractionStage());
            }
        }
    }

    public void onSoundAlert(Vector3 position, AgentBasicData.AgentFaction faction)
    {
        if( m_currentState != m_combatStage)
        {
            //switchToCombatStage();
            m_visualSensor.forceUpdateSneosr();
        }     

        // If sound is comming from a enemy agent, force guess the location of the agent
        if(!faction.Equals(m_movingAgent.AgentData.m_agentFaction))
        {
            m_visualSensor.forceGussedTargetLocation(position);
        }
    }
    #endregion

    
}
