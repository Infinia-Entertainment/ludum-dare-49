namespace Wizard
{
    public interface IDamageable
    {
        public int CurrentHealth { get; }
        public int MaxHealth { get; }
        void OnDamage(int damage);
        void OnDeath();
    }
}