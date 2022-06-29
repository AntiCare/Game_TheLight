using System;
using Enums;

namespace Quests.Objectives.Events
{
    public static class KillEnemyEvent
    {
        public static event Action<EnemyType> OnKill;
        
        public static void Kill(EnemyType enemyType)
        {
            OnKill?.Invoke(enemyType);
        }
    }
}