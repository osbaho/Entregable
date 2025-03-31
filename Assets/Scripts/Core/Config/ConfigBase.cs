using UnityEngine;

namespace Core.Config
{
    public abstract class ConfigBase : ScriptableObject
    {
        protected virtual void OnValidate()
        {
            ValidateFields();
        }

        protected abstract void ValidateFields();

        protected void ValidateGreaterThanZero(ref float value, string fieldName, float defaultValue = -1f)
        {
            if (value <= 0)
            {
                if (defaultValue > 0)
                {
                    Debug.LogWarning($"{fieldName} in {name} must be greater than zero. Resetting to {defaultValue}.");
                    value = defaultValue;
                }
                else
                {
                    Debug.LogError($"{fieldName} in {name} must be greater than zero.");
                }
            }
        }
    }
}
