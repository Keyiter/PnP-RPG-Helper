using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour {

    //objects set in inspector
    public Transform gridParent;

    //control bools
    public bool isBuilding;

    //other scripts
    private GlobalController gc;
    private BuildingInterface buildInt;

    //place highlight
    private GameObject currentInstance;
    private MeshRenderer instanceRenderer;
    private List<GameObject> instanceArray;
    private List<Vector3Int> alreadyFilled;

    //control bools
    private bool canBePlaced;
    private bool canBeRemoved;
    private bool rotateArrows;
    private bool cameraRotating;
    private bool startDragSelect;
    private bool somethingSelected;
    //floor control
    private List<GameObject> floorParents;
    private int currentFloor;

    //grid
    private GameGrid worldGrid;

    //rotating help
    private Quaternion tempRot;
    private Vector3 rot;

    //old cursor position
    public Vector3Int cursorPoint;
    //public Vector3Int selectPoint;



    private void Start() {
        gc = GameObject.FindWithTag("GlobalController").GetComponent<GlobalController>();
        isBuilding = false;
        rotateArrows = false;
        cameraRotating = false;
        somethingSelected = false;
        Vector3Int size = gc.getSize();    
        currentFloor = 0;
        instanceArray = new List<GameObject>();
        alreadyFilled = new List<Vector3Int>();
        floorParents = new List<GameObject>();
        InstantiateGridObjects(size);
        buildInt = gameObject.GetComponent<BuildingInterface>();
        currentInstance = null;
        if (gc.isGameLoaded()) {
            FillGridWithSave();
        }else {
            worldGrid = new GameGrid(size);
        }
    }




    private void InstantiateGridObjects(Vector3Int _size) {
        int gridSizeX = Mathf.Clamp((int)Mathf.Ceil(_size.x / 10), 1, 100);
        int gridSizeZ = Mathf.Clamp((int)Mathf.Ceil(_size.z / 10), 1, 100);
        
        for (int x = 0; x < gridSizeX; x++)
            for (int z = 0; z < gridSizeZ; z++) {
               GameObject obj = Instantiate(Resources.Load("Prefabs/gridVisualisation") as GameObject, new Vector3((x * 10) + 5f, 0, (z * 10) + 5f), Quaternion.identity) as GameObject;
               obj.transform.parent = gridParent;
            }
        for (int y = 0; y < _size.y; y++) {
            floorParents.Add(new GameObject("Floor" + y));
            floorParents[y].SetActive(false);
        }
        floorParents[0].SetActive(true);
    }

    private void FillGridWithSave() {
        worldGrid = gc.getActualGrid();

        int sizex = worldGrid.MaxSize(GameGrid.side.x);
        int sizey = worldGrid.MaxSize(GameGrid.side.y);
        int sizez = worldGrid.MaxSize(GameGrid.side.z);

        for (int x = 0; x < sizex; x++)
            for (int y = 0; y < sizey; y++)
                for (int z = 0; z < sizez; z++) {
                    switch (worldGrid.getGridNode(x, y, z).placedThing) {
                        case Node.worldObject.wall:
                            currentInstance = Instantiate(Resources.Load("Objects/Wall") as GameObject, worldGrid.getGridNode(x,y,z).gridToWorldPosition(), Quaternion.identity);
                            instanceRenderer = currentInstance.GetComponent<MeshRenderer>();
                            break;
                        case Node.worldObject.ground:
                            currentInstance = Instantiate(Resources.Load("Objects/Ground") as GameObject, worldGrid.getGridNode(x, y, z).gridToWorldPosition(), Quaternion.identity);
                            instanceRenderer = currentInstance.GetComponent<MeshRenderer>();
                            break;
                    }
                    RotateToPoint(worldGrid.getGridNode(x, y, z).direction);
                    instanceRenderer.material.color = new Color(1, 1, 1, 1);
                    currentInstance.GetComponent<Collider>().enabled = true;
                    currentInstance.transform.parent = floorParents[y].transform;
                    currentInstance = null;

                }
    }

    private void RotateToPoint(Node.orientation _dir) {
        switch (_dir) {
            case Node.orientation.east:
                currentInstance.transform.Rotate(0, 90, 0);
                break;
            case Node.orientation.south:
                currentInstance.transform.Rotate(0, 180, 0);
                break;
            case Node.orientation.west:
                currentInstance.transform.Rotate(0, 270, 0);
                break;
        }
    }

    public void floorUp() {      
        if (worldGrid.MaxSize(GameGrid.side.y) - 1 != currentFloor) {

            gridParent.position += new Vector3(0, 1, 0);
            currentFloor++;
            floorParents[currentFloor].SetActive(true);
        }
    }

    public void floorDown() {
        if (0 != currentFloor) {
            floorParents[currentFloor].SetActive(false);
            gridParent.position -= new Vector3(0, 1, 0);
            currentFloor--;
        }
    }


    public void PlaceAdjust(RaycastHit hit, Node.worldObject obj) {
        currentInstance.transform.position = new Vector3(
                            Mathf.Clamp(Mathf.Floor(hit.point.x) + 0.5f, 0, worldGrid.MaxSize(GameGrid.side.x) + 0.5f),
                            currentFloor,
                            Mathf.Clamp(Mathf.Floor(hit.point.z) + 0.5f, 0, worldGrid.MaxSize(GameGrid.side.z) + 0.5f)
                        );

        if (worldGrid.getThingInPoint(currentInstance.transform.position,currentFloor) == Node.worldObject.empty || (buildInt.currentObject() != Node.worldObject.wall && buildInt.currentObject() != Node.worldObject.ground &&worldGrid.getThingInPoint(currentInstance.transform.position) == Node.worldObject.ground )) {
            if (buildInt.currentObject() == Node.worldObject.stairsUp) {
                if (currentFloor < worldGrid.MaxSize(GameGrid.side.y)-1 ) {
                    Debug.Log("cf = " + currentFloor + " mf = " + (worldGrid.MaxSize(GameGrid.side.y) - 1));

                    instanceRenderer.material.color = new Color(0, 0.5f, 0, 0.5f);
                    canBePlaced = true;
                }else {
                    instanceRenderer.material.color = new Color(0.5f, 0, 0, 0.5f);
                    canBePlaced = false;
                }
            } else {
                instanceRenderer.material.color = new Color(0, 0.5f, 0, 0.5f);
                canBePlaced = true;
            }
        } else {
            currentInstance.transform.position += new Vector3(0, 1, 0);
            instanceRenderer.material.color = new Color(0.5f, 0, 0, 0.5f);
            canBePlaced = false;
        }
    }

    public void RotateObject(RaycastHit hit) {
        if (!rotateArrows) {
            rotateArrows = true;
            tempRot = currentInstance.transform.rotation;
        }
        tempRot *= Quaternion.Euler(new Vector3(0, (
                                       (hit.point.x > currentInstance.transform.position.x ? -Input.GetAxis("Mouse Y") : Input.GetAxis("Mouse Y")) +
                                       (hit.point.z > currentInstance.transform.position.z ? Input.GetAxis("Mouse X") : -Input.GetAxis("Mouse X")))
                                       * 10, 0)
                               );
        rot = tempRot.eulerAngles;
        rot.x = Mathf.Round(rot.x / 90) * 90;
        rot.y = Mathf.Round(rot.y / 90) * 90;
        rot.z = Mathf.Round(rot.z / 90) * 90;
        currentInstance.transform.eulerAngles = rot;
    }

    public void DoorsCantFly() {
        if(buildInt.currentObject() != Node.worldObject.ground && buildInt.currentObject() != Node.worldObject.wall)
             if (worldGrid.getThingInPoint(currentInstance.transform.position) == Node.worldObject.empty) {
                 GameObject temp = Instantiate(Resources.Load("Objects/Ground") as GameObject, currentInstance.transform.position, currentInstance.transform.rotation);
                 temp.transform.parent = currentInstance.transform;
             } else {
                worldGrid.getObjectInPoint(currentInstance.transform.position).transform.parent = currentInstance.transform;

            }
    }

    public void PlaceObject() {
        instanceRenderer.material.color = new Color(1, 1, 1, 1);
        currentInstance.GetComponent<Collider>().enabled = true;
        currentInstance.transform.parent = floorParents[currentFloor].transform;
        canBePlaced = false;
        currentInstance = null;
    }

    


    public void removeObjectinPoint(Vector3 hit) {
        Destroy(worldGrid.getObjectInPoint(hit, currentFloor));
        worldGrid.removeObjectInPoint(hit,currentFloor);
        instanceRenderer = null;
        currentInstance = null;
    }


    public GameGrid getWorldGrid() {
        return worldGrid;
    }

    private bool ChangedCursorPosition(Vector3 _point) {
        Vector3Int pointToCompare = new Vector3Int((int)_point.x, (int)_point.y, (int)_point.z);
        if (pointToCompare != cursorPoint) {
            cursorPoint = pointToCompare;
            return true;
        } else
            return false;       
    }

    private GameObject InstantiateObjects(Vector3 point ) {
        GameObject ret = null;
            switch (buildInt.currentObject()) {
                case Node.worldObject.wall:
                    ret = Instantiate(Resources.Load("Objects/Wall") as GameObject, point, Quaternion.identity);                  
                    break;
                case Node.worldObject.ground:
                    ret = Instantiate(Resources.Load("Objects/Ground") as GameObject, point, Quaternion.identity);
                    break;
                case Node.worldObject.door:
                    ret = Instantiate(Resources.Load("Objects/Door") as GameObject, point, Quaternion.identity);        
                    break;
                case Node.worldObject.stairsUp:
                    ret = Instantiate(Resources.Load("Objects/Stairs") as GameObject, point, Quaternion.identity);
                    break;
            }
        return ret;
        
    }

    private void DestroyFillObjects() {
        if (instanceArray.Count > 0) {
            foreach (GameObject obj in instanceArray) {
                Destroy(obj);    
            }
            instanceArray = new List<GameObject>();
            alreadyFilled = new List<Vector3Int>();
        }
    }

    private void InitiateFillOperation(Vector3 _point) {
        DestroyFillObjects();
         Vector3Int point = new Vector3Int((int)_point.x, currentFloor, (int)_point.z);
        MakeRecursiveFill(point);
    }

    private void MakeRecursiveFill(Vector3Int _point) {
        if (_point.x < worldGrid.MaxSize(GameGrid.side.x) && _point.z < worldGrid.MaxSize(GameGrid.side.z) && _point.x >=0 && _point.z >=0) {
            if (worldGrid.getThingInPoint(_point,currentFloor) == Node.worldObject.empty) {
                bool filled = false; ;
                foreach (Vector3Int pos in alreadyFilled) {
                    if (pos == _point) {
                        filled = true;
                        break;
                    }
                }
                if (!filled) {
                    instanceArray.Add(InstantiateObjects(new Vector3 (_point.x + 0.5f, _point.y, _point.z+0.5f)));
                    instanceArray[instanceArray.Count - 1].GetComponent<MeshRenderer>().material.color = new Color(0, 0.5f, 0, 0.5f);
                    alreadyFilled.Add(_point);
                    MakeRecursiveFill(new Vector3Int(_point.x - 1, _point.y, _point.z));
                    MakeRecursiveFill(new Vector3Int(_point.x + 1, _point.y, _point.z));
                    MakeRecursiveFill(new Vector3Int(_point.x, _point.y, _point.z - 1));
                    MakeRecursiveFill(new Vector3Int(_point.x, _point.y, _point.z + 1));
                }
            }
        }
    }

    private void PlaceFillObjects() {
        foreach(GameObject obj in instanceArray) {
            worldGrid.placeObjectInPoint(obj.transform.position, buildInt.currentObject(), obj);
            obj.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 1);
            obj.GetComponent<Collider>().enabled = true;
            obj.transform.parent = floorParents[currentFloor].transform;
        }
        instanceArray = new List<GameObject>();
        alreadyFilled = new List<Vector3Int>();
    }

    private bool AddToSelected(Vector3 _point,bool drag = false) {
        if(worldGrid.getThingInPoint(_point, currentFloor) != Node.worldObject.empty && worldGrid.getThingInPoint(_point, currentFloor) != Node.worldObject.stairsDown) {
            if (!drag  && !Input.GetKey(KeyCode.LeftControl)) {
                foreach (GameObject obj in instanceArray)
                    obj.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 1);
                instanceArray = new List<GameObject>();
                alreadyFilled = new List<Vector3Int>();
            }
            Vector3Int point = new Vector3Int((int)_point.x, (int)_point.y, (int)_point.z);
            foreach (Vector3Int pos in alreadyFilled)
                if (pos == point) return false;
            instanceArray.Add(worldGrid.getObjectInPoint(_point, currentFloor));
            alreadyFilled.Add(point);
        }
        somethingSelected = true;
        return true;
    }

    public void DragSelect(Vector3 _point) {
        if (!startDragSelect) {
            startDragSelect = true;
            cursorPoint = new Vector3Int((int)_point.x, (int)_point.y, (int)_point.z);
            if (!Input.GetKey(KeyCode.LeftControl)) {
                foreach (GameObject obj in instanceArray)
                    obj.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, 1);
                instanceArray = new List<GameObject>();
                alreadyFilled = new List<Vector3Int>();
            }
           
        }

        for (int x = (cursorPoint.x < (int)_point.x ? cursorPoint.x : (int)_point.x); x <= (cursorPoint.x < (int)_point.x ? (int)_point.x : cursorPoint.x); x++)
            for (int z = (cursorPoint.z < (int)_point.z ? cursorPoint.z : (int)_point.z); z <= (cursorPoint.z < (int)_point.z ? (int)_point.z : cursorPoint.z); z++) {
                HighlightThisObject(new Vector3Int(x, currentFloor, z), new Color(0, 0, 1, 1));
            }
    }

    public void IterateThruSelection(Vector3 _point) {
        for (int x = (cursorPoint.x < (int)_point.x ? cursorPoint.x : (int)_point.x); x <= (cursorPoint.x < (int)_point.x ? (int)_point.x : cursorPoint.x); x++)
            for (int z = (cursorPoint.z < (int)_point.z ? cursorPoint.z : (int)_point.z); z <= (cursorPoint.z < (int)_point.z ? (int)_point.z : cursorPoint.z); z++) {
                AddToSelected(new Vector3(x,currentFloor,z),true);
            }
    }




    private void HighlightSelectedObjects() {
            foreach(GameObject obj in instanceArray) 
                obj.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 1, 1);
               
    }

    private void HighlightThisObject(Vector3 _point, Color _color) {
        if (worldGrid.getThingInPoint(_point, currentFloor) != Node.worldObject.empty && worldGrid.getThingInPoint(_point, currentFloor) != Node.worldObject.stairsDown) {
            worldGrid.getObjectInPoint(_point, currentFloor).GetComponent<MeshRenderer>().material.color = _color;
        }
    }

    private void DeHighlightSelectedObjects() {
        for(int x = 0; x < worldGrid.MaxSize(GameGrid.side.x); x++)
            for(int z = 0; z< worldGrid.MaxSize(GameGrid.side.z); z++) {
                if (worldGrid.getThingInPoint(new Vector3(x,0,z), currentFloor) != Node.worldObject.empty  && worldGrid.getThingInPoint(new Vector3(x, 0, z), currentFloor) != Node.worldObject.stairsDown) {
                    worldGrid.getObjectInPoint(new Vector3(x, 0, z), currentFloor).GetComponent<MeshRenderer>().material.color = new Color(1,1,1,1);
                }
            }
        
    }

    public void RemoveSelected() {
        if (instanceArray.Count > 0 && somethingSelected) {
            foreach (Vector3Int pos in alreadyFilled)
                removeObjectinPoint(new Vector3(pos.x, pos.y, pos.z));
            instanceArray = new List<GameObject>();
            alreadyFilled = new List<Vector3Int>();
            somethingSelected = false;
        }
    }


    private void FixedUpdate() {
        if (Input.GetMouseButton(2))
            cameraRotating = true;
        else
            cameraRotating = false;

        if (isBuilding && !cameraRotating) {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            DeHighlightSelectedObjects();
            if (Input.GetKey(KeyCode.Delete))
                RemoveSelected();
            switch (buildInt.currentAction()) {
                case BuildingInterface.thingsToDo.place:
                    if (Physics.Raycast(ray, out hit) && !gc.isMouseOverUi()) {
                        if (currentInstance == null) {
                            currentInstance = InstantiateObjects(hit.point);
                            instanceRenderer = currentInstance.GetComponent<MeshRenderer>();
                        } else {
                            if (!rotateArrows) {
                                PlaceAdjust(hit, buildInt.currentObject());
                                currentInstance.transform.eulerAngles = rot;
                            }
                            if (Input.GetMouseButton(1)) {
                                RotateObject(hit);
                            } else {
                                rotateArrows = false;
                            }
                        }
                        if (Input.GetMouseButton(0) && canBePlaced == true) {
                            DoorsCantFly();
                            worldGrid.placeObjectInPoint(currentInstance.transform.position, buildInt.currentObject(), currentInstance);
                            PlaceObject();
                        }

                    } else {
                        if (!rotateArrows && currentInstance != null) {
                            Destroy(currentInstance);
                            currentInstance = null;
                            instanceRenderer = null;
                            canBePlaced = false;
                        }
                    }
                    break;

                case BuildingInterface.thingsToDo.fill:
                    if (somethingSelected) {
                        instanceArray = new List<GameObject>();
                        alreadyFilled = new List<Vector3Int>();
                        somethingSelected = false;

                    }
                    if (Physics.Raycast(ray, out hit) && !gc.isMouseOverUi()) {
                        if(worldGrid.getThingInPoint(hit.point,currentFloor) == Node.worldObject.empty) {
                            if(buildInt.currentObject() != Node.worldObject.door && buildInt.currentObject() != Node.worldObject.stairsUp)
                            if (ChangedCursorPosition(hit.point))
                                InitiateFillOperation(hit.point);
                            if (Input.GetMouseButton(0)) {
                                PlaceFillObjects();
                            }
                        } else {
                            DestroyFillObjects();
                        }
                    } else {
                        DestroyFillObjects();
                    }
                            break;
                case BuildingInterface.thingsToDo.select:
                    HighlightSelectedObjects();
                    if (Physics.Raycast(ray, out hit) && !gc.isMouseOverUi()) {
                        HighlightThisObject(hit.point, new Color(0, 0, 0.5f, 0.5f));
                        if (Input.GetMouseButton(0))  {
                            AddToSelected(hit.point);
                        } else if (Input.GetMouseButton(1)) {
                            DragSelect(hit.point);
                        } else if(startDragSelect) {
                            IterateThruSelection(hit.point);
                            startDragSelect = false;
                        }                       
                    }
                    
                    break;
            }
        }

    }
}
