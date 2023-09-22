using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.Mathematics;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("References")]
    [SerializeField] private HexGrid _hexGrid;
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private GemConfigs _gemConfigs;
    [SerializeField] private GameObject _gemPrefab;

    [Space(10)]
    private GameObject currentGem;
    
    [Header("Gem Properties")]
    [SerializeField] private int seed;
    private int nextGemID = -1;
    public List<int> nextGemIDList = new List<int>();
    private int gemSpawned = 0;
    private int gemSpeed = 25;
    [SerializeField] private Transform GemStartPos;

    [Header("GemCollate")] public List<CollatedRemainingGems> collatedGems = new List<CollatedRemainingGems>();
    
    public GemConfigs getGemConfigs => _gemConfigs;
    public GameObject getGemPrefab => _gemPrefab;
    public HexGrid getHexGrid => _hexGrid;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Invoke("StartGame", 2f);
    }

    void Update()
    {
        
    }

    public void StartGame()
    {
        foreach (var gc in _gemConfigs.listOfGems)
        {
            CollatedRemainingGems cl = new CollatedRemainingGems();
            cl.gemType = gc.gemtype;
            collatedGems.Add(cl);
        }
        _levelManager.BuildLevel();
         FirstSpawnTetromino();
    }
    private void FirstSpawnTetromino()
    {
        if (nextGemIDList.Count <= 0)
        {
            for(int i = 0; i< 3;i++)
            {
                if (nextGemIDList.Count <= 0)
                {
                    int generatedGemID = seed % _gemConfigs.listOfGems.Count;
                    nextGemIDList.Add(generatedGemID);
                }
                else
                {
                    int generatedGemID = Mathf.Abs(((seed * nextGemIDList[nextGemIDList.Count - 1]) - gemSpawned) % _gemConfigs.listOfGems.Count);
                    nextGemIDList.Add(generatedGemID);
                }
            }
        }
        SpawnGem();
    }
    
    public void SpawnGem()
    {
        MatchManager.Instance.ClearList();
        currentGem = Instantiate(getGemPrefab, GemStartPos.position, quaternion.identity);
        currentGem.GetComponent<Gem>().ConstructGem(_gemConfigs.listOfGems[nextGemIDList[0]].gemtype,
            _gemConfigs.listOfGems[nextGemIDList[0]].GemSprites[0],nextGemIDList[0]);
        nextGemIDList.RemoveAt(0);
        gemSpawned++;
        GetNextGemID();
    }
    private void GetNextGemID()
    {
        int nextTetrominoID = Mathf.Abs(((seed * nextGemIDList[nextGemIDList.Count-1]) - gemSpawned) % _gemConfigs.listOfGems.Count);
        nextGemIDList.Add(nextTetrominoID);
    }
    public void LaunchGem(Vector2 dir)
    {
        if(currentGem == null)
            return;
        currentGem.GetComponent<Gem>().LaunchGem(dir,gemSpeed);
        currentGem = null;
    }
}

[System.Serializable]
public class CollatedRemainingGems
{
    public GemTypes gemType;
    public int remainingGemType;
}
