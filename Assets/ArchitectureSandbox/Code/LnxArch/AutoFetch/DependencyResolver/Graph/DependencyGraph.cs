using System;
using System.Collections.Generic;
using System.Linq;

namespace LnxArch
{
    public class DependencyGraph<T>
    {
        private readonly Dictionary<T, DependencyNode<T>> _nodes = new();

        public void AddPair(T origin, T dependency)
        {
            NodeFor(origin).AddDependency(NodeFor(dependency));
        }

        private DependencyNode<T> NodeFor(T key)
        {
            if (!_nodes.ContainsKey(key))
            {
                _nodes[key] = new DependencyNode<T>(key);
            }
            return _nodes[key];
        }

        public DependencyNode<T> GetNodeFor(T key)
        {
            return _nodes.GetValueOrDefault(key, null);
        }

        public void DoBreadthFirstTraversalForEachParentlessNode(System.Action<T, int> visit)
        {
            if (_nodes.Count == 0) return;
            IEnumerable<DependencyNode<T>> parentlessNodes = 
                _nodes
                .Values
                .Where(n => !n.Dependents.Any());

            if (!parentlessNodes.Any())
            {
                UnityEngine.Debug.LogError($"LnxArch: All nodes in the dependency graph has a parent, which means there's a circular dependency.");
                _nodes.Values.FirstOrDefault().BreadthFirstTraversal((_, _) => {});
            }

            foreach (DependencyNode<T> node in parentlessNodes)
            {
                node.BreadthFirstTraversal(visit);
            }
        }

        public IEnumerable<T> EnumerateBranchFromBottomUpTo(T key)
        {
            DependencyNode<T> searchRootNode = GetNodeFor(key);
            if (searchRootNode == null) {
                yield return key;
                yield break;
            }
            Stack<DependencyNode<T>> stack = new();
            Queue<DependencyNode<T>> queue = new();
            queue.Enqueue(searchRootNode);
            while (queue.Count != 0)
            {
                DependencyNode<T> node = queue.Dequeue();
                stack.Push(node);
                foreach (var dependency in node.Dependencies)
                {
                    queue.Enqueue(dependency);
                }
            }
            foreach (var node in stack)
            {
                yield return node.Key;
            }
        }
    }

    public class TraversalVisit<T>
    {
        public DependencyNode<T> Node;
        public int Depth;
        public TraversalVisit<T> PreviousVisit;

        public void Deconstruct(out DependencyNode<T> node, out int depth)
        {
            node = Node;
            depth = Depth;
        }

        public IEnumerable<T> BackTrack()
        {
            TraversalVisit<T> current = this;
            while (current != null)
            {
                yield return current.Node.Key;
                current = current.PreviousVisit;
            }
        }
    }

    public class DependencyNode<T>
    {
        public List<DependencyNode<T>> Dependents {get; private set;} = new();
        public List<DependencyNode<T>> Dependencies {get; private set;} = new();
        public T Key {get; private set;}

        public DependencyNode(T key)
        {
            Key = key;
        }

        public void AddDependency(DependencyNode<T> node)
        {
            Dependencies.Add(node);
            node.Dependents.Add(this);
        }

        public void BreadthFirstTraversal(Action<T, int> visit)
        {
            HashSet<DependencyNode<T>> visited = new();
            Queue<TraversalVisit<T>> queue = new();
            queue.Enqueue(new TraversalVisit<T>{Node=this, Depth=0});
            while (queue.Any())
            {
                TraversalVisit<T> visitData = queue.Dequeue();
                (DependencyNode<T> node, int depth) = visitData;
                if (visited.Contains(node))
                {
                    string trackStr = string.Join("\n", visitData.BackTrack().Reverse());
                    UnityEngine.Debug.LogError($"LnxArch circular dependency: \n{trackStr}");
                    throw new LnxCircularDependencyException();
                }

                visit(node.Key, depth);
                visited.Add(node);

                foreach (DependencyNode<T> dependency in node.Dependencies)
                {
                    queue.Enqueue(new TraversalVisit<T>{Node=dependency, Depth=depth + 1, PreviousVisit=visitData});
                }
            }
        }
    }
}