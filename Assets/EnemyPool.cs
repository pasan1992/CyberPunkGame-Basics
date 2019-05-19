using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    // Start is called before the first frame update
    AutoDroneController[] drones;
    AutoHumanoidAgentController[] droids;
    SpawnPoint[] spawnPoints;

    private AgentController.agentOnDestoryEventDelegate m_onDestoryEvent;

    void Awake()
    {
        droids = this.GetComponentsInChildren<AutoHumanoidAgentController>();
        drones = this.GetComponentsInChildren<AutoDroneController>();
        spawnPoints = FindObjectsOfType<SpawnPoint>();

        foreach (var droid in droids)
        {
            droid.addOnDestroyEvent(OnAgentDestory);
        }

        foreach (var drone in drones)
        {
            drone.addOnDestroyEvent(OnAgentDestory);
        }
    }

    public void OnAgentDestory(AgentController controller)
    {
        m_onDestoryEvent(controller);
    }

    public void setPoolAgentOndestoryEvent(AgentController.agentOnDestoryEventDelegate onDestoryEvent)
    {
        m_onDestoryEvent += onDestoryEvent;
    }

    public AutoDroneController getDrone(Vector3 position, float health, float skill)
    {
        foreach (var drone in drones)
        {
            if(!drone.isActiveAndEnabled)
            {
                drone.transform.position = position;
                drone.health = health;
                drone.skill = skill;
                return drone;
            }
        }

        return null;
    }

    public AutoHumanoidAgentController getDroid(Vector3 position, float health, float skill)
    {
        foreach (var droid in droids)
        {
            if (!droid.isActiveAndEnabled)
            {
                droid.transform.position = position;
                droid.health = health;
                droid.skillLevel = skill;
                return droid;
            }
        }

        return null;
    }
}
