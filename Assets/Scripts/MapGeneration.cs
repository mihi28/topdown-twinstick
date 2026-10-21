
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
    [SerializeField, Range(0f, 1f)] private float obstacleChance;

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
        maxTiles = (int)(width * height * 0.7f);
        GenerateMapData();
        //PrintMatrixToConsole();
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
        int mapWidth = width + 2;
        int mapHeight = height + 2; // adding +2 for padding
        mapMatrix = new MapNode[mapWidth, mapHeight]; 
        for(int x = 1; x < mapWidth-1; x++)
        {
            for(int z = 1; z < mapHeight-1; z++)
            {
                MapNode newMapNode = new MapNode();

                newMapNode.x = x;
                newMapNode.z = z;
                newMapNode.tileType = 0;

                mapMatrix[x,z] = newMapNode;
            }
        }

        //linking neighbours
        for (int x = 1; x < mapWidth-1; x++)
        {
            for (int z = 1; z < mapHeight-1; z++)
            {
                MapNode mapNode = mapMatrix[x, z];

                mapNode.up = mapMatrix[x, z + 1];
                mapNode.down = mapMatrix[x, z - 1];
                mapNode.right = mapMatrix[x + 1, z];
                mapNode.left = mapMatrix[x - 1, z];
            }
        }

        //iterating through the matrix to randomly generate floor plan
        int currentX = mapWidth/2;
        int currentZ = mapHeight/2;
        int tilesCreated = 0;

        while(tilesCreated <= maxTiles)
        {
            //if the tile lands on a spot that was already generated or is on the edge of the matrix we skip it
            if (currentX < 1) currentX = 1;
            if (currentX > mapWidth - 2) currentX = mapWidth - 2;
            if (currentZ < 1) currentZ = 1;
            if (currentZ > mapHeight - 2) currentZ = mapHeight - 2;

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
        }

        //generate corners, hallways, rooms
        for(int x = 1; x<mapWidth - 1; x++)
        {
            for(int z = 1; z<mapHeight-1; z++)
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

        //offsets so the map centre is at (0,0,,0)
        float offsetX = ((width + 2) * tileSize) / 2f;
        float offsetZ = ((height + 2) * tileSize) / 2f;

        for(int x = 0; x < width + 2; x++)
        {
            for(int z = 0; z < height + 2; z++)
            {
                Vector3 pos = new Vector3(x*tileSize - offsetX, 0 , z * tileSize - offsetZ);

                if(mapMatrix[x,z] != null){

                    switch(mapMatrix[x, z].tileType)
                    {
                        case 1:
                            Instantiate(tile,pos,Quaternion.identity);
                            break;
                        case 2:
                            //walls, rotate according to which neighbour the tile does NOT have
                            //no neighbour down stays the same
                            //no left rotates 90
                            //no up rotates 180
                            //no right rotates 270
                            if(!mapMatrix[x,z].hasDown())
                                Instantiate(wallTile,pos,Quaternion.identity,transform);
                            else if(!mapMatrix[x,z].hasLeft())
                                Instantiate(wallTile,pos,Quaternion.Euler(0,90,0),transform);
                            else if(!mapMatrix[x,z].hasUp())
                                Instantiate(wallTile,pos,Quaternion.Euler(0,180,0),transform);
                            else if(!mapMatrix[x,z].hasRight())
                                Instantiate(wallTile,pos,Quaternion.Euler(0,270,0),transform);
                            break;
                        case 3:
                            //corners, rotate according to which neighbours it DOES have
                            //up and right stays the same
                            //right and down 90
                            //down and left 180
                            //left and up 270
                            if(mapMatrix[x,z].hasUp() && mapMatrix[x,z].hasRight())
                                Instantiate(cornerTile,pos,Quaternion.identity,transform);
                            else if(mapMatrix[x,z].hasRight() && mapMatrix[x,z].hasDown())
                                Instantiate(cornerTile,pos,Quaternion.Euler(0,90,0),transform);
                            else if(mapMatrix[x,z].hasDown() && mapMatrix[x,z].hasLeft())
                                Instantiate(cornerTile,pos,Quaternion.Euler(0,180,0),transform);
                            else if(mapMatrix[x,z].hasLeft() && mapMatrix[x,z].hasUp())
                                Instantiate(cornerTile,pos,Quaternion.Euler(0,270,0),transform);
                            break;
                        case 4:
                            //hallways, rotate according to which neighbours it DOES have
                            //left and right stays the same
                            //up and down 90
                            if(mapMatrix[x,z].hasLeft() && mapMatrix[x,z].hasRight())
                                Instantiate(hallTile,pos,Quaternion.identity);
                            else if(mapMatrix[x,z].hasUp() && mapMatrix[x,z].hasDown())
                                Instantiate(hallTile,pos,Quaternion.Euler(0,90,0),transform);
                            break;
                        case 5:
                            //rooms, rotate according to which neighbour it has
                            // up same
                            //right 90
                            //down 180
                            //left 270
                            if(mapMatrix[x,z].hasUp())
                                Instantiate(roomTile,pos,Quaternion.identity);
                            else if(mapMatrix[x,z].hasRight())
                                Instantiate(roomTile,pos,Quaternion.Euler(0,90,0),transform);
                            else if(mapMatrix[x,z].hasDown())
                                Instantiate(roomTile,pos,Quaternion.Euler(0,180,0),transform);
                            else if(mapMatrix[x,z].hasLeft())
                                Instantiate(roomTile,pos,Quaternion.Euler(0,270,0),transform);
                            break;
                    }

                if(mapMatrix[x,z].tileType != 0){
                    float randomObstacleRoll = UnityEngine.Random.Range(0f,1f);
                    if(randomObstacleRoll < obstacleChance)
                        {
                            Instantiate(obstacle,pos,Quaternion.identity,transform);
                        }
                    }
                }
            }
        }
    }
    /*private void PrintMatrixToConsole()
    {
        // Use a StringBuilder because it is much faster than adding strings together
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        // We loop Z backward so the "top" of the map prints at the top of the console
        for (int z = height + 1; z >= 0; z--)
        {
            for (int x = 0; x < width + 1; x++)
            {
                MapNode node = mapMatrix[x, z];
                
                if (node == null)
                {
                    sb.Append("- "); // Print a dash if the node doesn't exist (padding)
                }
                else
                {
                    sb.Append(node.tileType + " "); // Print the raw number
                }
            }
            sb.AppendLine(); // Start a new line for the next row
        }

        Debug.Log("Map Matrix Data:\n" + sb.ToString());
    }*/
}
