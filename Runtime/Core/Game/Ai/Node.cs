using System.Collections.Generic;
using UnityEngine;

namespace OneDay.Core.Game.Ai
{
    public enum NodeState
    {
        Ready,
        Running,
        Success,
        Failure
    }
        
    public abstract class Node
    {
        public Node Parent { get; private set; }
        public BehaviourTree BehaviourBehaviourTree { get; private set; }
        public IReadOnlyList<Node> Children => children;

        protected NodeState state;
        protected List<Node> children = new();
  
        protected Node()
        {
            Parent = null;
            BehaviourBehaviourTree = null;
        }

        public void SetBehaviourTree(BehaviourTree tree)
        {
            BehaviourBehaviourTree = tree;
            children.ForEach(x=>SetBehaviourTree(tree));
        }
        
        protected TreeContext GetTreeContext()
        {
            if (BehaviourBehaviourTree != null)
                return BehaviourBehaviourTree.DataContext;
           
            var node = Parent;
            while (node != null)
            {
                if (node.BehaviourBehaviourTree != null)
                    return node.BehaviourBehaviourTree.DataContext;

                node = node.Parent;
            }

            return null;
        }
        
        protected Node(params Node[] children)
        {
            state = NodeState.Ready;
            foreach (var node in children)
            {
                Attach(node);
            }
        }

        protected void Attach(Node node)
        {
            children.Add(node);
            node.Parent = this;
        }


        // perframe check if node is still valid
        protected virtual bool OnPerformAlwaysCheck() => true;
        public virtual void OnReset(ResetReason reason) {}
    
        protected virtual void OnEnter() {}
        
        private void Reset(ResetReason reason)
        {
            OnReset(reason);
            state = NodeState.Ready;
        }
        
        public NodeState Evaluate()
        {
            if (!OnPerformAlwaysCheck())
            {
                Reset(ResetReason.Fail);
                return NodeState.Failure;
            }

            if (state == NodeState.Ready)
            {
                OnEnter();
                state = NodeState.Running;
            }
            
            state = OnEvaluate();
            //Debug.Log($"[BT] Tree: {BehaviourBehaviourTree.Id} OnEvaluate of {this.GetType()} is  {state}");
            Debug.Assert(state != NodeState.Ready, "Node should not return Ready state");
            switch (state)
            {
                case NodeState.Failure:
                    Reset(ResetReason.Fail);
                    return NodeState.Failure;
                case NodeState.Success:
                    Reset(ResetReason.Success);
                    return NodeState.Success;
                default:
                    return NodeState.Running;
            }
           
        }
        protected virtual NodeState OnEvaluate() => NodeState.Failure;

        public virtual string Log() => GetType().ToString();
    }
}