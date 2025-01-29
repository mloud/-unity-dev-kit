using System.Collections.Generic;

namespace OneDay.Core.Game.BehaviourTree
{
    public class AndSequence : Node
    {
        public AndSequence(List<Node> children, Tree tree) : base(children, tree) { }

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