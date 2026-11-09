using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace DialogueSystem
{
    public class EventSystemCreator : MonoBehaviour
    {
        void Awake()
        {
            if (FindFirstObjectByType<EventSystem>() == null)
            {
#if ENABLE_INPUT_SYSTEM
                GameObject newEventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
#else
                GameObject oldEventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
#endif
            }
        }
    }
}
