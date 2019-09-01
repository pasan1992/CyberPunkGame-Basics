using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using humanoid;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SpaceShipLevel : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Tutorial")]
    public Image MoveJoystick;
    public Image aimJoystick;
    public Image CrouchButton;
    public Image RunButton;
    public Image DogeButton;
    public Image weaponButton1;
    public Image weaponButton2;
    public Text skiptext;
    public Text tutorialText;
    public GameObject blockingObject;


    [Header("Stage 1")]
    public SpawnPoint[] stage1SpawnPoints;
    public int Stage1StartingUnitCount;
    public int Stage1UnitCountIncrementer;
    public int Stage1WaveCount;
    public int Stage1Health;
    public float Stage1Skill;
    public GameDoor Stage1Door;

    [Header("Stage 2")]
    public SpawnPoint[] stage2SpawnPoints;
    public int Stage2StartingUnitCount;
    public int Stage2UnitCountIncrementer;
    public int Stage2WaveCount;
    public int Stage2Health;
    public float Stage2Skill;
    public GameDoor Stage2Door;
    

    [Header("Stage 3")]
    public SpawnPoint[] stage3SpawnPoints;
    public int Stage3StartingUnitCount;
    public int Stage3UnitCountIncrementer;
    public int Stage3WaveCount;
    public int Stage3Health;
    public float Stage3Skill;


    private SpawnPoint[] activeSpawnPoints;
    private int activeUnitCount;
    private int activeIncrementer;
    private int activeWaveCount;
    private int activeHealth;
    private float activeSkill;
    private GameDoor activeDoor;
    private int activeAreaMask;

    private int currentWaveCount;
    private int currentUnitCount;
    private float currentSkill;
    private int currentHealth;

    private EnemyPool m_enemyPool;

    private MovingAgent playerAgent;
    private PlayerControllerMobile playerController;
   
    private float rateOfSpawn = 1.5f;
    private bool continueTutorial = false;

    [Header("Attributes")]
    public int currentStage = 1;
    public bool tutorialOver = false;
    #region Initialize

    [Header("Other UI")]
    public Text restartText;


    void Start()
    {
        m_enemyPool = FindObjectOfType<EnemyPool>();
        playerController = FindObjectOfType<PlayerControllerMobile>();
        playerAgent = (MovingAgent)playerController.getICyberAgent();
        m_enemyPool.setPoolAgentOndestoryEvent(onUnityDestory);
        startTutorial();
        restartText.gameObject.SetActive(false);
        //startStage1();
    }

    public void activeRestart()
    {
        restartText.gameObject.SetActive(true);
        restartText.text = "You are dead. Next time use covers to project from enemy fire. Tap here to restart.";
    }

    public void restartMission()
    {
        Scene loadedLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(loadedLevel.buildIndex);
    }

    #endregion

    #region Update
    #endregion

    #region Getters and Setters
    #endregion

    #region Events

    public void onUnityDestory(AgentController agentController)
    {
        currentUnitCount--;

        // if( (agentController.m_currentType == AgentController.TypeOfController.droid && playerAgent.getSecondaryWeaponAmmoCount() < 20 && Random.value > 0.8f) || (agentController.m_currentType == AgentController.TypeOfController.droid && Random.value < 0.8f))
        // {
        //     ProjectilePool.POOL_OBJECT_TYPE type = ProjectilePool.POOL_OBJECT_TYPE.PistolAmmo;

        //     if (Random.value < 0.3f)
        //     {
        //         type = ProjectilePool.POOL_OBJECT_TYPE.RifleAmmo;
        //     }

        //    GameObject ammoMag =  ProjectilePool.getInstance().getPoolObject(type);

        //     if(ammoMag != null)
        //     {
        //         ammoMag.SetActive(true);
        //         ammoMag.transform.position = agentController.getICyberAgent().getCurrentPosition() + new Vector3(0,2,0);
        //         Rigidbody rb = ammoMag.GetComponent<Rigidbody>();
        //         rb.AddForce(Vector3.up * 2, ForceMode.Impulse);
        //     }

        // }
    }
    #endregion

    #region Commands

    private void startTutorial()
    {
         MoveJoystick.gameObject.SetActive(false);
         aimJoystick.gameObject.SetActive(false);
         CrouchButton.gameObject.SetActive(false);
         RunButton.gameObject.SetActive(false);
         DogeButton.gameObject.SetActive(false);
         weaponButton1.gameObject.SetActive(false);
         weaponButton2.gameObject.SetActive(false);
         blockingObject.gameObject.SetActive(true);
         StartCoroutine(startTutorialBasics());

    }


    private void startStage1()
    {
        activeSpawnPoints = stage1SpawnPoints;
        activeUnitCount = Stage1StartingUnitCount;
        activeIncrementer = Stage1UnitCountIncrementer;
        activeIncrementer = Stage1UnitCountIncrementer;
        activeWaveCount = Stage1WaveCount;
        currentWaveCount = 0;
        currentUnitCount = 0;
        currentHealth = activeHealth =  Stage1Health;
        currentSkill = activeSkill = Stage1Skill;
        StartCoroutine(startCurrentStage());
        activeDoor = Stage1Door;
        rateOfSpawn = 1.6f;
    }

    public void startStage2()
    {
        activeSpawnPoints = stage2SpawnPoints;
        activeUnitCount = Stage2StartingUnitCount;
        activeIncrementer = Stage2UnitCountIncrementer;
        activeIncrementer = Stage2UnitCountIncrementer;
        activeWaveCount = Stage2WaveCount;
        currentWaveCount = 0;
        currentUnitCount = 0;
        currentHealth = activeHealth = Stage2Health;
        currentSkill = activeSkill = Stage2Skill;
        StartCoroutine(startCurrentStage());
        activeDoor = Stage2Door;
        rateOfSpawn = 1.3f;

        activeAreaMask = playerController.getNavMeshAgent().areaMask;
        activeAreaMask += 1 << NavMesh.GetAreaFromName("Area2Block");
        playerController.getNavMeshAgent().areaMask = activeAreaMask;
        currentStage = 2;
    }

    public void startStage3()
    {
        activeSpawnPoints = stage3SpawnPoints;
        activeUnitCount = Stage3StartingUnitCount;
        activeIncrementer = Stage3UnitCountIncrementer;
        activeIncrementer = Stage3UnitCountIncrementer;
        activeWaveCount = Stage3WaveCount;
        currentWaveCount = 0;
        currentUnitCount = 0;
        currentHealth = activeHealth = Stage3Health;
        currentSkill = activeSkill = Stage3Skill;
         StartCoroutine(startCurrentStage());
        activeDoor = null;
        rateOfSpawn = 0.5f;

        activeAreaMask = playerController.getNavMeshAgent().areaMask;
        activeAreaMask += 1 << NavMesh.GetAreaFromName("Area3Bloack");
        playerController.getNavMeshAgent().areaMask = activeAreaMask;
        playerController.getNavMeshAgent().areaMask = activeAreaMask;
        currentStage = 3;
    }



    #endregion

    #region Utility

    public void contTutorial()
    {
        continueTutorial = true;
    }

    public void skipTutorial()
    {
        tutorialOver = true;
        tutorialText.gameObject.SetActive(false);
        skiptext.gameObject.SetActive(false);

        MoveJoystick.gameObject.SetActive(true);
        aimJoystick.gameObject.SetActive(true);
        CrouchButton.gameObject.SetActive(true);
        RunButton.gameObject.SetActive(true);
        DogeButton.gameObject.SetActive(true);
        weaponButton1.gameObject.SetActive(true);
        weaponButton2.gameObject.SetActive(true);

        activeAreaMask = playerController.getNavMeshAgent().areaMask;
        activeAreaMask += 1 << NavMesh.GetAreaFromName("Area1Block");
        playerController.getNavMeshAgent().areaMask = activeAreaMask;

        GameObject fire = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.DroneExplosion);
        fire.transform.position = blockingObject.transform.position + new  Vector3(0,0,-2);
        fire.GetComponent<BasicExplosion>().exploade();
        fire.SetActive(true);
        blockingObject.gameObject.SetActive(false);
        startStage1();
        continueTutorial = true;
    }

    IEnumerator startTutorialBasics()
    {
        yield return new WaitForSeconds(1);

        if (!tutorialOver)
        {
            continueTutorial = false;
            MoveJoystick.gameObject.SetActive(true);
            tutorialText.text = "Move using this joystick. Tap to continue.";
        }


        yield return new WaitUntil(() => continueTutorial);

        if (!tutorialOver)
        {
            continueTutorial = false;
            aimJoystick.gameObject.SetActive(true);
            tutorialText.text = "Aim using this joystick. Tap to continue.";
        }


        yield return new WaitUntil(() => continueTutorial);

        if (!tutorialOver)
        {
            continueTutorial = false;
            CrouchButton.gameObject.SetActive(true);
            tutorialText.text = "Toggle Crouch Using this button. Tap to continue.";
        }


        yield return new WaitUntil(() => continueTutorial);

        if (!tutorialOver)
        {
            continueTutorial = false;
            RunButton.gameObject.SetActive(true);
            tutorialText.text = "Press this button while moving to run. Tap to continue.";
        }


        yield return new WaitUntil(() => continueTutorial);

        if (!tutorialOver)
        {
            continueTutorial = false;
            DogeButton.gameObject.SetActive(true);
            tutorialText.text = "Dodge Using this button. Tap to continue.";
        }

        yield return new WaitUntil(() => continueTutorial);

        if (!tutorialOver)
        {
            continueTutorial = false;
            weaponButton1.gameObject.SetActive(true);
            weaponButton2.gameObject.SetActive(true);
            tutorialText.text = "Selecte Weapons Using these buttons. Tap to continue.";
        }

        yield return new WaitUntil(() => continueTutorial);

        if (!tutorialOver)
        {
            continueTutorial = false;
            tutorialText.text = "Your health dont regenarate. use covers often. Tap to continue.";
        }

        yield return new WaitUntil(() => continueTutorial);

        if (!tutorialOver)
        {
            continueTutorial = false;
            tutorialText.text = "Stay crouch near a cover ojbect to cover from enemy fire. Tap to continue.";
        }

        yield return new WaitUntil(() => continueTutorial);

        if (!tutorialOver)
        {
            continueTutorial = false;
            tutorialText.text = "Dont forget to Pick up Ammo for your weapons. Tap to continue.";
        }



        yield return new WaitUntil(() => continueTutorial);

        if (!tutorialOver)
        {
            continueTutorial = false;
            tutorialText.text = "Your Mission is to Save the space ship. Tap to continue.";
        }

        yield return new WaitUntil(() => continueTutorial);
        if (!tutorialOver)
        {
            continueTutorial = false;
            tutorialText.text = "Move to Next room to start the Demo. Tap to continue.";
            GameObject fire = ProjectilePool.getInstance().getPoolObject(ProjectilePool.POOL_OBJECT_TYPE.FireEXplosionParticle);
            fire.transform.position = blockingObject.transform.position;
            fire.SetActive(true);
            blockingObject.gameObject.SetActive(false);
        }




        if (!tutorialOver)
        {
            activeAreaMask = playerController.getNavMeshAgent().areaMask;
            activeAreaMask += 1 << NavMesh.GetAreaFromName("Area1Block");
            playerController.getNavMeshAgent().areaMask = activeAreaMask;


            tutorialOver = true;
            tutorialText.gameObject.SetActive(false);
            skiptext.gameObject.SetActive(false);
            startStage1();
        }



    }

    IEnumerator startCurrentStage()
    {
        yield return new WaitForSeconds(1);

        for (currentWaveCount = 0; currentWaveCount < activeWaveCount; currentWaveCount++)
        {
            for (int i = 0; i < activeUnitCount; i++)
            {
                AgentController agent = getRandomAgent(getRandomSpawnPoint().transform.position, currentSkill, currentHealth);

                if (agent !=null)
                {
                    agent.gameObject.SetActive(true);
                    currentUnitCount++;
                }
                else
                {
                    i--;
                }
                yield return new WaitForSeconds(rateOfSpawn);

            }

            yield return new WaitUntil(() => currentUnitCount == 0);

            if(currentWaveCount%2 ==0)
            {
                activeUnitCount += activeIncrementer;
                currentHealth += 1;
                currentSkill += 0.1f;
            }


        }
        Debug.Log("waveOver");

        // Wave Over
        if(activeDoor !=null)
        {
            activeDoor.activateForPlayer();
        }

        if(currentStage == 1)
        {
            tutorialText.gameObject.SetActive(true);
            tutorialText.text = "Room 1 cleared. Move to next room";
            yield return new WaitForSeconds(1);
            tutorialText.gameObject.SetActive(false);
            startStage2();
        }
        else if(currentStage == 2)
        {
            tutorialText.text = "Room 2 cleared. Move to next room";
            yield return new WaitForSeconds(1);
            tutorialText.gameObject.SetActive(false);
            startStage3();
        }
        else
        {
            tutorialText.gameObject.SetActive(true);
            tutorialText.text = "Mision Complete. Thanks for playing this Demo. Press Here to restart.";
        }

        

    }

    private SpawnPoint getRandomSpawnPoint()
    {
      return activeSpawnPoints[  Random.Range(0, activeSpawnPoints.Length) ];
    }

    private AgentController getRandomAgent(Vector3 pos,float skill,float health)
    {
        //return m_enemyPool.getDroid(pos, health, skill);
        if (Random.value > 0.5f)
        {
            //Debug.Log("droid");
            return m_enemyPool.getDroid(pos, (int)health, skill);
        }
        else
        {
            //Debug.Log("drone");
            return m_enemyPool.getDrone(pos, (float)health/2, skill);
        }
    }

    #endregion
}
