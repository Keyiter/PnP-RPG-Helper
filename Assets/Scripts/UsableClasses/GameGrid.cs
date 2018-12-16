using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid {

    public enum side { x, y, z }
    protected Vector3Int size;
    protected Node[,,] grid;

    public GameGrid(Vector3Int _dim) {
        size = new Vector3Int(_dim.x, _dim.y, _dim.z);

        grid = new Node[size.y, size.x, size.z];

        for (int y = 0; y < size.y; y++)
            for (int x = 0; x < size.x; x++)
                for (int z = 0; z < size.z; z++) {
                    grid[y, x, z] = new Node(
                        Node.worldObject.empty,
                        Node.terrainType.walkable,
                        new Vector3Int(x, y, z),
                        Node.orientation.north

                    );

                }
    }

    public Node getGridNode(int x, int y, int z) {
        return grid[y, x, z];
    }

    public int MaxSize(side _side) {
        switch (_side) {
            case side.x:
                return size.x;
            case side.y:
                return size.y;
            case side.z:
                return size.z;
        }
        return -1;
    }
    public Node.worldObject getThingInPoint(Vector3 _point, float _currFloor = -1) {
        
            int x = (int)Mathf.Floor(_point.x);
            int y;
            if (_currFloor == -1)
                y = (int)Mathf.Floor(_point.y);
            else
                y = (int)_currFloor;
            int z = (int)Mathf.Floor(_point.z);

            return grid[y, x, z].placedThing;
        
    }

    public GameObject getObjectInPoint(Vector3 _point, float _currFloor = -1) {
        int x = (int)Mathf.Floor(_point.x);
        int y;
        if (_currFloor == -1)
            y = (int)Mathf.Floor(_point.y);
        else
            y = (int)_currFloor;
        int z = (int)Mathf.Floor(_point.z);

        return grid[y, x, z].placedObject;
    }

    public void placeObjectInPoint(Vector3 _pos, Node.worldObject _thing, GameObject _obj, byte _txt = 0, bool isLoading = false, Node.orientation _dir = Node.orientation.north) {
        int x = (int)Mathf.Floor(_pos.x);
        int y = (int)Mathf.Floor(_pos.y);
        int z = (int)Mathf.Floor(_pos.z);

        grid[y, x, z].placedThing = _thing;
        grid[y, x, z].placedObject = _obj;
        grid[y, x, z].texture = _txt;

        if (_thing == Node.worldObject.ground) {
            grid[y, x, z].walkable = Node.terrainType.walkable;
        } else if (_thing == Node.worldObject.stairsUp) {
            grid[y, x, z].walkable = Node.terrainType.difficult;
            removeObjectInPoint(new Vector3(x, y + 1, z),y+1);
            placeObjectInPoint(new Vector3(x, y + 1, z), Node.worldObject.stairsDown, null);
        } else if (_thing == Node.worldObject.stairsDown) {
            grid[y, x, z].walkable = Node.terrainType.difficult;
        } else grid[y, x, z].walkable = Node.terrainType.notwalkable;

        if (_thing != Node.worldObject.stairsDown) {
            if (isLoading) {
                grid[y, x, z].direction = _dir;
            } else {
                if (_obj.transform.rotation.eulerAngles.y < 2) {
                    grid[y, x, z].direction = Node.orientation.north;
                } else if (_obj.transform.rotation.eulerAngles.y < 92) {
                    grid[y, x, z].direction = Node.orientation.east;
                } else if (_obj.transform.rotation.eulerAngles.y < 182) {
                    grid[y, x, z].direction = Node.orientation.south;
                } else {
                    grid[y, x, z].direction = Node.orientation.west;
                }
            }
        }

    }

    public void removeObjectInPoint(Vector3 _point,int _floor) {
        int x = (int)Mathf.Floor(_point.x);
        int y = _floor;
        int z = (int)Mathf.Floor(_point.z);

        if (grid[y, x, z].placedThing == Node.worldObject.stairsDown) {
            grid[y, x, z].placedThing = Node.worldObject.empty;
            removeObjectInPoint(new Vector3(x, y - 1, z),y-1);
        } else if (grid[y, x, z].placedThing == Node.worldObject.stairsUp) {
            grid[y, x, z].placedThing = Node.worldObject.empty;
            removeObjectInPoint(new Vector3(x, y + 1, z),y+1);
        }

            grid[y, x, z].placedThing = Node.worldObject.empty;
        grid[y, x, z].walkable = Node.terrainType.notwalkable;
        grid[y, x, z].placedObject = null;
    }

}
