using System;

namespace TwoDShooting.Proto.Domain
{
    public enum MovementMode
    {
        Free,
        HorizontalOnly,
        VerticalOnly
    }

    public enum BulletTrajectory
    {
        Straight,
        SineWave
    }

    public sealed class DamageableModel
    {
        public event Action<int> HealthChanged;
        public event Action Defeated;

        public int MaxHealth { get; }
        public int CurrentHealth { get; private set; }
        public bool IsDefeated => CurrentHealth <= 0;

        public DamageableModel(int maxHealth)
        {
            MaxHealth = Math.Max(1, maxHealth);
            CurrentHealth = MaxHealth;
        }

        public void ApplyDamage(int damage)
        {
            if (IsDefeated)
            {
                return;
            }

            CurrentHealth = Math.Max(0, CurrentHealth - Math.Max(0, damage));
            HealthChanged?.Invoke(CurrentHealth);

            if (IsDefeated)
            {
                Defeated?.Invoke();
            }
        }
    }

    public sealed class GameProgressModel
    {
        public int RequiredDefeats { get; }
        public int DefeatedEnemies { get; private set; }
        public bool IsComplete => DefeatedEnemies >= RequiredDefeats;

        public GameProgressModel(int requiredDefeats)
        {
            RequiredDefeats = Math.Max(1, requiredDefeats);
        }

        public void AddDefeat()
        {
            if (!IsComplete)
            {
                DefeatedEnemies++;
            }
        }
    }
}
