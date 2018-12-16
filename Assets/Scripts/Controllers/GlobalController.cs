using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalController : MonoBehaviour {

    public Vector3Int sizeOfWorld;
    public GameGrid grid;
    public List<Creature> creatures;

    public string firstSceneName;

    private bool gameLoaded;
    private bool mouseOverUi;
    private bool quickBuild = false;
    private bool quickErase = false;

    public static GlobalController Instance;

    void Awake() {
        if (Instance == null) {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            gameLoaded = false;
            creatures = new List<Creature>();
        } else if (Instance != this) {
            Destroy(gameObject);
        }
        
    }

    void FileSelected(string url) {
        StartCoroutine(LoadFile(url));
    }

    IEnumerator LoadFile(string url) {
        WWW file = new WWW(url);
        yield return file;
        SaveController.LoadSave(new System.IO.MemoryStream(file.bytes));
    }

    public void InitializeGameLoad() {
        gameLoaded = true;
        SceneManager.LoadScene("Scenes/WorldView", LoadSceneMode.Single);



    }

    public void SetSizeOfWorld(Vector3Int _size ) {
        sizeOfWorld = _size;
    }

    public Vector3Int getSize() {
        return sizeOfWorld;
    }

    public bool isMouseOverUi() {
        return mouseOverUi;
    }

    public bool isGameLoaded() {
        return gameLoaded;
    }

    public bool quickBuildEnabled() {
        return quickBuild;
    }

    public bool quickEraseEnabled() {
        return quickErase;
    }

    public void setMouseOverUi(bool _a) {
        mouseOverUi = _a;
    }

    public GameGrid getUpdatedGrid() {
        grid = GameObject.FindWithTag("BuildingController").GetComponent<BuildingController>().getWorldGrid();

        return grid;
    }

    public GameGrid getActualGrid() {
        return grid;
    }
}
