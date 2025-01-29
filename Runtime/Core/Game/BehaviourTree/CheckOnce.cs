namespace OneDay.Core.Game.BehaviourTree
{
    public class CheckOnce : Node
    {
        private readonly Node childNode;
        private bool hasCheckedOnce;
        private NodeState cachedResult = NodeState.Ready;

        public CheckOnce(Node nodeToCheck, Tree tree) : base(tree)
        {
            childNode = nodeToCheck;
            Attach(nodeToCheck);
        }

        protected override NodeState OnEvaluate()
        {
            if (!hasCheckedOnce)
            {
                cachedResult = childNode.Evaluate();
                hasCheckedOnce = true;
            }
        
            return cachedResult;
        }

        public override void OnReset(ResetReason reason)
        {
            if (reason == ResetReason.CompositeNodeFinished)
            {
                hasCheckedOnce = false;
                cachedResult = NodeState.Ready;
            }
        }
    }
}