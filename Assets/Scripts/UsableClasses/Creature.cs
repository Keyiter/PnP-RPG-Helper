using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature {

    public enum creatureType { ally, enemy, neutral}
    public enum CreatureModel {human, fighter, wizard, rouge, healer, wolf, watcher, goblin, orc, ogre, giant, gargantum}
    public enum CreatureSex { male, female, none}
    public enum creatureSize { medium, Large, Huge, Gargantun}

    public string name;
    public creatureType type;
    public CreatureModel model;
    public CreatureSex sex;
    public creatureSize size;
    public Vector3Int worldSpacePosition;
    public GameObject placedObject;

    public Creature(string _name, Vector3Int _pos, creatureType _type = creatureType.neutral , CreatureModel _model = CreatureModel.human, CreatureSex _sex = CreatureSex.none, creatureSize _size = creatureSize.medium) {
        name = _name;
        type = _type;
        model = _model;
        sex = _sex;
        size = _size;
        worldSpacePosition = _pos;
    }

}
