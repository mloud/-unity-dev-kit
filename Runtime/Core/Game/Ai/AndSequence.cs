namespace OneDay.Core.Game.Ai
{
    public class AndSequence : Node
    {
        public AndSequence(params Node[] children) 
            : base(children) { }
        
        protected override NodeState OnEvaluate()
        {
            foreach (var node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.Failure:
                        state = NodeState.Failure;
                        return state;
                    case NodeState.Running:
                        state = NodeState.Running;
                        return state;
                    case NodeState.Success:
                        state = NodeState.Success;
                        continue;
                }
            }
            state = NodeState.Success;
            return state;
        }
        
        public override void OnReset(ResetReason reason)
        {
            foreach (Node child in Children)
            {
                child.OnReset(ResetReason.CompositeNodeFinished);
            }
        }
    }
}