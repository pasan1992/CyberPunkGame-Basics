using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentGenarator : MonoBehaviour
{
    //Road Obsticle.
    public GameObject[] RoadObsticles;
    TileManager tileManager;

    private float spawnInterval = 1.6f;
    private float timeSinceLastSpawn;

    private void genarateRoadObsticle()
    {
        GameObject obsticle = GameObject.Instantiate(RoadObsticles[Random.Range(0, RoadObsticles.Length - 1)]);
        
        if(Random.value > 0.5)
        {
            obsticle.transform.position = this.transform.position + new Vector3(-7.5f, 0, 0);
        }
        else
        {
            obsticle.transform.position = this.transform.position + new Vector3(-2.5f, 0, 0);
        }

        obsticle.GetComponent<MovingTile>().setParameters(tileManager.Block_Speed/2, tileManager.Tile_Max_Distance, tileManager.movingDirection);
        tileManager.AddToMovingItemList(obsticle.GetComponent<MovingTile>());
    }

    private void calculateNextSpawn()
    {
        genarateRoadObsticle();
    }

    public void Start()
    {
        tileManager = this.GetComponent<TileManager>();
    }

    public void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if(timeSinceLastSpawn >= spawnInterval)
        {
            timeSinceLastSpawn = 0;

            // SpawnLogic
            calculateNextSpawn();
        }
    }
}
