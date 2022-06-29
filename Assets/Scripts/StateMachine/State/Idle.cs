using Enums;
using Interfaces;
using UnityEngine;

namespace StateMachine.State
{
    public class Idle : IState
    {
        public States Action(States state)
        {
            if (state != States.dashing)
                return IdleAction();
            return state;
        }

        private States IdleAction()
        {
            return States.idle;
        }
    }
}
