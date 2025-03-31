namespace Core.Events
{
    public struct HealthChangedEventData
    {
        public float CurrentHealth;
        public float MaxHealth;

        public HealthChangedEventData(float current, float max)
        {
            CurrentHealth = current;
            MaxHealth = max;
        }
    }

    public struct ExperienceChangedEventData
    {
        public int Experience;

        public ExperienceChangedEventData(int exp)
        {
            Experience = exp;
        }
    }
}
