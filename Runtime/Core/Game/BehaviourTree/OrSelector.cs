using System.Collections.Generic;

namespace OneDay.Core.Game.BehaviourTree
{
    public class OrSelector : Node
    {
        private bool continueAfterSuccess;

        public OrSelector(List<Node> children, Tree tree, bool continueAfterSuccess = false) 
            : base(children, tree)
        {
            this.continueAfterSuccess = continueAfterSuccess;
        }
       
        protected override NodeState OnEvaluate()
        {
            foreach (var node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.Failure:
                        continue;
                    case NodeState.Running:
                        state = NodeState.Running;
                        return state;
                    case NodeState.Success:
                        state = NodeState.Success;
                        if (continueAfterSuccess)
                        {
                            break;
                        }
                        return state;
                }
            }

            state = NodeState.Failure;
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