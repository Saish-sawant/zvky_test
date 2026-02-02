using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BootState : IState
{
    private GameManager game;
    private MonoBehaviour runner;
    private AssetLoader loader;

    public BootState(GameManager game, MonoBehaviour runner)
    {
        this.game = game;
        this.runner = runner;
        loader = new AssetLoader();
    }

    public void Enter()
    {
        runner.StartCoroutine(BootRoutine());
    }

    IEnumerator BootRoutine()
    {
        // 1️⃣ Load Splash
        yield return SceneManager.LoadSceneAsync("Splash");

        // 2️⃣ Start loading assets
        IEnumerator loadRoutine = loader.LoadAllAssets();
        while (loadRoutine.MoveNext())
        {
            game.UpdateLoadProgress(loader.Progress);
            yield return loadRoutine.Current;
        }

        // 3️⃣ Load main game scene
        yield return SceneManager.LoadSceneAsync("Game");

        // 4️⃣ Reset progress (optional)
        game.UpdateLoadProgress(1f);

        // 5️⃣ Enter PreGame
        game.FSM.ChangeState(game.PreGameState);
    }

    public void Exit() { }
}
