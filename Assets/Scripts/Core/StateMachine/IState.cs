using UnityEngine;

namespace Core.StateMachine
{
    public interface IState<TContext>
    {
        void Enter();
        void Exit();
        void Update();
        void FixedUpdate();
        void HandleInput(); // Add this line
    }
}
