public class MapNode
{
    public int x, z;
    public int tileType; //0 empty, 1 ground, 2 wall, 3 corner, 4 hallway, 5 room
    public bool isObstacle;

    public MapNode up;
    public MapNode down;
    public MapNode right;
    public MapNode left;  

    public MapNode()
    {
        x = 0;
        z = 0;
        tileType = 0;
        up = null;
        down = null;
        right = null;
        left = null;
        isObstacle = false;
    }

    public int noOfNeighbours()
    {
        int nr = 0;
        if(hasDown()) nr++;
        if(hasUp()) nr++;
        if(hasLeft()) nr++;
        if(hasRight()) nr++; 

        return nr;
    }

    public bool hasUp()
    {
        // Check if the node exists AND if it is not an empty void (0)
        return up != null && up.tileType != 0;
    }

    public bool hasDown()
    {
        return down != null && down.tileType != 0;
    }

    public bool hasLeft()
    {
        return left != null && left.tileType != 0;
    }
    
    public bool hasRight()
    {
        return right != null && right.tileType != 0;
    }
}