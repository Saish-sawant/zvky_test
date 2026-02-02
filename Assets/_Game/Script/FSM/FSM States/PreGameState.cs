using UnityEngine;

public class PreGameState : IState
{
    private GameManager game;

    public PreGameState(GameManager game)
    {
        this.game = game;
    }

    public void Enter()
    {
        Debug.Log("💰 PreGame: Place Bet");
        // Enable bet UI
    }

    public void Exit()
    {
        // Disable bet UI
    }
}
