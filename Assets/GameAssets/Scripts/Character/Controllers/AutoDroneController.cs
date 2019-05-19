using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using humanoid;

[RequireComponent(typeof(FlyingAgent))]
public class AutoDroneController :  AgentController
{
    // Start is called before the first frame update
    public string enemyTag;
    public float skill;
    public float health = 1;

    private FlyingAgent m_selfAgent;
    private NavMeshAgent m_navMeshAgent;
    private ICyberAgent m_enemy;
    private ICharacterBehaviorState m_behaviorState;

    private float tempFloat;

    #region initalize
    void Awake()
    {
        initalizeNavMeshAgent();      
        initalizeTarget();
        m_selfAgent = this.GetComponent<FlyingAgent>();
        m_behaviorState = new DroneCombatStage(m_selfAgent, m_navMeshAgent, m_enemy);

        tempFloat = Random.value * 10;
    }

    private void Start()
    {
        initalizeICyberAgent();
    }

    private void initalizeTarget()
    {
        GameObject[] playerTaggedObjects = GameObject.FindGameObjectsWithTag(enemyTag);

        foreach (GameObject obj in playerTaggedObjects)
        {
            if (obj != this.gameObject)
            {
                m_enemy = obj.GetComponent<MovingAgent>();

                if (m_enemy != null)
                {
                    break;
                }
            }

        }
    }

    private void initalizeNavMeshAgent()
    {
        m_navMeshAgent = this.GetComponent<NavMeshAgent>();
        m_navMeshAgent.updateRotation = false;
    }

    private void initalizeICyberAgent()
    {
        m_selfAgent.setFaction(m_agentFaction);
        intializeAgentCallbacks(m_selfAgent);
        m_selfAgent.aimWeapon();
        m_selfAgent.setSkill(skill);
        m_selfAgent.setHealth(health);
    }

    #endregion

    #region update

    // Update is called once per frame
    void Update()
    {
        if(m_selfAgent.IsFunctional() && !m_selfAgent.isDisabled() && isInUse())
        {
            m_behaviorState.updateStage();
        }
    }

    private void droneUpdate()
    {
        //m_navMeshAgent.SetDestination(m_enemy.getTransfrom().transform.position + new Vector3(tempFloat, 0, tempFloat));
        //m_navMeshAgent.updateRotation = true;

        //if (!m_navMeshAgent.pathPending)
        //{
        //    Vector3 velocity = m_navMeshAgent.desiredVelocity;
        //    velocity = new Vector3(velocity.x, 0, velocity.z);
        //    m_selfAgent.moveCharacter(velocity);
        //}

        //m_selfAgent.setTargetPoint(m_enemy.getTransfrom().position);
    }

    #endregion

    #region commands
    #endregion

    #region getters and setters

    public override void setMovableAgent(ICyberAgent agent)
    {
    }

    public override float getSkill()
    {
        throw new System.NotImplementedException();
    }

    public override ICyberAgent getICyberAgent()
    {
        return m_selfAgent;
    }

    #endregion

    #region events
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
        //this.gameObject.SetActive(false);

        //Invoke("resetCharacher", Random.value*5 + 5);
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
        //TEMP CODE
        //this.transform.position = FindObjectOfType<SpawnPoint>().transform.position + new Vector3(0,20,0);


        m_selfAgent.resetAgent(health, skill);

        if(m_navMeshAgent.isOnNavMesh && m_navMeshAgent.isStopped)
        {
            m_navMeshAgent.isStopped = false;
        }

        m_navMeshAgent.enabled = true;
        this.gameObject.SetActive(true);
    }



    #endregion

}
