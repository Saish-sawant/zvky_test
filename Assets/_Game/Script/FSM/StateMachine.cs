namespace SpinWheel
{
    public class StateMachine
    {
        public IState CurrentState { get; private set; }

        public void ChangeState(IState nextState)
        {
            if (CurrentState == nextState)
                return;

            CurrentState?.Exit();
            CurrentState = nextState;
            CurrentState.Enter();
        }
    }
    public interface IState
    {
        void Enter();
        void Exit();
    }
}