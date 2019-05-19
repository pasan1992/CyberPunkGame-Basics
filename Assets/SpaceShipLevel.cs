using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using humanoid;

public class SpaceShipLevel : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Stage 1")]
    public SpawnPoint[] stage1SpawnPoints;
    public int Stage1StartingUnitCount;
    public int Stage1UnitCountIncrementer;
    public int Stage1WaveCount;
    public int Stage1Health;
    public float Stage1Skill;

    [Header("Stage 2")]
    public SpawnPoint[] stage2SpawnPoints;
    public int Stage2StartingUnitCount;
    public int Stage2UnitCountIncrementer;
    public int Stage2WaveCount;
    public int Stage2Health;
    public float Stage2Skill;

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

    private int currentWaveCount;
    private int currentUnitCount;
    private float currentSkill;
    private int currentHealth;

    private EnemyPool m_enemyPool;



    #region Initialize

    void Start()
    {
        m_enemyPool = FindObjectOfType<EnemyPool>();
        m_enemyPool.setPoolAgentOndestoryEvent(onUnityDestory);
        startStage1();
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
    }
    #endregion

    #region Commands


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
    }



    #endregion

    #region Utility

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
                yield return new WaitForSeconds(1);

            }

            yield return new WaitUntil(() => currentUnitCount == 0);

            activeUnitCount += activeIncrementer;
            currentHealth += 1;
            currentSkill += 0.1f;

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
            return m_enemyPool.getDroid(pos, health, skill);
        }
        else
        {
            //Debug.Log("drone");
            return m_enemyPool.getDrone(pos, health, skill);
        }
    }

    #endregion
}
