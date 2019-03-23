using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIDrone : MonoBehaviour, AgentController
{
    // Start is called before the first frame update
    public string enemyTag;

    private FlyingAgent m_agent;
    private NavMeshAgent m_navMeshAgent;
    private MovingAgent enemy;

    private float tempFloat;

    #region initalize
    void Awake()
    {
        m_agent = this.GetComponent<FlyingAgent>();
        m_navMeshAgent = this.GetComponent<NavMeshAgent>();
        m_navMeshAgent.updateRotation = false;

        // Finding Player
        GameObject[] playerTaggedObjects = GameObject.FindGameObjectsWithTag(enemyTag);

        foreach (GameObject obj in playerTaggedObjects)
        {
            if (obj != this.gameObject)
            {
                enemy = obj.GetComponent<MovingAgent>();

                if (enemy != null)
                {
                    break;
                }
            }

        }

        m_agent.AimWeapon();

        tempFloat = Random.value * 10;

    }
    #endregion

    #region update

    // Update is called once per frame
    void Update()
    {
       m_navMeshAgent.SetDestination(enemy.transform.position + new Vector3(tempFloat, 0, tempFloat));
        m_navMeshAgent.updateRotation = false;

        if (!m_navMeshAgent.pathPending)
        {
            Vector3 velocity = m_navMeshAgent.desiredVelocity;
            velocity = new Vector3(velocity.x, 0, velocity.z);
            m_agent.moveCharacter(velocity);
        }

        m_agent.setTargetPoint(enemy.transform.position);
    }

    #endregion

    #region commands
    #endregion

    #region getters and setters

    public void setMovableAgent(ICyberAgent agent)
    {
    }

    #endregion

    #region events
    #endregion

}
