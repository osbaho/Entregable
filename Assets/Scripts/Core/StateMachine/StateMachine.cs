// Assets/Scripts/Core/StateMachine/StateMachine.cs
using System;
using System.Collections.Generic;

namespace Core.StateMachine
{
    public class StateMachine<TEnum, TContext> where TEnum : Enum
    {
        private readonly TContext context;
        private readonly Dictionary<TEnum, IState<TContext>> states = new Dictionary<TEnum, IState<TContext>>();
        public IState<TContext> CurrentState { get; private set; }

        public StateMachine(TContext context)
        {
            this.context = context;
        }

        public void AddState(TEnum stateEnum, IState<TContext> state)
        {
            states.Add(stateEnum, state);
        }

        public void ChangeState(TEnum newStateEnum)
        {
            if (CurrentState != null)
            {
                CurrentState.Exit();
            }

            CurrentState = states[newStateEnum];
            CurrentState.Enter();
        }

        public void UpdateState()
        {
            CurrentState?.Update();
        }

        public void FixedUpdateState()
        {
            CurrentState?.FixedUpdate();
        }
    }
}
