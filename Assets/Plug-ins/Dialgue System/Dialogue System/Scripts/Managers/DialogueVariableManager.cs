using UnityEngine;

namespace DialogueSystem
{
    public class DialogueVariableManager : MonoBehaviour
    {
        public static DialogueVariableManager Instance { get; private set; }

        [SerializeField]
        private bool dontDestroyOnLoad = false;

        public DictionarySerializable<DialogueIntVariable, ReturnEvent<int>> intVariables;
        public DictionarySerializable<DialogueFloatVariable, ReturnEvent<float>> floatVariables;
        public DictionarySerializable<DialogueStringVariable, ReturnEvent<string>> stringVariables;
        public DictionarySerializable<DialogueBoolVariable, ReturnEvent<bool>> boolVariables;

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
    }
}
