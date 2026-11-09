using System.Collections.Generic;

namespace DialogueSystem
{
    public class DialogueManagerInternal : DialogueManager
    {
        private DialogueUIManager dialogueUIManager;

        private bool dialogueStarted = false;
        bool DialogueManager.DialogueStarted
        {
            get { return dialogueStarted; }
        }

        private NodeInternal currentNode;
        Node DialogueManager.CurrentNode
        {
            get { return GetNodeInterface(currentNode); }
        }

        private DialogueEventManager dialogueEventManager;
        private DialogueVariableManager dialogueVariableManager;


        public DialogueManagerInternal(DialogueUIManager talkableHandler)
        {
            this.dialogueUIManager = talkableHandler;
        }

        public void StartDialogue(DialogueTreeSO dialogueTree)
        {
            StartDialogueInternal(dialogueTree);
        }

        public void StartDialogue(DialogueTreeSO dialogueTree, string nodeID)
        {
            StartDialogueInternal(dialogueTree, nodeID);
        }

        private void StartDialogueInternal(DialogueTreeSO dialogueTree, string nodeID = null)
        {
            if (dialogueTree == null)
            {
                string functionName = "StartDialogue";
                string parameterName = "dialogueTree";
                Logger.LogNullParameterPassedError(functionName, parameterName);
                return;
            }
            dialogueStarted = true;
            currentNode = GetPreviousNodeByGUID(dialogueTree, nodeID) ?? dialogueTree.startNode;

            if (currentNode is ChoiceNodeInternal)
            {
                ChoiceNodeInternal choiceNode = currentNode as ChoiceNodeInternal;
                for (int i = 0; i < choiceNode.choices.Count; i++)
                {
                    int index = i;
                    ChoiceInternal choice = choiceNode.choices[index];
                    if (choice.child.nodeID == nodeID)
                    {
                        NextNode(index);
                        break;
                    }
                }
            }
            else
            {
                NextNode();
            }
        }

        private NodeInternal GetPreviousNodeByGUID(DialogueTreeSO dialogueTree, string nodeID)
        {
            if (nodeID == null)
            {
                return null;
            }

            Queue<KeyValuePair<NodeInternal, NodeInternal>> queue = new Queue<KeyValuePair<NodeInternal, NodeInternal>>();
            queue.Enqueue(new KeyValuePair<NodeInternal, NodeInternal>(null, dialogueTree.startNode));
            while (queue.Count > 0)
            {
                var pair = queue.Dequeue();

                var previous = pair.Key;
                var current = pair.Value;

                if (current.nodeID == nodeID)
                {
                    return previous;
                }

                List<NodeInternal> children = dialogueTree.GetChildren(current);
                foreach (var child in children)
                {
                    queue.Enqueue(new KeyValuePair<NodeInternal, NodeInternal>(current, child));
                }
            }
            return null;
        }

        public void NextNode(int choice = -1)
        {
            if (!dialogueStarted)
            {
                Logger.LogNextNodeCalledBeforeStartDialogueError();
                return;
            }
            NodeInternal previousNode = currentNode;
            if (currentNode is StartNodeInternal)
            {
                StartNodeInternal startNode = currentNode as StartNodeInternal;
                currentNode = startNode.child;
            }
            else if (currentNode is DialogueNodeInternal)
            {
                DialogueNodeInternal dialogueNode = currentNode as DialogueNodeInternal;
                currentNode = dialogueNode.child;
            }
            else if (currentNode is ChoiceNodeInternal)
            {
                if (choice <= -1 || choice >= ((ChoiceNodeInternal)currentNode).choices.Count)
                {
                    string functionName = "NextNode";
                    string parameterName = "choice";
                    Logger.LogInvalidChoicePassedWarning(functionName, parameterName);
                    return;
                }
                ChoiceNodeInternal choiceNode = currentNode as ChoiceNodeInternal;
                currentNode = choiceNode.choices[choice].child;
            }
            else if (currentNode is EventNodeInternal)
            {
                EventNodeInternal eventNode = currentNode as EventNodeInternal;
                currentNode = eventNode.child;
            }
            else if (currentNode is IfNodeInternal)
            {
                IfNodeInternal ifNode = currentNode as IfNodeInternal;
                NodeInternal temp = null;
                foreach (IfPortSO ifPort in ifNode.conditions)
                {
                    if (ifPort.ContainsNullField())
                    {
                        continue;
                    }
                    else if (ifPort.variableType.RuntimeType == typeof(DialogueStringVariable))
                    {
                        NodeInternal nextNode = ProcessStringVariable(ifPort, ifPort.stringVariable);
                        if (nextNode)
                        {
                            temp = nextNode;
                            break;
                        }
                    }
                    else if (ifPort.variableType.RuntimeType == typeof(DialogueIntVariable))
                    {
                        NodeInternal nextNode = ProcessIntVariable(ifPort, ifPort.intVariable);
                        if (nextNode)
                        {
                            temp = nextNode;
                            break;
                        }
                    }
                    else if (ifPort.variableType.RuntimeType == typeof(DialogueFloatVariable))
                    {
                        NodeInternal nextNode = ProcessFloatVariable(ifPort, ifPort.floatVariable);
                        if (nextNode)
                        {
                            temp = nextNode;
                            break;
                        }
                    }
                    else if (ifPort.variableType.RuntimeType == typeof(DialogueBoolVariable))
                    {
                        NodeInternal nextNode = ProcessBoolVariable(ifPort, ifPort.boolVariable);
                        if (nextNode)
                        {
                            temp = nextNode;
                            break;
                        }
                    }
                }
                currentNode = temp != null ? temp : ifNode.elseChild;
            }

            if (currentNode == null)
            {
                currentNode = previousNode;
                Logger.LogEmptyNodeOutputError(currentNode.tree.name, currentNode.name);
                return;
            }
            else if (currentNode is EventNodeInternal && dialogueEventManager == null)
            {
                dialogueEventManager = DialogueEventManager.Instance;
                if (dialogueEventManager == null)
                {
                    currentNode = previousNode;
                    string objectName = "DialogueEventManager";
                    Logger.LogGameObjectNotFoundInSceneError(objectName);
                    return;
                }
            }
            else if (currentNode is IfNodeInternal && dialogueVariableManager == null)
            {
                dialogueVariableManager = DialogueVariableManager.Instance;
                if (dialogueVariableManager == null)
                {
                    currentNode = previousNode;
                    string objectName = "DialogueVariableManager";
                    Logger.LogGameObjectNotFoundInSceneError(objectName);
                    return;
                }
            }
            CallOnNodeFunction(currentNode);
            if (currentNode is EndNodeInternal)
            {
                currentNode = null;
                dialogueStarted = false;
            }
        }

