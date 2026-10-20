
using System;
using Unity.AI.Navigation;
using UnityEngine;


public class MapGeneration : MonoBehaviour
{
    [Header("Map Dimensions")]
    [SerializeField] private int height;
    [SerializeField] private int width;
    [SerializeField] private float tileSize = 10;
    [SerializeField] private int maxTiles;
    [Range(0f,1f)] private float obstacleChance;

    [Header("Prefabs")]
    [SerializeField] private GameObject tile;
    [SerializeField] private GameObject wallTile;
    [SerializeField] private GameObject cornerTile;
    [SerializeField] private GameObject roomTile;
    [SerializeField] private GameObject hallTile;
    [SerializeField] private GameObject alcoveTile;
    [SerializeField] private GameObject obstacleTile;

    [Header("Components")] 
    [SerializeField] private NavMeshSurface navMesh;

    private int [,] mapMatrix;

    void Awake()
    {
        navMesh = GetComponent<NavMeshSurface>();

        maxTiles = (width * height) - UnityEngine.Random.Range(0,Math.Min(width,height));
        GenerateMapData();
        BuildMap();

        navMesh.BuildNavMesh();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void GenerateMapData()
    {
        //generate the procedural map data
        
        mapMatrix = new int[width,height];

        int currentX = width/2;
        int currentZ = height/2;
        int tilesCreated = 0;

        while(tilesCreated <= maxTiles)
        {
            //if the tile lands on a spot that was already generated or is on the edge of the matrix we skip it
            if(mapMatrix[currentX,currentZ] == 1 || currentX == 0 || currentX == width || currentZ == 0 || currentZ == height){
                if(currentX == 0)
                    currentX ++;
                else if(currentX == width)
                    currentX --;
                if(currentZ == 0)
                    currentZ ++;
                else if(currentZ == height)
                    currentZ --;

                continue;
            }
            mapMatrix[currentX,currentZ] = 1;

            //randomly go in a way
            //0 up
            //1 down
            //2 right
            //3 left

            int randomMove = UnityEngine.Random.Range(0,4);

            switch (randomMove)
            {
                case 0:
                    currentZ ++;
                    break;
                case 1:
                    currentZ --;
                    break;
                case 2:
                    currentX ++;
                    break;
                case 3:
                    currentX --;
                    break;
                
            }

            tilesCreated ++;
        }

        //all tiles created, pad with wall tiles (2)
        for(int x = 1; x < width - 1; x++)
        {
            for(int z = 1; z < height -1; z++)
            {
                if((mapMatrix[x-1,z] == 1 || mapMatrix[x, z-1] == 1 || mapMatrix[x+1,z] == 1 || mapMatrix[x,z+1] == 1) && mapMatrix[x,z] == 0)
                    mapMatrix[x,z] = 2;
            }
        }
        
    }

    private int DetermineTileType(int x, int z)
    {
        //0 = void
        //1 = tile
        //2 = wall (1 neighbour)
        //3 = corner (2 adiacent neighbours)
        //4 = hall (2 opposite neighbour)
        //5 = room ()
        //6 alcove

        int tileType = 0;
        return tileType;
    }

    public void BuildMap()
    {
        
    }
}
