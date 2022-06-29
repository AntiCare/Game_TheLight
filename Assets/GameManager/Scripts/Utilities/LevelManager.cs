using System;
using System.Collections.Generic;
using Quests;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject MainCamera;
    [SerializeField] private GameObject luluPrefab;
    [SerializeField] private GameObject discoverManager;

    private Player       Lulu;
    private QuestManager questManager;

    public Player Player
    {
        get => Lulu;
        set => Lulu = value;
    }

    private bool hasEnemies;

    public bool HasEnemies
    {
        get => hasEnemies;
        set => hasEnemies = value;
    }

    [SerializeField] private Transform  bossLocation;
    [SerializeField] private GameObject bossPrefab;

    [SerializeField] private List<Transform> rangedEnemyLocations;
    [SerializeField] private GameObject      rangedEnemyPrefab;

    [SerializeField] private List<Transform> meleeEnemySpawnLocations;
    [SerializeField] private GameObject      meleeEnemyPrefab;

    [SerializeField] private List<NpcLocation> npcLocations;

    [SerializeField] private GameObject ENEMIES;

    [Serializable]
    public struct NpcLocation
    {
        public Transform  npcLocation;
        public GameObject npcPrefab;
    }

    private readonly List<GameObject> instantiatedEnemyObjects = new List<GameObject>();

    void Start()
    {
        SpawnMain();
        SpawnNpc();
    }

    public void SpawnEnemies()
    {
        if (hasEnemies)
        {
            return;
        }

        GameObject enemy = Instantiate(ENEMIES, Vector3.zero, Quaternion.identity, transform);

        AddToInstantiatedObjects(enemy);

        hasEnemies = true;
    }

    public void SpawnNpc()
    {
        foreach (NpcLocation npcLocation in npcLocations)
        {
            GameObject npc = Instantiate(npcLocation.npcPrefab, npcLocation.npcLocation.position, Quaternion.identity,
                transform);
            AddToInstantiatedObjects(npc);
        }
    }


    public void SpawnMain()
    {
        GameObject lulu = Instantiate(luluPrefab);
        GameObject cam  = Instantiate(MainCamera);

        Player = lulu.GetComponent<Player>();

        GameManager.Instance.Player = Player;

        SaveLoad.LoadData(out GameData data, GameManager.Instance.FilePath);

        Player.LoadData(data);
        
        GameManager.Instance.GameData = data;
        EnemyCounter.InitData(data.slimeCounter, data.rockerCounter, data.bossCounter);
        QuestManager.Instance.Load(data);

        AddToInstantiatedObjects(cam);
        AddToInstantiatedObjects(lulu);
    }

    public void DestroyInstantiatedObjects()
    {
        foreach (GameObject instantiatedEnemyObject in instantiatedEnemyObjects)
        {
            Destroy(instantiatedEnemyObject);
        }
    }

    void AddToInstantiatedObjects(GameObject obj)
    {
        instantiatedEnemyObjects.Add(obj);
    }
}