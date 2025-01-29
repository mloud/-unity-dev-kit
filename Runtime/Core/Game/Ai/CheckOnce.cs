namespace OneDay.Core.Game.Ai
{
    public class CheckOnce : Node
    {
        private readonly Node childNode;
        private bool hasCheckedOnce;
        private NodeState cachedResult = NodeState.Ready;

        public CheckOnce(Node nodeToCheck, BehaviourTree behaviourTree) : base(behaviourTree)
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