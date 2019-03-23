using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour {

	// Public Properties 
	public GameObject Tile_Prefab;
	public float Block_Speed = 2;
	public float Tile_Max_Distance = 30;
	public float tileLength =1;
	public Vector3 tileScale = new Vector3(1f,1f,1f);
    public Vector3 movingDirection;
	private float distanceLatestTileTravelled =0;
    List<MovingTile> tileList = new List<MovingTile>();
    void Start () 
	{
        SpawnInitialTiles();
    }
	
	void FixedUpdate () 
	{
		// Spawn a new tile
		if(distanceLatestTileTravelled >=tileLength)
		{
			distanceLatestTileTravelled = 0;
			SpawnTile(0);				
		}
		else
		{
			distanceLatestTileTravelled +=Time.deltaTime*Block_Speed;
		}

		// Update current tiles
        for(int i = tileList.Count - 1;i>=0;i--)
        {
            // Remove thiles that reached maximum distance
            if (!tileList[i].MoveTile())
            {
                MovingTile tile = tileList[i];
                tileList.Remove(tileList[i]);
                Destroy(tile.gameObject);
            }
        }
	}

    public void AddToMovingItemList(MovingTile movingObject)
    {
        tileList.Add(movingObject);
    }

	private void SpawnTile(float offset)
	{
        GameObject go = Instantiate(Tile_Prefab) as GameObject;
        MovingTile movingTilee = go.transform.GetComponent<MovingTile>();
        movingTilee.setParameters(Block_Speed,Tile_Max_Distance, movingDirection);
		go.transform.SetParent(transform);
		go.transform.localPosition = movingDirection*offset;
        movingTilee.setDistanceTravelled(offset);
		go.transform.localScale = tileScale;
        tileList.Add(movingTilee);
	}

    private void SpawnInitialTiles()
    {
        int count = (int)(Tile_Max_Distance / tileLength);
        for(int i =0;i<count;i++)
        {
            SpawnTile(tileLength * i);
        }
    }


}
