using UnityEngine;

public class MapNode
{
    public int x,z;
    public int tileType;//0 empty, 1 gournd, 2 wall, 3 corner, 4 hallway, 5 room, 6 alcove

    public MapNode north;
    public MapNode south;
    public MapNode east;
    public MapNode west;  
}
