using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using humanoid;

public class EnemyPool : MonoBehaviour
{
    // Start is called before the first frame update
    public AutoDroneController[] drones;
    public AutoHumanoidAgentController[] droids;
    SpawnPoint[] spawnPoints;
    

    private AgentController.agentOnDestoryEventDelegate m_onDestoryEvent;
    private PlayerControllerMobile m_mobilePlayer;

    void Awake()
    {
        //droids = this.GetComponentsInChildren<AutoHumanoidAgentController>();
        //drones = this.GetComponentsInChildren<AutoDroneController>();
        spawnPoints = FindObjectsOfType<SpawnPoint>();
        m_mobilePlayer = FindObjectOfType<PlayerControllerMobile>();

        //foreach (var droid in droids)
        //{
        //    droid.addOnDestroyEvent(OnAgentDestory);
        //}

        //foreach (var drone in drones)
        //{
        //    drone.addOnDestroyEvent(OnAgentDestory);
        //}
    }

    public void OnAgentDestory(AgentController controller)
    {
        m_onDestoryEvent(controller);

        if (m_mobilePlayer != null)
        {
            m_mobilePlayer.removeTarget(controller.getICyberAgent());
        }
    }

    public void setPoolAgentOndestoryEvent(AgentController.agentOnDestoryEventDelegate onDestoryEvent)
    {
        m_onDestoryEvent += onDestoryEvent;
    }

    public AutoDroneController getDrone(Vector3 position, float health, float skill)
    {
        foreach (var drone in drones)
        {
            if(!drone.isInUse())
            {
                //drone.gameObject.SetActive(true);
                drone.setPosition(position);
                drone.health = health;
                drone.skill = skill;

                drone.addOnDestroyEvent(OnAgentDestory);
                drone.resetCharacher();
                drone.setInUse(true);
                if (m_mobilePlayer !=null)
                {
                    m_mobilePlayer.addTarget(drone.getICyberAgent());
                }
                drone.transform.position = position;
                return drone;
            }
        }

        return null;
    }

    public AutoHumanoidAgentController getDroid(Vector3 position, int health, float skill)
    {
        foreach (var droid in droids)
        {
            if (!droid.isInUse())
            {
                droid.setPosition(position);
                droid.health = health;
                droid.skillLevel = skill;

                droid.addOnDestroyEvent(OnAgentDestory);
                droid.resetCharacher();
                droid.setInUse(true);

                if (m_mobilePlayer != null)
                {
                    m_mobilePlayer.addTarget(droid.getICyberAgent());
                }
                
                if( (Random.value + skill > 0.95f) && Random.value > 0.5f)
                {
                    MovingAgent movingAgent = droid.getICyberAgent() as MovingAgent;

                    if(movingAgent.getCurrentWeaponType().Equals(Weapon.WEAPONTYPE.secondary))
                    {
                        movingAgent.togglePrimaryWeapon();
                    }
                }

                return droid;
            }
        }

        return null;
    }


    public AgentController[] getAllDroids()
    {
        return droids;
    }

    public AgentController[] getAllDrones()
    {
        return drones;
    }
 }
