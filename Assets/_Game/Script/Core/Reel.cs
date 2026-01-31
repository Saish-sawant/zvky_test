using System.Collections.Generic;
using UnityEngine;

public class Reel
{
    private readonly List<Card> cards = new();
    private readonly ReelsManager.ReelConfig config;

    private float currentSpeed;
    private float remainingDistance;

    private bool isSpinning;
    private bool resultLocked;
    private bool resultInjected;

    // Forced visible result: TOP, CENTER, BOTTOM
    //public int[] forcedResult = { 2, 4, 3 };

    int topSymbol,
           centerSymbol,
           bottomSymbol;


    // -----------------------------
    // CONSTRUCTOR
    // -----------------------------
    public Reel(ReelsManager.ReelConfig config)
    {
        this.config = config;
    }

    // -----------------------------
    // INITIALIZATION
    // -----------------------------
    public void Initialize(Transform parent)
    {
        int centerIndex = config.totalCards / 2;

        for (int i = 0; i < config.totalCards; i++)
        {
            Card card = ReelsManager.Instance.GetCard(parent);

            float y = (centerIndex - i) * config.symbolHeight;
            card.transform.localPosition = new Vector3(0f, y, 0f);

            AssignRandom(card);
            cards.Add(card);
        }
    }

    // -----------------------------
    // MANUAL UPDATE (called by manager)
    // -----------------------------
    public void Tick(float dt)
    {
        if (!isSpinning) return;

        UpdateSpeed(dt);
        Spin(dt);
    }

    // -----------------------------
    // PUBLIC API
    // -----------------------------
    public void StartSpin()
    {
        LockResult();
        CalculateStopDistance();

        resultInjected = false;
        currentSpeed = 0f;
        isSpinning = true;
    }

    // -----------------------------
    // CORE LOGIC
    // -----------------------------
    void UpdateSpeed(float dt)
    {
        if (remainingDistance > config.stopDistance)
        {
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                config.maxSpinSpeed,
                config.acceleration * dt
            );
        }
        else
        {
            float t = remainingDistance / config.stopDistance;
            currentSpeed = Mathf.Lerp(config.minStopSpeed, currentSpeed, t);
        }
    }

    void Spin(float dt)
    {
        float delta = currentSpeed * dt;
        remainingDistance -= delta;

        float limit = (config.totalCards / 2) * config.symbolHeight;

        foreach (var card in cards)
        {
            Transform t = card.transform;
            t.localPosition += Vector3.down * delta;

            if (t.localPosition.y < -limit)
            {
                t.localPosition += Vector3.up * config.totalCards * config.symbolHeight;

                if (!resultInjected)
                    AssignRandom(card);
            }
        }

        if (resultLocked && !resultInjected && remainingDistance <= config.injectDistance)
            InjectFinalCards();

        if (remainingDistance <= 0f)
            FinishStop();
    }

    // -----------------------------
    // RESULT CONTROL
    // -----------------------------

    public void SetForcedResult(int top, int center, int bottom)
    {
        topSymbol = top;
        centerSymbol = center;
        bottomSymbol = bottom;

    }

    void InjectFinalCards()
    {
        int center = config.totalCards / 2;

        AssignForced(cards[center - 1], topSymbol);
        AssignForced(cards[center], centerSymbol);
        AssignForced(cards[center + 1], bottomSymbol);

        resultInjected = true;
    }

    void LockResult()
    {
        resultLocked = true;
    }

    void CalculateStopDistance()
    {
        int fullLoops = 3;
        float loopDistance = config.totalCards * config.symbolHeight;

        float centerOffset =
            Mathf.Abs(cards[config.totalCards / 2].transform.localPosition.y);

        remainingDistance = fullLoops * loopDistance + centerOffset;
    }

    // -----------------------------
    // FINISH
    // -----------------------------

    public int[] GetVisibleSymbols()
    {
        // Order: top, center, bottom
        return new int[]
        {
        topSymbol,
        centerSymbol,
        bottomSymbol
        };
    }

    // public Transform[] GetVisibleSymbolTransforms()
    // {
    //     int centerIndex = config.totalCards / 2;

    //     // Order: TOP, CENTER, BOTTOM
    //     return new Transform[]
    //     {
    //     cards[centerIndex - 1].transform, // top
    //     cards[centerIndex].transform,     // center
    //     cards[centerIndex + 1].transform  // bottom
    //     };
    // }
    public Card[] GetVisibleCards()
    {
        int centerIndex = config.totalCards / 2;

        // Order: TOP, CENTER, BOTTOM
        return new Card[]
        {
        cards[centerIndex - 1], // top
        cards[centerIndex],     // center
        cards[centerIndex + 1]  // bottom
        };
    }

    
    void FinishStop()
    {
        isSpinning = false;
        currentSpeed = 0f;
        SnapToGrid();
    }

    void SnapToGrid()
    {
        foreach (var card in cards)
        {
            Vector3 pos = card.transform.localPosition;
            float snappedY =
                Mathf.Round(pos.y / config.symbolHeight) * config.symbolHeight;

            card.transform.localPosition = new Vector3(0f, snappedY, 0f);
        }
        ReelsManager.Instance.NotifyReelStopped();
    }

    // -----------------------------
    // CARD ASSIGNMENT
    // -----------------------------
    void AssignRandom(Card card)
    {
        CardConfig cfg =
            ReelsManager.Instance.cardDatabase.GetRandom().config;

        card.ApplyConfig(cfg);
    }

    void AssignForced(Card card, int cardId)
    {
        CardConfig cfg =
            ReelsManager.Instance.cardDatabase.GetById(cardId).config;

        card.ApplyConfig(cfg);
    }

    // -----------------------------
    // CLEANUP
    // -----------------------------
    public void Dispose()
    {
        foreach (var card in cards)
            ReelsManager.Instance.ReturnCard(card);

        cards.Clear();
    }
}
