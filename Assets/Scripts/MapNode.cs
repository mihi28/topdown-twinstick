
public class MapNode
{
    public int x,z;
    public int tileType;//0 empty, 1 gournd, 2 wall, 3 corner, 4 hallway, 5 room, 6 alcove

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
    }

    public int noOfNeighbours()
    {
        int nr = 0;
        if(up != null)
            nr++;
        if(down != null)
            nr++;
        if(left != null)
            nr++;
        if(right != null)
            nr++;

        return nr;
    }

    public bool hasUp()
    {
        return up != null ? true : false;
    }

    public bool hasDown()
    {
        return down != null ? true : false;
    }

    public bool hasLeft()
    {
        return left != null ? true : false;
    }
    
    public bool hasRight()
    {
        return up != null ? true : false;
    }
}
