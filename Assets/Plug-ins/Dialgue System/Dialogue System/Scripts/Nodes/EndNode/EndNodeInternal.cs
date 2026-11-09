namespace DialogueSystem
{
    public partial class EndNodeInternal : NodeInternal, EndNode
    {
        public DialogueTreeSO nextDialogue;

#if UNITY_EDITOR
        public void OnNextDialogueChanged(DialogueTreeSO nextDialogue)
        {
            this.nextDialogue = nextDialogue;
            Save();
        }

        public override void CopyFrom(NodeInternal other)
        {
            base.CopyFrom(other);
            EndNodeInternal otherEndNode = other as EndNodeInternal;
            if (otherEndNode != null)
            {
                nextDialogue = otherEndNode.nextDialogue;
            }
        }

        public override void Remove() { }
#endif

        DialogueTreeSO EndNode.NextDialogue
        {
            get
            {
                //if (nextDialogue == null && tree != null)
                //{
                //    string fieldName = "Next Dialogue";
                //    Logger.LogNullFieldWarning(tree.name, name, fieldName);
                //    return null;
                //}
                return nextDialogue;
            }
        }
    }
}
