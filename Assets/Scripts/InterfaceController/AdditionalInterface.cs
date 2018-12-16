using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdditionalInterface : MonoBehaviour {
    
    //additonal menu buttons
    [Header("Additional menu")]
    public Button newMap;
    public Button loadMap;
    public Button saveMap;
    public Button settings;


    //new menu
    [Header("New menu buttons")]
    public Button startNew;
    public InputField y;
    public InputField x;
    public InputField z;

    //new menu alert
    [Header("New menu alert")]
    public Button saveNA;
    public Button newNA;

    //save menu alert
    [Header("Save menu alert")]
    public Button saveLA;
    public Button loadLA;

    //seve menu
    [Header("Save menu ")]
    public InputField fileNameInput;
    public Button saveButon;

    //settings Menu
    [Header("Settings menu ")]

    //close buttons
    [Header("Close buttons")]
    public Button close1;
    public Button close2;
    public Button close3;
    public Button close4;
    public Button close5;


    //windows
    [Header("Windows")]
    public GameObject newMenu;
    public GameObject newMenuAlert;
    public GameObject loadMenuAlert;
    public GameObject saveMenu;
    public GameObject settingsMenu;

    private bool newAfterSave;
    private bool loadAfterSave;
	// Use this for initialization
	void Start () {
        //open window
        {
            newMap.onClick.AddListener(delegate { ShowMenu(newMenuAlert); });
            loadMap.onClick.AddListener(delegate { ShowMenu(loadMenuAlert); });
            saveMap.onClick.AddListener(delegate { ShowMenu(saveMenu); });
            settings.onClick.AddListener(delegate { ShowMenu(settingsMenu); });
        }
        //new Menu
        {
            startNew.onClick.AddListener(StartNewMap);
        }
        //new menu alert
        {
            saveNA.onClick.AddListener(delegate { ShowMenu(saveMenu); });
            newNA.onClick.AddListener(delegate { ShowMenu(newMenu); });
        }
        //load menu alert
        {
            saveLA.onClick.AddListener(delegate { ShowMenu(saveMenu); });
            loadLA.onClick.AddListener(LoadGame);
        }
        //save menu 
        {
            saveButon.onClick.AddListener(SaveGame);
        }


        //closes 
        {
            close1.onClick.AddListener(delegate { CloseCurrentWindow(close1.transform.parent.gameObject); });
            close2.onClick.AddListener(delegate { CloseCurrentWindow(close2.transform.parent.gameObject); });
            close3.onClick.AddListener(delegate { CloseCurrentWindow(close3.transform.parent.gameObject); });
            close4.onClick.AddListener(delegate { CloseCurrentWindow(close4.transform.parent.gameObject); });
            close5.onClick.AddListener(delegate { CloseCurrentWindow(close5.transform.parent.gameObject); });
        }
    }

    private void LoadGame() {
        SaveController.InitLoadSave();
    }

   

    void SaveGame() {
        GlobalController gc = GameObject.FindWithTag("GlobalController").GetComponent<GlobalController>();
        BuildingController bc = GameObject.FindWithTag("BuildingController").GetComponent<BuildingController>();
        SaveController.MakeSave(bc.getWorldGrid(), gc.creatures, fileNameInput.text);
        if (newAfterSave) {
            ShowMenu(newMenu);
            newAfterSave = false;
        } else if (loadAfterSave) {
            LoadGame();
            loadAfterSave = false;
        }
        saveMenu.SetActive(false);
    }

    void StartNewMap() {
        GameObject.FindWithTag("GlobalController").GetComponent<GlobalController>().SetSizeOfWorld(
            new Vector3Int(
                 x.text != "" && int.Parse(x.text) >=10 ? int.Parse(x.text) : 10,
                 y.text != "" && int.Parse(y.text) >= 1 ? int.Parse(y.text) : 10,
                 z.text != "" && int.Parse(z.text) >= 10 ? int.Parse(z.text) : 10));
        SceneManager.LoadScene("Scenes/WorldView", LoadSceneMode.Single);

    }


    void CloseCurrentWindow(GameObject _parent) {
        _parent.SetActive(false);
    }

    void ShowMenu(GameObject _openThis) {
        //dependencies
        {
            newAfterSave = false;
            loadAfterSave = false;
            if (newMenuAlert.active)
                newAfterSave = true;
            if (loadMenuAlert.active)
                loadAfterSave = true;
        }
        //hidding windows
        {
            newMenu.SetActive(false);
            loadMenuAlert.SetActive(false);
            newMenuAlert.SetActive(false);
            saveMenu.SetActive(false);
            settingsMenu.SetActive(false);
        }
        _openThis.SetActive(true);

    }
}
