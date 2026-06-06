using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSaveManager : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private GraphManager graphManager;
    [SerializeField] private CoinWallet coinWallet;
    [SerializeField] private DayNightCycle dayNightCycle;
    [SerializeField] private CatUIList catUIList;

    [Header("Cats")]
    [SerializeField] private List<GameObject> catPrefabs = new();

    [Header("Resources")]
    [SerializeField] private CatPlaceableResource foodPrefab;
    [SerializeField] private CatPlaceableResource bedPrefab;

    [Header("Traffic Lights")]
    [SerializeField] private TrafficLightController trafficLightPrefab;
    [SerializeField] private List<TrafficLightSlot> trafficLightSlots = new();

    [Header("Dogs")]
    [SerializeField] private DogDangerSystem dogDangerSystem;

    [Header("Save UI")]
    [SerializeField] private List<SaveSlotUI> saveSlotUIs = new();
    [SerializeField] private GameObject saveSuccessPanel;

    private void Start()
    {
        if (SaveLoadBridge.ShouldLoadGame)
        {
            LoadGame(SaveLoadBridge.SelectedSlot);

            SaveLoadBridge.ShouldLoadGame = false;
        }
    }

    public void SaveGame(int slot)
    {
        SaveData data = new SaveData();

        data.saveDate = System.DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        data.totalGameTime = Time.timeSinceLevelLoad;

        SaveCoins(data);
        SaveDayNight(data);
        SaveCats(data);
        SaveResources(data);
        SaveTrafficLights(data);
        SaveBlockedNodes(data);
        SaveDogs(data);

        SaveSystem.Save(data, slot);

        foreach (SaveSlotUI slotUI in saveSlotUIs)
        {
            if (slotUI != null)
                slotUI.Refresh();
        }

        if (saveSuccessPanel != null)
            saveSuccessPanel.SetActive(true);
    }

    public void LoadGame(int slot)
    {
        SaveData data = SaveSystem.Load(slot);

        if (data == null)
            return;

        ClearSceneBeforeLoad();

        LoadCoins(data);
        LoadDayNight(data);
        LoadResources(data);
        LoadTrafficLights(data);
        LoadBlockedNodes(data);
        LoadDogs(data);
        LoadCats(data);
    }

    private void SaveCoins(SaveData data)
    {
        if (coinWallet != null)
            data.coins = coinWallet.Coins;
    }

    private void LoadCoins(SaveData data)
    {
        Debug.Log("Загружаю монеты: " + data.coins);

        if (coinWallet != null)
            coinWallet.SetCoins(data.coins);
    }

    private void SaveDayNight(SaveData data)
    {
        if (dayNightCycle == null)
            return;

        data.isDay = dayNightCycle.IsDay;
        data.days = dayNightCycle.Days;
        data.dayNightTimer = dayNightCycle.Timer;
    }

    private void LoadDayNight(SaveData data)
    {
        if (dayNightCycle != null)
            dayNightCycle.LoadState(data.isDay, data.days, data.dayNightTimer);
    }

    private void SaveCats(SaveData data)
    {
        CatNeeds[] cats = FindObjectsByType<CatNeeds>(FindObjectsSortMode.None);

        foreach (CatNeeds needs in cats)
        {
            if (needs == null)
                continue;

            CatProfile profile = needs.GetComponent<CatProfile>();

            string prefabName = needs.gameObject.name.Replace("(Clone)", "").Trim();

            data.cats.Add(new CatSaveData
            {
                prefabName = prefabName,
                catName = profile != null ? profile.CatName : "Кошка",

                position = needs.transform.position,
                rotation = needs.transform.rotation,

                hunger = needs.hunger,
                sleepiness = needs.sleepiness,
                happiness = needs.happiness,
                health = needs.health
            });
        }
    }

    private void LoadCats(SaveData data)
    {
        foreach (CatSaveData catData in data.cats)
        {
            GameObject prefab = GetCatPrefab(catData.prefabName);

            if (prefab == null)
            {
                Debug.LogWarning("Не найден prefab кота: " + catData.prefabName);
                continue;
            }

            GameObject catObject = Instantiate(
                prefab,
                catData.position,
                catData.rotation
            );

            CatProfile profile = catObject.GetComponent<CatProfile>();
            if (profile != null)
                profile.CatName = catData.catName;

            CatNeeds needs = catObject.GetComponent<CatNeeds>();
            if (needs != null)
            {
                needs.hunger = catData.hunger;
                needs.sleepiness = catData.sleepiness;
                needs.happiness = catData.happiness;
                needs.health = catData.health;

                if (catUIList != null)
                    catUIList.AddCat(needs);
            }

            CatBrain brain = catObject.GetComponent<CatBrain>();
            if (brain != null)
                brain.GraphManager = graphManager;
        }
    }

    private void SaveResources(SaveData data)
    {
        if (graphManager == null)
            return;

        foreach (GraphNode node in graphManager.Nodes)
        {
            if (node == null || node.CurrentResource == null)
                continue;

            data.resources.Add(new ResourceSaveData
            {
                nodeId = node.Id,
                resourceType = node.CurrentResource.ResourceType,
                usesLeft = node.CurrentResource.UsesLeft
            });
        }
    }

    private void LoadResources(SaveData data)
    {
        foreach (ResourceSaveData resourceData in data.resources)
        {
            GraphNode node = GetNodeById(resourceData.nodeId);

            if (node == null)
                continue;

            CatPlaceableResource prefab =
                resourceData.resourceType == CatResourceType.Food
                    ? foodPrefab
                    : bedPrefab;

            NodeType newNodeType =
                resourceData.resourceType == CatResourceType.Food
                    ? NodeType.Food
                    : NodeType.SleepSpot;

            if (prefab == null)
                continue;

            CatPlaceableResource resource = Instantiate(
                prefab,
                node.Position,
                Quaternion.identity
            );

            resource.InitLoaded(node, newNodeType, resourceData.usesLeft);
            node.SetResource(resource);
        }
    }

    private void SaveTrafficLights(SaveData data)
    {
        for (int i = 0; i < trafficLightSlots.Count; i++)
        {
            TrafficLightSlot slot = trafficLightSlots[i];

            if (slot != null && slot.IsOccupied)
            {
                data.trafficLights.Add(new TrafficLightSaveData
                {
                    slotIndex = i
                });
            }
        }
    }

    private void LoadTrafficLights(SaveData data)
    {
        foreach (TrafficLightSaveData lightData in data.trafficLights)
        {
            if (lightData.slotIndex < 0 || lightData.slotIndex >= trafficLightSlots.Count)
                continue;

            TrafficLightSlot slot = trafficLightSlots[lightData.slotIndex];

            if (slot == null || trafficLightPrefab == null)
                continue;

            TrafficLightController light = Instantiate(
                trafficLightPrefab,
                slot.transform.position,
                slot.transform.rotation
            );

            slot.Crosswalk.AttachTrafficLight(light);
            slot.MarkOccupied();
        }
    }

    private void SaveBlockedNodes(SaveData data)
    {
        if (graphManager == null)
            return;

        foreach (GraphNode node in graphManager.Nodes)
        {
            if (node != null && node.IsBlocked)
            {
                data.blockedNodes.Add(new NodeBlockSaveData
                {
                    nodeId = node.Id
                });
            }
        }
    }

    private void LoadBlockedNodes(SaveData data)
    {
        foreach (NodeBlockSaveData blockData in data.blockedNodes)
        {
            GraphNode node = GetNodeById(blockData.nodeId);

            if (node == null)
                continue;

            NodeBlocker blocker = node.GetComponent<NodeBlocker>();

            if (blocker != null)
                blocker.SetBlocked(true);
            else
                node.IsBlocked = true;
        }
    }

    private void SaveDogs(SaveData data)
    {
        DogDanger[] dogs = FindObjectsByType<DogDanger>(FindObjectsSortMode.None);

        foreach (DogDanger dog in dogs)
        {
            if (dog == null || dog.Node == null)
                continue;

            data.dogs.Add(new DogSaveData
            {
                nodeId = dog.Node.Id
            });
        }
    }

    private void LoadDogs(SaveData data)
    {
        if (dogDangerSystem == null)
            dogDangerSystem = FindObjectOfType<DogDangerSystem>();

        if (dogDangerSystem == null)
            return;

        foreach (DogSaveData dogData in data.dogs)
        {
            GraphNode node = GetNodeById(dogData.nodeId);

            if (node != null)
                dogDangerSystem.SpawnDogOnNode(node);
        }
    }

    private void ClearSceneBeforeLoad()
    {
        if (catUIList != null)
            catUIList.ClearAll();

        foreach (CatNeeds cat in FindObjectsByType<CatNeeds>(FindObjectsSortMode.None))
            Destroy(cat.gameObject);

        foreach (CatPlaceableResource resource in FindObjectsByType<CatPlaceableResource>(FindObjectsSortMode.None))
            Destroy(resource.gameObject);

        foreach (DogDanger dog in FindObjectsByType<DogDanger>(FindObjectsSortMode.None))
            dog.Remove();

        foreach (TrafficLightController light in FindObjectsByType<TrafficLightController>(FindObjectsSortMode.None))
            Destroy(light.gameObject);

        if (graphManager != null)
        {
            foreach (GraphNode node in graphManager.Nodes)
            {
                if (node == null)
                    continue;

                node.IsBlocked = false;

                if (node.CurrentResource != null)
                    node.ClearResource(node.CurrentResource);

                if (node.Type == NodeType.Food || node.Type == NodeType.SleepSpot)
                    node.Type = NodeType.CatsZone;
            }
        }

        foreach (TrafficLightSlot slot in trafficLightSlots)
        {
            if (slot != null)
                slot.ResetSlot();
        }
    }

    private GraphNode GetNodeById(int id)
    {
        if (graphManager == null)
            return null;

        foreach (GraphNode node in graphManager.Nodes)
        {
            if (node != null && node.Id == id)
                return node;
        }

        return null;
    }

    private GameObject GetCatPrefab(string prefabName)
    {
        foreach (GameObject prefab in catPrefabs)
        {
            if (prefab != null && prefab.name == prefabName)
                return prefab;
        }

        return null;
    }
}
