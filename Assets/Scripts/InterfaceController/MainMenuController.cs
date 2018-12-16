using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

    public Button New;
    public Button Load;
    public Button Creadits;
    public Button New2;
    public Button Back1;
    public Button Back2;
    public InputField inputx;
    public InputField inputy;
    public InputField inputz;

    public GameObject mainmenu;
    public GameObject newmenu;
    public GameObject credmenu;

    // Use this for initialization
    void Start () {
        New.onClick.AddListener(NewMap);
        New2.onClick.AddListener(StartNewMap);
        Load.onClick.AddListener(LoadMap);
        Creadits.onClick.AddListener(ShowCreadits);
        Back1.onClick.AddListener(GoBack);
        Back2.onClick.AddListener(GoBack2);


    }

    private void StartNewMap() {
        int gridSizeX = Mathf.Clamp((int.Parse(inputx.text) - (int.Parse(inputx.text) % 10)), 10, 1000);
        int gridSizeZ = Mathf.Clamp((int.Parse(inputz.text) - (int.Parse(inputz.text) % 10)), 10, 1000);
        Debug.Log(18 + " " + (18 % 10) + "  " + (18 - (18 % 10)));
        GameObject.FindWithTag("GlobalController").GetComponent<GlobalController>().SetSizeOfWorld(new Vector3Int(gridSizeX, int.Parse(inputy.text), gridSizeZ));
        SceneManager.LoadScene("Scenes/WorldView", LoadSceneMode.Single);
    }

    private void GoBack2() {
        mainmenu.SetActive(true);
        credmenu.SetActive(false);
    }

    private void GoBack() {
        mainmenu.SetActive(true);
        newmenu.SetActive(false);
    }

    private void ShowCreadits() {
        mainmenu.SetActive(false);
        credmenu.SetActive(true);
    }

    private void LoadMap() {
        SaveController.InitLoadSave();
    }


    public void NewMap() {
        mainmenu.SetActive(false);
        newmenu.SetActive(true);
    }


}
