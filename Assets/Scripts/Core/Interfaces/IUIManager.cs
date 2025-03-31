namespace Core.UI
{
    public interface IUIManager
    {
        void UpdateHealthUI(int currentHealth, int maxHealth); 
        void UpdateExperienceUI(int experience);
    }
}
