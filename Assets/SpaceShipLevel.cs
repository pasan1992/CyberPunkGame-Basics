using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipLevel : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Stage 1")]
    public SpawnPoint[] stage1SpawnPoints;
    public int Stage1TartingUnitCount;
    public int Stage1UnitCountIncrementer;
    public int Stage1WaveCount;

    [Header("Stage 2")]
    public SpawnPoint[] stage2SpawnPoints;
    public int Stage2TartingUnitCount;
    public int Stage2UnitCountIncrementer;
    public int Stage2WaveCount;

    [Header("Stage 3")]
    public SpawnPoint[] stage3SpawnPoints;
    public int Stage3TartingUnitCount;
    public int Stage3UnitCountIncrementer;
    public int Stage3WaveCount;


    private SpawnPoint[] activeSpawnPoints;
    private int activeUnitCount;
    private int activeIncrementer;
    private int activeWaveCount;

    private int currentWaveCount;
    private int currentEnemyCount;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
