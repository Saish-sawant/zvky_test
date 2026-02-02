using UnityEngine;

public class InGameState : IState
{
    private GameManager game;

    public InGameState(GameManager game)
    {
        this.game = game;
    }

    public void Enter()
    {
        Debug.Log("🎰 InGame: Spin Started");

        // Start spin here
        // When spin ends -> game.OnSpinEnded();
    }

    public void Exit()
    {
        // Stop input
    }
}