        public void ExitDialogue()
        {
            currentNode = null;
            dialogueStarted = false;
        }

        private void CallOnNodeFunction(NodeInternal node)
        {
            switch (node)
            {
                case DialogueNodeInternal:
                    dialogueUIManager.OnDialogueNode(node as DialogueNode);
                    break;
                case ChoiceNodeInternal:
                    dialogueUIManager.OnChoiceNode(node as ChoiceNode);
                    break;
                case EventNodeInternal:
                    dialogueUIManager.OnEventNode(node as EventNode, dialogueEventManager);
                    break;
                case IfNodeInternal:
                    dialogueUIManager.OnIfNode(node as IfNode);
                    break;
                case EndNodeInternal:
                    dialogueUIManager.OnEndNode(node as EndNode);
                    break;
                default:
                    break;
            }
        }

        private NodeInternal ProcessStringVariable(IfPortSO ifPort, DialogueStringVariable variable)
        {
            if (!dialogueVariableManager.stringVariables.ContainsKey(variable))
            {
                Logger.LogKeyNotFoundInDictionaryWarning(variable.ToString(), dialogueVariableManager.name);
                return null;
            }
            ReturnEvent<string> returnEvent = dialogueVariableManager.stringVariables[variable];
            if (returnEvent != null)
            {
                string value = returnEvent.GetValue();
                if (ifPort.GetCondition(value))
                {
                    return ifPort.child;
                }
            }
            return null;
        }

        private NodeInternal ProcessIntVariable(IfPortSO ifPort, DialogueIntVariable variable)
        {
            if (!dialogueVariableManager.intVariables.ContainsKey(variable))
            {
                Logger.LogKeyNotFoundInDictionaryWarning(variable.ToString(), dialogueVariableManager.name);
                return null;
            }
            ReturnEvent<int> returnEvent = dialogueVariableManager.intVariables[variable];
            if (returnEvent != null)
            {
                int value = returnEvent.GetValue();
                if (ifPort.GetCondition(value))
                {
                    return ifPort.child;
                }
            }
            return null;
        }

        private NodeInternal ProcessFloatVariable(IfPortSO ifPort, DialogueFloatVariable variable)
        {
            if (!dialogueVariableManager.floatVariables.ContainsKey(variable))
            {
                Logger.LogKeyNotFoundInDictionaryWarning(variable.ToString(), dialogueVariableManager.name);
                return null;
            }
            ReturnEvent<float> returnEvent = dialogueVariableManager.floatVariables[variable];
            if (returnEvent != null)
            {
                float value = returnEvent.GetValue();
                if (ifPort.GetCondition(value))
                {
                    return ifPort.child;
                }
            }
            return null;
        }

        private NodeInternal ProcessBoolVariable(IfPortSO ifPort, DialogueBoolVariable variable)
        {
            if (!dialogueVariableManager.boolVariables.ContainsKey(variable))
            {
                Logger.LogKeyNotFoundInDictionaryWarning(variable.ToString(), dialogueVariableManager.name);
                return null;
            }
            ReturnEvent<bool> returnEvent = dialogueVariableManager.boolVariables[variable];
            if (returnEvent != null)
            {
                bool value = returnEvent.GetValue();
                if (ifPort.GetCondition(value))
                {
                    return ifPort.child;
                }
            }
            return null;
        }

        private Node GetNodeInterface(NodeInternal node)
        {
            if (node is DialogueNodeInternal)
            {
                return (DialogueNode)node;
            }
            else if (node is ChoiceNodeInternal)
            {
                return (ChoiceNode)node;
            }
            else if (node is EventNodeInternal)
            {
                return (EventNode)node;
            }
            else if (node is IfNodeInternal)
            {
                return (IfNode)node;
            }
            else if (node is EndNodeInternal)
            {
                return (EndNode)node;
            }
            return null;
        }
    }
}
