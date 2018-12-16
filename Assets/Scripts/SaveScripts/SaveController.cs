using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.IO;
using System;

[System.Serializable]
public struct CreatureSave {
    public string name;
    public Creature.creatureType type;
    public Creature.CreatureModel model;
    public Creature.CreatureSex sex;
    public Creature.creatureSize size;
    public int[] worldSpacePosition;

    public CreatureSave(string _name, Creature.creatureType _type, Creature.CreatureModel _model, Creature.CreatureSex _sex,Creature.creatureSize _size, Vector3Int _pos) {
        name = _name;
        type = _type;
        model = _model;
        sex = _sex;
        size = _size;
        worldSpacePosition = new int[3] { _pos.x, _pos.y, _pos.z };
    }
}

[System.Serializable]
public struct NodeSave {
    public Node.worldObject placedThing;
    public Node.orientation direction;

    public int[] worldSpacePosition;
    public byte texture;

    public NodeSave(Node.worldObject _placedThing, Vector3Int _worldSpacePosition, byte _texture, Node.orientation _dir) {
        placedThing = _placedThing;
        worldSpacePosition = new int[3] { _worldSpacePosition.x, _worldSpacePosition.y, _worldSpacePosition.z } ;
        texture = _texture;
        direction = _dir;
    }


}

[System.Serializable]
public struct SaveData {
    public int[] dimension;
    public List<NodeSave> nodes;
    public List<CreatureSave> creatures;
}


public static class SaveController  {

    [DllImport("__Internal")]
    private static extern void DownloadFile(byte[] array, int size, String fileNamePtr);

    [DllImport("__Internal")]
    private static extern void WindowAlert(string message);

    [DllImport("__Internal")]
    private static extern void LoadData();

    public static SaveData PrepareSave(GameGrid _grid, List<Creature> _creatures) {
        SaveData save = new SaveData();

        int sizex = _grid.MaxSize(GameGrid.side.x);
        int sizey = _grid.MaxSize(GameGrid.side.y);
        int sizez = _grid.MaxSize(GameGrid.side.z);

        save.dimension = new int[3] { sizex, sizey, sizez };

        save.nodes = new List<NodeSave>();
        save.creatures = new List<CreatureSave>();
        for (int x = 0; x < sizex; x++)
            for (int y = 0; y < sizey; y++)
                for (int z = 0; z < sizez; z++) {
                  
                    if (_grid.getGridNode(x, y, z).placedThing != Node.worldObject.empty) {
                        NodeSave node = new NodeSave(_grid.getGridNode(x, y, z).placedThing, _grid.getGridNode(x, y, z).worldSpacePosition, _grid.getGridNode(x, y, z).texture, _grid.getGridNode(x, y, z).direction);
                       
                        save.nodes.Add(node);
                      
                    }
                }

        foreach (Creature actor in _creatures) {
            CreatureSave creat = new CreatureSave(actor.name, actor.type, actor.model, actor.sex, actor.size, actor.worldSpacePosition);
            save.creatures.Add(creat);
        }

        return save;
    }

    public static void UnpackSave(SaveData _save) {
        GlobalController GC = GameObject.FindWithTag("GlobalController").GetComponent<GlobalController>();
        GC.SetSizeOfWorld(new Vector3Int(_save.dimension[0], _save.dimension[1], _save.dimension[2]));
        GC.grid = new GameGrid(new Vector3Int(_save.dimension[0], _save.dimension[1], _save.dimension[2]));
        foreach(NodeSave node in _save.nodes) {
            GC.grid.placeObjectInPoint(new Vector3Int(node.worldSpacePosition[0], node.worldSpacePosition[1], node.worldSpacePosition[2]), node.placedThing, null,node.texture,true, node.direction);
        }
        foreach(CreatureSave creature in _save.creatures) {
            GC.creatures.Add(new Creature(creature.name, new Vector3Int(creature.worldSpacePosition[0], creature.worldSpacePosition[1], creature.worldSpacePosition[2]), creature.type, creature.model, creature.sex, creature.size));
        }
        GC.InitializeGameLoad();
        
    }

	public static void MakeSave(GameGrid _grid, List<Creature> _creatures, string _filename) {
       
        try {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream memo = new MemoryStream();

            SaveData data = PrepareSave(_grid, _creatures);

            bf.Serialize(memo, data);


            if (Application.platform == RuntimePlatform.WebGLPlayer)
                DownloadFile(memo.ToArray(), memo.ToArray().Length, _filename + ".save");
                
        } catch (Exception e) {
            PlatformSafeMessage("Failed to Save: " + e.Message);
        }
       
    }

    public static void InitLoadSave() {
        try {
            FileStream file;
            if (Application.platform == RuntimePlatform.WebGLPlayer) {
                LoadData();
            } else {
                file = File.Open(Application.persistentDataPath + "/SaveData.dat", FileMode.Open);
                // LoadSave(file);
            }
        } catch (Exception e) {
            PlatformSafeMessage("Failed to Save:" + e.Message);
        }
    }

    public static void LoadSave(MemoryStream _memo) {
        try {
            BinaryFormatter bf = new BinaryFormatter();
            
            

            SaveData save = (SaveData)bf.Deserialize(_memo);
            UnpackSave(save);

        } catch (Exception e) {
            PlatformSafeMessage("Failed to Load Save" + e.Message);
        }

    }


    public static void PlatformSafeMessage(string message) {
        if (Application.platform == RuntimePlatform.WebGLPlayer) {
            WindowAlert(message);
        } else {
            Debug.Log(message);
        }
    }
}
