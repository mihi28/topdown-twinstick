
using System;
using Unity.AI.Navigation;
using Unity.Collections;
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
    [SerializeField] private GameObject obstacle;

    [Header("Components")] 
    [SerializeField] private NavMeshSurface navMesh;

    private MapNode[,] mapMatrix;

    void Awake()
    {
        navMesh = GetComponent<NavMeshSurface>();

        //maximum number of tiles is given by the maximum number inside the matrix minus 20%
        maxTiles = (int)(width * height * 0.8f);
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

        //fristly create a random drunken walk map with just floor
        
        //initializing matrix
        width += 1;
        height += 1; // adding +1 for padding
        mapMatrix = new MapNode[width, height]; 
        for(int x = 1; x < width-1; x++)
        {
            for(int z = 1; z < height-1; z++)
            {
                MapNode newMapNode = new MapNode();

                newMapNode.x = x;
                newMapNode.z = z;
                newMapNode.tileType = 0;

                mapMatrix[x,z] = newMapNode;
            }
        }

        //linking neighbours
        for (int x = 1; x < width-1; x++)
        {
            for (int z = 1; z < height-1; z++)
            {
                MapNode mapNode = mapMatrix[x, z];

                mapNode.up = mapMatrix[x, z + 1];
                mapNode.down = mapMatrix[x, z - 1];
                mapNode.right = mapMatrix[x + 1, z];
                mapNode.left = mapMatrix[x - 1, z];
            }
        }

        //iterating through the matrix to randomly generate floor plan
        int currentX = width/2;
        int currentZ = height/2;
        int tilesCreated = 0;

        while(tilesCreated <= maxTiles)
        {
            //if the tile lands on a spot that was already generated or is on the edge of the matrix we skip it
            if (currentX < 1) currentX = 1;
            if (currentX > width - 1) currentX = width - 2;
            if (currentZ < 1) currentZ = 1;
            if (currentZ > height - 1) currentZ = height - 2;

            //paint the floor ONLY if it's currently empty
            if(mapMatrix[currentX, currentZ].tileType == 0)
            {
                mapMatrix[currentX, currentZ].tileType = 1;
                tilesCreated++; 
            }

            //randomly go aorund the matrix
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

        //generate corners, hallways, rooms
        for(int x = 1; x<width - 1; x++)
        {
            for(int z = 1; z<height-1; z++)
            {
                DetermineTileType(mapMatrix[x,z]);
            }
        }
        
    }

    private void DetermineTileType(MapNode node)
    {
        //0 = void
        //1 = tile
        //2 = wall (3 neighbour)
        //3 = corner (2 adiacent neighbours)
        //4 = hall (2 opposite neighbour)
        //5 = room (1 neighb)

        if(node.tileType == 1)
        {
            if(node.noOfNeighbours() == 1)
                node.tileType = 5;
            else if(node.noOfNeighbours() == 2)
            {
                if((node.hasDown() && node.hasUp()) || (node.hasLeft() && node.hasRight()))
                    node.tileType = 4;
                else
                    node.tileType = 3;
            }
            else if(node.noOfNeighbours() == 3)
                node.tileType = 2;
        }

    }

    public void BuildMap()
    {
        //iterate through the map matrix, and rotate prefabs accordingly
        //rotating prefabs according to which neighbours they have
        //also generate obstacles here

        for(int x = 1; x < width - 1; x++)
        {
            for(int z = 1; z < height - 1; z++)
            {
                Vector3 pos = new Vector3(x*tileSize, 0 , z * tileSize);

                switch(mapMatrix[x, z].tileType)
                {
                    case 1:
                        Instantiate(tile,pos,Quaternion.identity);
                        break;
                    case 2:
                        //walls, rotate according to which neighbours the tile does NOT have
                        Instantiate(wallTile,pos,Quaternion.identity);
                        break;
                    case 3:
                        //corners, rotate according to which neighbours it DOES have
                        Instantiate(cornerTile,pos,Quaternion.identity);
                        break;
                    case 4:
                        //hallways, rotate according to which neighbours it DOES have
                        Instantiate(hallTile,pos,Quaternion.identity);
                        break;
                    case 5:
                        //rooms, rotate according to which neighbour it has
                        Instantiate(cornerTile,pos,Quaternion.identity);
                        break;
                }
            }
        }
    }

    [Header("Debug")]
    public bool showDebugMatrix = true;

    private void OnDrawGizmos()
    {
        // Don't draw if the game isn't running or the toggle is off
        if (!showDebugMatrix || mapMatrix == null) return;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                MapNode node = mapMatrix[x, z];
                if (node == null) continue;

                Vector3 pos = new Vector3(x * tileSize, 1, z * tileSize); // Lifted slightly so it doesn't clip into the floor

                // Assign a color based on the data type
                switch (node.tileType)
                {
                    case 0: Gizmos.color = new Color(0, 0, 0, 0.1f); break; // Void (Faint Black)
                    case 1: Gizmos.color = Color.white; break;  // Standard Tile
                    case 2: Gizmos.color = Color.red; break;    // Wall
                    case 3: Gizmos.color = Color.blue; break;   // Corner
                    case 4: Gizmos.color = Color.yellow; break; // Hallway
                    case 5: Gizmos.color = Color.green; break;  // Room
                }

                // Draw a holographic box slightly smaller than the tile size
                Gizmos.DrawCube(pos, new Vector3(tileSize * 0.9f, 0.1f, tileSize * 0.9f));
            }
        }
    }
}
