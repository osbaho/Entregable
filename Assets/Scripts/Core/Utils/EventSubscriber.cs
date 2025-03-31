using UnityEngine;

namespace Core.Utils
{
    public abstract class EventSubscriber : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            SubscribeToEvents();
        }

        protected virtual void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        protected abstract void SubscribeToEvents();
        protected abstract void UnsubscribeFromEvents();
    }
}
