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

    public GemConfigs getGemConfigs => _gemConfigs;
    public GameObject getGemPrefab => _gemPrefab;
    public HexGrid getHexGrid => _hexGrid;
    
    [Space(10)]
    private GameObject currentGem;
    
    [Header("Gem Properties")]
    [SerializeField] private bool randomSeed = false;
    [SerializeField] private int seed;
    public List<GemConfig> gemPool = new List<GemConfig>();
    public List<int> nextGemIDList = new List<int>();
    private int gemSpawned = 0;
    private int gemSpeed = 25;
    [SerializeField] private Transform GemStartPos;

    [Header("Game Settings")] 
    private bool _gameStarted = false;
    [SerializeField] private bool _gameOnGoing = true;
    [SerializeField] private int matchToClear = 3;
    [SerializeField] private float _gameTime = 0;
    [SerializeField] private float _dangerTime = 60;
    [SerializeField] private bool _isPaused = false;

    public int MatchToClear => matchToClear;
    public bool GameOnGoing => _gameOnGoing;
    public bool GameIsPause
    {
        get { return _isPaused; }
        set { _isPaused = value; }
    } 
    
    [Header("Cave Rumble")]
    private bool _rumbleStarted = false;
    [SerializeField] private int rumbleInstances = 3;
    [SerializeField] private List<Transform> rumbleAffectedObjects = new List<Transform>();

    [Header("GemCollate")] 
    public List<CollatedRemainingGems> collatedGems = new List<CollatedRemainingGems>();

    [SerializeField] private int gemsRemaining = 0;
    
    

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
        if(!_gameStarted || !_gameOnGoing || _isPaused)
            return;
        
        if (!_isPaused)
        {
            _gameTime += Time.deltaTime;
            if (_gameTime > _dangerTime)
            {
                GameUI.Instance.ChangeTimerUI(_gameTime, true);
                if (!_rumbleStarted)
                {
                    _rumbleStarted = true;
                }
            }
            else
                GameUI.Instance.ChangeTimerUI(_gameTime, false);
        }

        if (gemsRemaining <= 0)
        {
            GameOver();
        }
    }
    public void StartGame()
    {
        if (randomSeed)
            seed = Random.Range(0, 100);
        foreach (var gc in _gemConfigs.listOfGems)
        {
            CollatedRemainingGems cl = new CollatedRemainingGems();
            cl.gemType = gc.gemtype;
            collatedGems.Add(cl);
            gemPool.Add(gc);
        }
        _levelManager.BuildLevel();
        Invoke("EvaluateProgress", 1f);
        Invoke("FirstSpawnGem", 1.5f);
    }
    private void FirstSpawnGem()
    {
        if (nextGemIDList.Count <= 0)
        {
            for(int i = 0; i< 3;i++)
            {
                if (nextGemIDList.Count <= 0)
                {
                    int generatedGemID = seed % gemPool.Count;
                    nextGemIDList.Add(generatedGemID);
                }
                else
                {
                    int generatedGemID = Mathf.Abs(((seed * nextGemIDList[nextGemIDList.Count - 1]) - gemSpawned) % gemPool.Count);
                    nextGemIDList.Add(generatedGemID);
                }
            }
        }
        GemUI.Instance.UpdateNextGems(nextGemIDList,gemPool);
        Invoke("SpawnGem", 1f);
        _gameStarted = true;
        GameUI.Instance.ShowScreen(true);
    }
    public void SpawnGem()
    {
        if(!_gameOnGoing)
            return;
        MatchManager.Instance.ClearList();
        currentGem = Instantiate(getGemPrefab, GemStartPos.position, quaternion.identity);
        currentGem.GetComponent<Gem>().ConstructGem(gemPool[nextGemIDList[0]].gemtype,
            gemPool[nextGemIDList[0]].GemSprites[0],gemPool[nextGemIDList[0]].gemID);
        nextGemIDList.RemoveAt(0);
        gemSpawned++;
        GetNextGemID();
    }
    private void GetNextGemID()
    {
        int nextGemID = Mathf.Abs(((seed * nextGemIDList[nextGemIDList.Count-1]) - gemSpawned) % gemPool.Count);
        nextGemIDList.Add(nextGemID);
        GemUI.Instance.UpdateNextGems(nextGemIDList,gemPool);
    }
    private void RerollGems()
    {
        if(gemPool.Count<=0)
            return;
        for (int i = 0; i < nextGemIDList.Count; i++)
        {
            if (nextGemIDList[i] >= gemPool.Count)
            {
                int randIntFromPool = (gemPool.Count <= 1) ? 0 : Random.Range(0, gemPool.Count);
                nextGemIDList[i] = randIntFromPool;
            }
        }
        SpawnGem();
        GemUI.Instance.UpdateNextGems(nextGemIDList,gemPool);
    }
    public void LaunchGem(Vector2 dir)
    {
        if(!_gameOnGoing)
            return;
        if(currentGem == null)
            return;
        currentGem.GetComponent<Gem>().LaunchGem(dir,gemSpeed);
        currentGem = null;
    }
    private void CaveRumble()
    {
        foreach (var obj in rumbleAffectedObjects)
        {
            obj.transform.position = new Vector2(obj.transform.position.x, obj.transform.position.y - 1);
        }
        _hexGrid.OffsetYOrigin();
        MatchManager.Instance.CheckLoseLine();
    }
    public void GameOver()
    {
        _gameOnGoing = false;
        StopAllCoroutines();
        if(gemsRemaining>0)
            GameUI.Instance.EndGame(false);
        else
        {
            GameUI.Instance.EndGame(true);
        }
    }
    public void EvaluateProgress()
    {
        CollateGems();
        if(_rumbleStarted && gemSpawned%rumbleInstances==0)
            CaveRumble();
        for(int c = 0; c<collatedGems.Count;c++)
        {
            if (collatedGems[c].remainingGemType <= 0)
            {
                foreach (var gp in gemPool)
                {
                    if (gp.gemtype == collatedGems[c].gemType)
                    {
                        gemPool.Remove(gp);
                        RerollGems();
                        return;
                    }
                }
            }
        }
        if (_gameStarted && _gameOnGoing)
            Invoke("SpawnGem",.6f);
    }

    private void CollateGems()
    {
        foreach (var clG in collatedGems)
        {
            clG.remainingGemType = 0;
        }
        gemsRemaining = 0;
        foreach (var hexRows in _hexGrid.getHexRows)
        {
            if (hexRows.rowCollatedGems.Count > 0)
            {
                gemsRemaining += hexRows.gemCount;
                foreach (var clGem in hexRows.rowCollatedGems)
                {
                    AddGemsToCollate(clGem);
                }
            }
        }
    }
    private void AddGemsToCollate(CollatedRemainingGems colGm)
    {
        if (collatedGems.Count <= 0)
        {
            CollatedRemainingGems cl = new CollatedRemainingGems();
            cl.gemType = colGm.gemType;
            cl.remainingGemType += colGm.remainingGemType;
            collatedGems.Add(cl);
        }
        else
        {
            foreach (var crg in collatedGems)
            {
                if (crg.gemType == colGm.gemType)
                {
                    crg.remainingGemType+=colGm.remainingGemType;
                    return;
                }
            }
            CollatedRemainingGems cl = new CollatedRemainingGems();
            cl.gemType = colGm.gemType;
            cl.remainingGemType += colGm.remainingGemType;
            collatedGems.Add(cl);
            
        }
    }
    
}

[System.Serializable]
public class CollatedRemainingGems
{
    public GemTypes gemType;
    public int remainingGemType;
}
