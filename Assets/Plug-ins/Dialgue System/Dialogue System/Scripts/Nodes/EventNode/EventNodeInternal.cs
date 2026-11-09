namespace DialogueSystem
{
    public partial class EventNodeInternal : NodeInternal, EventNode
    {
        public DialogueEvent dialogueEvent;

        public NodeInternal child;

#if UNITY_EDITOR
        public void OnDialogueEventChanged(DialogueEvent dialogueEvent)
        {
            this.dialogueEvent = dialogueEvent;
            Save();
        }

        public void OnChildChanged(NodeInternal child)
        {
            this.child = child;
            Save();
        }

        public override void CopyFrom(NodeInternal other)
        {
            base.CopyFrom(other);
            EventNodeInternal otherEventNode = other as EventNodeInternal;
            if (otherEventNode != null)
            {
                dialogueEvent = otherEventNode.dialogueEvent;
            }
        }

        public override void Remove() { }
#endif

        DialogueEvent EventNode.DialogueEvent
        {
            get
            {
                return dialogueEvent;
            }
        }
    }
}
