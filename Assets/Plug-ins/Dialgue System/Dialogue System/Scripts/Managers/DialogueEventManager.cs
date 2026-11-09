using UnityEngine;
using UnityEngine.Events;

namespace DialogueSystem
{
    public class DialogueEventManager : MonoBehaviour
    {
        public static DialogueEventManager Instance { get; private set; }

        [SerializeField]
        private bool dontDestroyOnLoad = false;

        public DictionarySerializable<DialogueEvent, UnityEvent> events;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                if (dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Invoke(DialogueEvent dialogueEvent)
        {
            UnityEvent unityEvent = events[dialogueEvent];
            if (unityEvent != null)
            {
                unityEvent.Invoke();
            }
        }
    }
}
