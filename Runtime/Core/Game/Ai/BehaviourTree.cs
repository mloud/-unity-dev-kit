using UnityEngine;

namespace OneDay.Core.Game.Ai
{
    public abstract class BehaviourTree : MonoBehaviour
    {
        public string Id { get; protected set; }
        public TreeContext DataContext { get; private set; } = new();
        public void SetActiveNode(Node node) => activeNode = node;
        
        private Node root;
        private Node activeNode;
        
        
        private void Start()
        {
            root = SetupTree();
        }
        
        public void Update()
        {
            if (activeNode != null)
            {
                var state = activeNode.Evaluate();
                if (state is NodeState.Failure or NodeState.Success)
                {
                    activeNode = null;
                }
            }
            else
            {
                root?.Evaluate();
            }
        }
  
        protected abstract Node SetupTree();
    }
}