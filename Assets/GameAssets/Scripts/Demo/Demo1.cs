using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Demo1 : MonoBehaviour
{
    // Start is called before the first frame update
    public HumanoidMovingAgent[] Robots;
    public FlyingAgent[] Drones;

    private int robotID;

    private int DroneID;

    void Start()
    {
        foreach(ICyberAgent agent in Robots)
        {
            agent.getTransfrom().gameObject.SetActive(false);
        }

        foreach(ICyberAgent agent in Drones)
        {
            agent.getTransfrom().gameObject.SetActive(false);
        }       
        robotID = 0;
        DroneID = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            EnableRobots();
        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            EnableDrones();
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            reastartCurrentLevel();
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    private void EnableRobots()
    {
        Robots[robotID].gameObject.SetActive(true);
        robotID ++;

    }

    private void EnableDrones()
    {
        DroneID ++;
        Drones[DroneID].gameObject.SetActive(true);  
    }

    private void reastartCurrentLevel()
    {
        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
    }
}
