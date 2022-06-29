using Enums;
using Interfaces;

namespace StateMachine.State
{
    public class Dead : IState
    {
        private bool _isDead;
        public Dead(Player p)
        {
            p.PlayerDied.AddListener(Die);
        }
        public States Action(States state)
        {
            if (_isDead)
            {
                _isDead = false;
                return States.dead;
            }

            return state;
        }
        private void Die(Player player)
        {
            _isDead = true;
        }
    }
}