using UnityEngine;

public class PostGameState : IState
{
    private GameManager game;

    public PostGameState(GameManager game)
    {
        this.game = game;
    }

    public void Enter()
    {
        Debug.Log("🏁 PostGame: Show Result");
        // Show payout / ads
    }

    public void Exit()
    {
        // Hide result UI
    }
}
