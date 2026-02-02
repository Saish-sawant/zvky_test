using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public StateMachine FSM { get; private set; }

    // States
    public IState BootState { get; private set; }
    public IState PreGameState { get; private set; }
    public IState InGameState { get; private set; }
    public IState PostGameState { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        FSM = new StateMachine();

        BootState = new BootState(this, this);
        PreGameState = new PreGameState(this);
        InGameState = new InGameState(this);
        PostGameState = new PostGameState(this);
    }

    void Start()
    {
        FSM.ChangeState(BootState);
    }

    // ---- Shared helpers ----

    public IEnumerator LoadAssets()
    {
        // Addressables / SDK init later
        yield return new WaitForSeconds(2f);
    }

    // ---- UI hooks ----

    public void OnSpinPressed()
    {
        FSM.ChangeState(InGameState);
    }

    public void OnSpinEnded()
    {
        FSM.ChangeState(PostGameState);
    }

    public void OnPlayAgain()
    {
        FSM.ChangeState(PreGameState);
    }
    public float LoadProgress { get; private set; }

    public void UpdateLoadProgress(float value)
    {
        LoadProgress = value;
    }

}
