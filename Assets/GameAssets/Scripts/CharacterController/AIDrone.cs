using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIDrone : MonoBehaviour, AgentController
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
        m_selfAgent = new FlyingAgent(this.GetComponentInChildren<Animator>(), this.gameObject, this.GetComponentInChildren<Rigidbody>(), onDestroyDrone);

        // Finding Player
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

        m_selfAgent.AimWeapon();

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
        m_navMeshAgent.updateRotation = false;

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

    public void setMovableAgent(ICyberAgent agent)
    {
    }

    public float getSkill()
    {
        throw new System.NotImplementedException();
    }

    public ICyberAgent getICyberAgent()
    {
        return m_selfAgent;
    }

    #endregion

    #region events

    void onDestroyDrone()
    {
        m_navMeshAgent.isStopped = true;
        m_navMeshAgent.enabled = false;
    }

    #endregion

}
