using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public string saveDate;
    public float totalGameTime;

    public int coins;

    public bool isDay;
    public int days;
    public float dayNightTimer;

    public List<CatSaveData> cats = new();
    public List<ResourceSaveData> resources = new();
    public List<TrafficLightSaveData> trafficLights = new();
    public List<NodeBlockSaveData> blockedNodes = new();
    public List<DogSaveData> dogs = new();
}

[Serializable]
public class CatSaveData
{
    public string prefabName;
    public string catName;

    public Vector3 position;
    public Quaternion rotation;

    public float hunger;
    public float sleepiness;
    public float happiness;
    public float health;
}

[Serializable]
public class ResourceSaveData
{
    public int nodeId;
    public CatResourceType resourceType;
    public int usesLeft;
}

[Serializable]
public class TrafficLightSaveData
{
    public int slotIndex;
}

[Serializable]
public class NodeBlockSaveData
{
    public int nodeId;
}

[Serializable]
public class DogSaveData
{
    public int nodeId;
}
