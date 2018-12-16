using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

    public enum worldObject { empty, ground, wall, door, stairsUp, stairsDown}
    public enum terrainType { walkable, notwalkable, difficult}
    public enum orientation { north, east, south, west}
    public worldObject placedThing;
    public terrainType walkable;
    public orientation direction;
    public Vector3Int worldSpacePosition;
    public GameObject placedObject;
    public byte texture;

    public Node(worldObject _thing,terrainType _walk, Vector3Int _worldPos, orientation _dir) {
        placedThing = _thing;
        walkable = _walk;
        worldSpacePosition = _worldPos;
        texture = 1;
        direction = _dir;
    }

    public Vector3 gridToWorldPosition() {
        if(placedThing == worldObject.ground)
            return new Vector3(worldSpacePosition.x+ 0.5f, worldSpacePosition.y, worldSpacePosition.z + 0.5f);
        else
            return new Vector3(worldSpacePosition.x + 0.5f, worldSpacePosition.y+ 0.5f, worldSpacePosition.z + 0.5f);
    }
}
