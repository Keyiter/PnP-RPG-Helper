using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainInterface : MonoBehaviour {

    public Button addMenu;
    public Button swCam;
    public Button up;
    public Button down;
    public Button playMode;
    public Button creatureMode;
    public Button buildMode;

    public GameObject addMenuObj;
    public GameObject playModeMenuObj;
    public GameObject creatureModeObj;
    public GameObject buildModeObj;

    BuildingController buildCtrl;


    // Use this for initialization
    void Start () {
        buildCtrl = GameObject.FindWithTag("BuildingController").GetComponent<BuildingController>();
        addMenu.onClick.AddListener(toggleAddMenu);
        playMode.onClick.AddListener(togglePlayMode);
        creatureMode.onClick.AddListener(toggleCreatureMode);
        buildMode.onClick.AddListener(toggleBuildMode);
        up.onClick.AddListener(FloorUp);
        down.onClick.AddListener(FloorDown);
    }
	
	public void toggleAddMenu() {
        if (addMenuObj.active)
            addMenuObj.SetActive(false);
        else
            addMenuObj.SetActive(true);
    }

    public void toggleBuildMode() {
        if (buildModeObj.active) {
            buildModeObj.SetActive(false);
            buildCtrl.isBuilding = false;
        } else {
            buildModeObj.SetActive(true);
            buildCtrl.isBuilding = true;
            playModeMenuObj.SetActive(false);
            creatureModeObj.SetActive(false);
        }
    }

    public void toggleCreatureMode() {
        if (creatureModeObj.active) {
            creatureModeObj.SetActive(false);

        } else {
            creatureModeObj.SetActive(true);
            playModeMenuObj.SetActive(false);
            buildModeObj.SetActive(false);
            buildCtrl.isBuilding = false;
        }
    }

    public void togglePlayMode() {
        if (playModeMenuObj.active) {
            playModeMenuObj.SetActive(false);
        } else {
            playModeMenuObj.SetActive(true);
            buildModeObj.SetActive(false);
            creatureModeObj.SetActive(false);
            buildCtrl.isBuilding = false;
        }
    }

    protected void FloorUp() {
        buildCtrl.floorUp();
    }

    protected void FloorDown() {
        buildCtrl.floorDown();
    }

}
