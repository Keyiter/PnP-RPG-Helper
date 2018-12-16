using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingInterface : MonoBehaviour{

    public enum thingsToDo {select, place, fill }


    public Button wall;
    public Button ground;
    public Button door;
    public Button stairs;

    public Button remove;
    public Button fill;
    public Button select;

    protected thingsToDo action;
    protected Node.worldObject objects;
    protected BuildingController buildCtrl;

    private void Start() {
        buildCtrl = GameObject.FindWithTag("BuildingController").GetComponent<BuildingController>();
        wall.onClick.AddListener(WallPlacing);
        ground.onClick.AddListener(GroundPlacing);
        door.onClick.AddListener(DoorPlacing);
        stairs.onClick.AddListener(StairsPlacing);
        remove.onClick.AddListener(RemoveObject);
        fill.onClick.AddListener(toggleFill);
        select.onClick.AddListener(SelectObject);


    }


    protected void WallPlacing() {
        action = thingsToDo.place;
        objects = Node.worldObject.wall;
    }

    protected void GroundPlacing() {
        action = thingsToDo.place;
        objects = Node.worldObject.ground;
    }

    protected void DoorPlacing() {
        action = thingsToDo.place;
        objects = Node.worldObject.door;
    }
    protected void StairsPlacing() {
        action = thingsToDo.place;
        objects = Node.worldObject.stairsUp;
    }

    protected void RemoveObject() {
        buildCtrl.RemoveSelected();
    }

    protected void toggleFill() {
        if(action == thingsToDo.fill)
            action = thingsToDo.place;
        else 
            action = thingsToDo.fill;
        if(objects == Node.worldObject.empty) 
            objects = Node.worldObject.ground;
        
    }

    protected void SelectObject() {
        action = thingsToDo.select;
        objects = Node.worldObject.wall;
    }


    public thingsToDo currentAction() {
        return action;
    }

    public Node.worldObject currentObject() {
        return objects;
    }

}
