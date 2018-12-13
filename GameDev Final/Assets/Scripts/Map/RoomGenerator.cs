using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGenerator : MonoBehaviour {

    public Tilemap wallsMap;
    public Tilemap groundMap;
    public Tilemap featuresMap;
    public RuleTile wallRuleTile;
    public Tile baseGroundTile;
    public Tile[] wallTiles; //0 vertical left, 1 vertical right, 2 horizontal bottom, 3 horizontal top
    public Tile[] cornerTiles; // top left, top right, bottom left, bottom right
    public Tile[] doorCapTiles; //0 vertical wall above, 1 is vertical wall below, 2 horizontal wall left, 3 horizontal wall right;

    public GameObject doorPrefab;
    public GameObject chestPrefab;
    // Use this for initialization
    void Start () {
        //Generate(5, 5, 10, 6);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Generate(MapGenerator.Room r)
    {
        int x = (int)r.outerRect.x;
        int y = (int)r.outerRect.y;
        int w = (int)r.outerRect.width-1;
        int h = (int)r.outerRect.height-1;
        for (int i = x; i < x + w; i++)
        {
            for (int j = y; j < y + h; j++)
            {
                groundMap.SetTile(new Vector3Int(i, j, 0), baseGroundTile);
            }
        }

        /*
        for (int j = y; j < y + h; j++)
        {
            
            wallsMap.SetTile(new Vector3Int(x, j, 0), wallTiles[0]);
            wallsMap.SetTile(new Vector3Int(x + w, j, 0), wallTiles[1]);
        }
        for (int i = x; i < x + w; i++)
        {
            wallsMap.SetTile(new Vector3Int(i, y, 0), wallTiles[2]);
            wallsMap.SetTile(new Vector3Int(i, y + h, 0), wallTiles[3]);
        }

        wallsMap.SetTile(new Vector3Int(x, y, 0), cornerTiles[2]);
        wallsMap.SetTile(new Vector3Int(x + w, y, 0), cornerTiles[3]);
        wallsMap.SetTile(new Vector3Int(x, y + h, 0), cornerTiles[0]);
        wallsMap.SetTile(new Vector3Int(x + w, y + h, 0), cornerTiles[1]);

        */
        for (int j = y; j < y + h; j++)
        {

            wallsMap.SetTile(new Vector3Int(x, j, 0), wallRuleTile);
            wallsMap.SetTile(new Vector3Int(x + w, j, 0), wallRuleTile);
        }
        for (int i = x; i < x + w; i++)
        {
            wallsMap.SetTile(new Vector3Int(i, y, 0), wallRuleTile);
            wallsMap.SetTile(new Vector3Int(i, y + h, 0), wallRuleTile);
        }

        wallsMap.SetTile(new Vector3Int(x, y, 0), wallRuleTile);
        wallsMap.SetTile(new Vector3Int(x + w, y, 0), wallRuleTile);
        wallsMap.SetTile(new Vector3Int(x, y + h, 0), wallRuleTile);
        wallsMap.SetTile(new Vector3Int(x + w, y + h, 0), wallRuleTile);
        foreach (MapGenerator.Connection connect in r.confirmedConnections)
        {
            wallsMap.SetTile(connect.connectTile, null);
            GameObject d = Instantiate(doorPrefab, new Vector3(connect.connectTile.x+0.5f, connect.connectTile.y+0.5f), Quaternion.identity);
            if(connect.endTile.x!= connect.startTile.x) // horizontal connection, measn door is in a vertical wall
            {
                //wallsMap.SetTile(new Vector3Int(connect.connectTile.x, connect.connectTile.y + 1, 0), doorCapTiles[0]);
               // wallsMap.SetTile(new Vector3Int(connect.connectTile.x, connect.connectTile.y - 1, 0), doorCapTiles[1]);
            }
            else //door is in horizontal wall
            {

            }
            
        }
    }
    public void GeneratePaths(MapGenerator m)
    {
        for(int x = 0; x < m.mapWidth; x++)
        {
            for(int y = 0; y< m.mapHeight; y++)
            {
                if (m.mergedMap[x, y] == MapGenerator.TileType.MERGEDPATH)
                {
                    for(int i =-1; i <2; i++)
                    {
                        for (int j = -1; j <2; j++)
                        {
                            if(!(i==0 && j==0))
                            {
                                if (x + i >= 0 && y + j >= 0)
                                {
                                    if (m.mergedMap[x + i, y + j] == MapGenerator.TileType.NONE && !MapGenerator.IsOccupied(wallsMap, x + i, y + j))
                                    {
                                        wallsMap.SetTile(new Vector3Int(x + i, y + j, 0), wallRuleTile);
                                    }
                                }
                                if(x+i<0 && !MapGenerator.IsOccupied(wallsMap, x + i, y + j))
                                {
                                    wallsMap.SetTile(new Vector3Int(x + i, y + j, 0), wallRuleTile);
                                }
                                if (y+ j < 0 && !MapGenerator.IsOccupied(wallsMap, x + i, y + j))
                                {
                                    wallsMap.SetTile(new Vector3Int(x + i, y + j, 0), wallRuleTile);
                                }

                            }
                        }
                    }
                }
            }
        }
    }
    public void Generate(int x, int y, int w, int h)
    {
        for(int i = x; i < x+w; i++)
        {
            for(int j = y; j < y+h; j++)
            {
                groundMap.SetTile(new Vector3Int(i, j, 0), baseGroundTile);
            }
        }

        for(int j = y; j < y+h; j++)
        {
            wallsMap.SetTile(new Vector3Int(x, j, 0), wallTiles[0]);
            wallsMap.SetTile(new Vector3Int(x+w, j, 0), wallTiles[1]);
        }
        for (int i = x; i < x + w; i++)
        {
            wallsMap.SetTile(new Vector3Int(i, y, 0), wallTiles[2]);
            wallsMap.SetTile(new Vector3Int(i, y+h, 0), wallTiles[3]);
        }

        wallsMap.SetTile(new Vector3Int(x, y, 0), cornerTiles[2]);
        wallsMap.SetTile(new Vector3Int(x+w, y, 0), cornerTiles[3]);
        wallsMap.SetTile(new Vector3Int(x, y+h, 0), cornerTiles[0]);
        wallsMap.SetTile(new Vector3Int(x+w, y+h, 0), cornerTiles[1]);
    }
}
