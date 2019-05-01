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
    private ICyberAgent m_enemy;

    private float tempFloat;

    #region initalize
    void Awake()
    {
        m_navMeshAgent = this.GetComponent<NavMeshAgent>();
        m_navMeshAgent.updateRotation = false;

        m_selfAgent = this.GetComponent<FlyingAgent>();
        m_selfAgent.setFaction(m_agentFaction);

        //if(m_selfAgent == null)
        //{
        //    this.gameObject.AddComponent(typeof(FlyingAgent));
        //}

        m_selfAgent.setonDestoryCallback(onDestroyDrone);


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

        m_selfAgent.aimWeapon();

        tempFloat = Random.value * 10;

    }
    #endregion

    #region update

    // Update is called once per frame
    void Update()
    {
        if(m_selfAgent.IsFunctional())
        {
            droneUpdate();
        }
    }

    private void droneUpdate()
    {
        m_navMeshAgent.SetDestination(m_enemy.getTransfrom().transform.position + new Vector3(tempFloat, 0, tempFloat));
        m_navMeshAgent.updateRotation = true;

        if (!m_navMeshAgent.pathPending)
        {
            Vector3 velocity = m_navMeshAgent.desiredVelocity;
            velocity = new Vector3(velocity.x, 0, velocity.z);
            m_selfAgent.moveCharacter(velocity);
        }

        m_selfAgent.setTargetPoint(m_enemy.getTransfrom().position);
        m_selfAgent.updateAgent();
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

    void onDestroyDrone()
    {
        Debug.Log("destory");
        m_navMeshAgent.isStopped = true;
        m_navMeshAgent.enabled = false;
        this.gameObject.SetActive(false);
    }

    #endregion

}
