using UnityEngine;

namespace Core.Events
{
    public class CharacterDeathEventData
    {
        public GameObject Character { get; private set; }

        public CharacterDeathEventData(GameObject character)
        {
            Character = character;
        }
    }
}
