using UnityEngine;

namespace OneDay.Core.Game.BehaviourTree
{
    namespace BehaviourTree
    {
        public class RunOnce : Node
        {
            private readonly Node childNode;
            private bool finished;
         

            public RunOnce(Node nodeToCheck, Tree tree) : base(tree)
            {
                finished = false;
                childNode = nodeToCheck;
                Attach(nodeToCheck);
            }

            protected override NodeState OnEvaluate()
            {
                if (!finished)
                {
                    var result = childNode.Evaluate();
                    if (result == NodeState.Success)
                    {
                        finished = true;
                    }
                    return result;
                }
       
                return NodeState.Success;
            }

            public override void OnReset(ResetReason reason)
            {
                if (reason == ResetReason.CompositeNodeFinished)
                {
                    finished = false;
                }
            }

            public override string Log() =>
                $"RunsOnceDecorator - {childNode.Log()}";
        }
    }
}