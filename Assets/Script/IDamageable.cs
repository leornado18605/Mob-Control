public interface IDamageable
{
    void TakeDamage(int dmg);
    int  CurrentHealth { get; }
}