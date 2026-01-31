using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelsManager : MonoBehaviour
{
    public static ReelsManager Instance { get; private set; }
    [SerializeField] PatternDatabaseSO patternDatabase;
    [SerializeField] float currentBet = 1f;

    PatternManager patternManager;

    [Serializable]
    public struct ReelConfig
    {
        public int totalCards;
        public float symbolHeight;

        public float maxSpinSpeed;
        public float acceleration;

        public float minStopSpeed;
        public float stopDistance;
        public float injectDistance;
    }

    public ReelConfig reelConfig;

    [SerializeField] float spinStartDelay = 0.5f;


[SerializeField] PayTableDatabaseSO payTable;

    public Transform reel1, reel2, reel3, reel4, reel5;

    [Header("Pooling")]
    public Card cardPrefab;
    public Transform cardHolderParent;
    public int initialPoolSize = 30;
    public CardCollectionSO cardDatabase;


    private ObjectPoolManager<Card> cardPool;
    private readonly List<Reel> reels = new();

    void Awake()
    {
        Instance = this;
        InitializePool();
    }

    void Start()
    {
        reels.Add(CreateReel(reel1));
        reels.Add(CreateReel(reel2));
        reels.Add(CreateReel(reel3));
        reels.Add(CreateReel(reel4));
        reels.Add(CreateReel(reel5));

        patternManager = new PatternManager(patternDatabase, payTable);
        OnAllReelsStopped += EvaluatePatterns;
    }

    Reel CreateReel(Transform root)
    {
        Reel reel = new Reel(reelConfig);
        reel.Initialize(root);
        return reel;
    }

    public Action OnAllReelsStopped;

    int stoppedReels = 0;

    public void NotifyReelStopped()
    {
        stoppedReels++;

        if (stoppedReels >= reels.Count)
        {
            stoppedReels = 0;
            OnAllReelsStopped?.Invoke();
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;

        foreach (var reel in reels)
            reel.Tick(dt);
    }


    public void SpinAll()
    {
        AssignRandomResults();
        StartCoroutine(SpinRoutine());
    }

    IEnumerator SpinRoutine()
    {
        foreach (var reel in reels)
        {
            reel.StartSpin();
            yield return new WaitForSeconds(spinStartDelay);
            NotifyReelStopped();
        }
    }

    int GetRandomSymbolId()
    {
        return cardDatabase.GetRandom().config.id;
    }
    // void AssignRandomResults()
    // {
    //     foreach (var reel in reels)
    //     {
    //         int top = GetRandomSymbolId();
    //         int center = GetRandomSymbolId();
    //         int bottom = GetRandomSymbolId();

    //         reel.SetForcedResult(top, center, bottom);
    //     }
    // }
    void AssignRandomResults()
    {
        // Top row = win
        reels[0].SetForcedResult(1, 2, 3);
        reels[1].SetForcedResult(1, 9, 9);
        reels[2].SetForcedResult(1, 2, 2);
        reels[3].SetForcedResult(1, 9, 9);
        reels[4].SetForcedResult(1, 9, 2);
    }

    public int[,] BuildGrid()
    {
        int[,] grid = new int[3, 5];

        for (int col = 0; col < reels.Count; col++)
        {
            int[] symbols = reels[col].GetVisibleSymbols();

            grid[0, col] = symbols[0]; // top
            grid[1, col] = symbols[1]; // center
            grid[2, col] = symbols[2]; // bottom
        }

        return grid;
    }

    void EvaluatePatterns()
    {
        int[,] grid = BuildGrid();

         float win = patternManager.Evaluate(grid, currentBet);

        Debug.Log("TOTAL WIN: " + win);
    }


    void OnDestroy()
    {
        foreach (var reel in reels)
            reel.Dispose();
    }

    // ---------------- POOL ----------------
    void InitializePool()
    {
        cardPool = new ObjectPoolManager<Card>(
            cardPrefab,
            cardHolderParent,
            initialPoolSize
        );
    }

    public Card GetCard(Transform parent) => cardPool.Get(parent);
    public void ReturnCard(Card card) => cardPool.Return(card);
}
