using UnityEngine;
using Core.Config;

namespace Core.Enemy
{
    [CreateAssetMenu(fileName = "EnemyExperienceConfig", menuName = "Config/EnemyExperienceConfig")]
    public class EnemyExperienceConfig : ConfigBase
    {
        [SerializeField] private int experienceReward = 100;

        public int ExperienceReward => experienceReward;

        protected override void ValidateFields()
        {
            if (experienceReward <= 0)
            {
                Debug.LogError($"{nameof(experienceReward)} must be greater than zero");
                experienceReward = 1;
            }
        }
    }
}
