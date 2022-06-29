using Enums;
using Interfaces;
using UnityEngine;

namespace StateMachine.State
{
    public class Dash : IState
    {
        private float[] _directions = new float[2];
        private Cooldown _cooldown;
        private Rigidbody2D _body;
        private float _remainingDash;
        private float _dashDistance;
        private float _dashSpeed;
        private Animator _animator;

        public Dash(Rigidbody2D body, float dashSpeed, float dashDistance, float dashCooldown, Animator animator)
        {
            _body = body;
            _dashSpeed = dashSpeed;
            _dashDistance = dashDistance;
            _animator = animator;
            _cooldown = new Cooldown(dashCooldown * 50);
        }

        public States Action(States state)
        {
            _cooldown.CooldownTick();
            if (!AnimatorBusy())
            {
                if (state == States.dashing)
                    return ContinueDash(state);

                if (Input.GetKey(KeyCode.LeftShift) && !_cooldown.isOnCooldown())
                    return StartDash();
            }

            return state;
        }

        private bool AnimatorBusy()
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Player_hurt") ||
                _animator.GetCurrentAnimatorStateInfo(0).IsName("Player_death"))
            {
                _remainingDash = 0f;
                return true;
            }
            return false;
    }
        private States StartDash()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            if (horizontal == 0f && vertical == 0f)
                return States.idle;
            
            _remainingDash = _dashDistance;
            _directions[0] = horizontal;
            _directions[1] = vertical;
            return DashAction(_directions[0], _directions[1]);


        }
        
        private States ContinueDash(States state)
        {
            if (_remainingDash <= 0f)
                return EndDash();
            return DashAction(_directions[0],_directions[1]);

        }
        private States EndDash()
        {
            _cooldown.resetCooldown();
            _remainingDash = 0f;
            return States.idle;
        }
        private States DashAction(float hor, float ver)
        {
         
            float dashDelta = Time.deltaTime * _dashSpeed;
            _remainingDash -= dashDelta;

            Vector3 input = new Vector3(hor * _dashSpeed, ver * _dashSpeed, 0);
            
            _body.MovePosition(_body.transform.position + input  * Time.deltaTime);

            return States.dashing;
        }
    }
}