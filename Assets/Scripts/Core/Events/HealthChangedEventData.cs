public class HealthChangedEventData
{
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }

    public HealthChangedEventData(int currentHealth, int maxHealth)
    {
        CurrentHealth = currentHealth;
        MaxHealth = maxHealth;
    }
}
