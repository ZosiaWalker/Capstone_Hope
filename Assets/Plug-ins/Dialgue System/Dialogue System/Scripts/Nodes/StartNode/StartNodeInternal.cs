namespace DialogueSystem
{
    public class StartNodeInternal : NodeInternal
    {
        public NodeInternal child;

#if UNITY_EDITOR
        public void OnChildChanged(NodeInternal child)
        {
            this.child = child;
            Save();
        }

        public override void Remove() { }
#endif
    }
}
