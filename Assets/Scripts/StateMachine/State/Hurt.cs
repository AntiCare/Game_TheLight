using Enums;
using Interfaces;

namespace StateMachine.State
{
    public class Hurt : IState
    {
        private bool     _isHurt;
        private Cooldown _cooldown;

        public Hurt(Player player, float cooldown)
        {
            _cooldown = new Cooldown(cooldown);
            player.PlayerTakeDamage.AddListener(PlayerHurt);
        }


        public States Action(States state)
        {
            _cooldown.CooldownTick();
            if (_isHurt && !_cooldown.isOnCooldown())
            {   
                _isHurt = false;
                return States.hurt;
            }

            return state;
        }


        void PlayerHurt(Player player)
        {
            _isHurt = true;
        }
    }
}