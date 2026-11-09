using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace DialogueSystem
{
    public class SampleDialogueUIManagerExample : DialogueUIManager
    {
        [SerializeField]
        private DialogueTreeSO currentDialogue;

        [Header("Dialogue UI")]
        public GameObject dialogueUI;
        public TextMeshProUGUI dialogueText;
        public Button nextButton;

        [Header("Choice Node UI")]
        public GameObject choicesContainer;
        public GameObject choicePrefab;

        private Language language = Language.English;

        void Update()
        {
#if ENABLE_INPUT_SYSTEM
            // New Input System: Check if spacebar was pressed this frame
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                TryStartDialogue();
            }
#else
            // Old Input System
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TryStartDialogue();
            }
#endif
        }

        private void OnEnable()
        {
            nextButton.onClick.AddListener(NextNode);
        }

        private void OnDisable()
        {
            nextButton.onClick.RemoveListener(NextNode);
        }

        private void TryStartDialogue()
        {
            if (!dialogueManager.DialogueStarted)
            {
                dialogueManager.StartDialogue(currentDialogue);
                dialogueUI.SetActive(true);
                nextButton.gameObject.SetActive(true);
            }
        }

        private void NextNode()
        {
            if (dialogueManager.DialogueStarted && dialogueManager.CurrentNode is not ChoiceNode)
            {
                dialogueManager.NextNode();
            }
        }

        public override void OnDialogueNode(DialogueNode dialogueNode)
        {
            dialogueText.text = dialogueNode.GetMessage(language);
        }

        public override void OnChoiceNode(ChoiceNode choiceNode)
        {
            dialogueText.text = choiceNode.GetMessage(language);

            for (int i = 0; i < choiceNode.Choices.Count; i++)
            {
                GameObject choice = Instantiate(choicePrefab);
                choice.transform.SetParent(choicesContainer.transform, false);

                int index = i;
                Button button = choice.GetComponent<Button>();
                button.onClick.AddListener(() => OnChoiceClick(index));

                TextMeshProUGUI textMeshPro = choice.GetComponentInChildren<TextMeshProUGUI>();
                textMeshPro.text = choiceNode.Choices[index].GetMessage(language);
            }
        }

        private void OnChoiceClick(int index)
        {
            dialogueManager.NextNode(index);
            foreach (Transform child in choicesContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }

        public override void OnEventNode(EventNode eventNode, DialogueEventManager dialogueEventManager)
        {
            dialogueEventManager.Invoke(eventNode.DialogueEvent);
            NextNode();
        }

        public override void OnIfNode(IfNode ifNode)
        {
            NextNode();
        }

        public override void OnEndNode(EndNode endNode)
        {
            currentDialogue = endNode.NextDialogue;
            dialogueUI.SetActive(false);
            nextButton.gameObject.SetActive(false);
        }
    }
}