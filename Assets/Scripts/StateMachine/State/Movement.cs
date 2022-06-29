using Enums;
using Interfaces;
using UnityEngine;

namespace StateMachine.State
{
    public class Movement : IState
    {
        private Rigidbody2D _body;
        private float _runspeed = 10.0f;
        private Animator _animator;

        public Movement(Rigidbody2D body, float runspeed, Animator animator)
        {
            this._body = body;
            this._runspeed = runspeed;
            this._animator = animator;
        }

        public States Action(States state)
        {
            if (state == States.idle && ValidInput() && !AnimatorBusy())
                state = MoveAction();
            

            return state;
        }

        private bool ValidInput()
        {
            return !(Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0);
        }

        private bool AnimatorBusy()
        {
            return _animator.GetCurrentAnimatorStateInfo(0).IsName("Player_hurt") ||
                   _animator.GetCurrentAnimatorStateInfo(0).IsName("Player_death");
        }

        private States MoveAction()
        {
            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);

            _body.MovePosition(_body.transform.position + input * Time.deltaTime * _runspeed);

            return States.moving;
        }
    }
}