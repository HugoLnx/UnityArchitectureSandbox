using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LnxArch
{
    public class InvalidResolveTargetTypeException : Exception
    {
        public InvalidResolveTargetTypeException(string message) : base(message)
        {
        }

        public static InvalidResolveTargetTypeException BuildUnsupportedCollectionType()
        {
            return new("LnxEntity can resolve List and Array, but no other collection types are supported");
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
    }

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

        // TODO: public void ValidateCircularLoop()
    }

    public class LnxEntity : MonoBehaviour
    {
        private const bool DefaultIncludeInactive = true;
        private DependencyGraph<Type> _dependencyGraph;

        private void Awake()
        {
            _dependencyGraph ??= BuildDependencyGraph();
            ConstructBehaviours(FindObjectsByType<LnxBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None));
        }

        private static DependencyGraph<Type> BuildDependencyGraph()
        {
            Type[] behaviourTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && typeof(LnxBehaviour).IsAssignableFrom(t))
            .ToArray();

            DependencyGraph<Type> dependencyGraph = new();
            foreach (Type behaviourType in behaviourTypes)
            {
                // Debug.Log($"[GraphBuild:{behaviourType.Name}] Dependencies...");
                IEnumerable<Type> dependencyTypes = LnxBehaviour.GetDependencyTypes(behaviourType)
                .SelectMany(dependencyType =>
                    behaviourTypes.Where(behaviourType => dependencyType.IsAssignableFrom(behaviourType)))
                .Distinct();
                foreach (Type dependencyType in dependencyTypes)
                {
                    // Debug.Log($"[GraphBuild:{behaviourType.Name}][Dependency:{dependencyType.Name}] Add {behaviourType.Name} depends on {dependencyType.Name}");
                    dependencyGraph.AddPair(origin: behaviourType, dependency: dependencyType);
                }
            }
            return dependencyGraph;
        }

        private static IEnumerable<Type> GetAllTypesAssignableFrom(Type type)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(t => type.IsAssignableFrom(t));
        }

        private static IEnumerable<Type> GetAllTypesImplementedBy(Type type, Type inherithanceLimit = null)
        {
            yield return type;

            foreach (Type interfaceType in type.GetInterfaces())
            {
                yield return interfaceType;
            }

            foreach (Type ancestorType in GetAncestorsOf(type, fromBranch: inherithanceLimit))
            {
                yield return ancestorType;
            }
        }

        private static IEnumerable<Type> GetAncestorsOf(Type type, Type fromBranch = null)
        {
            Type baseType = type.BaseType;
            if (baseType == null) yield break;
            if (fromBranch == null || (fromBranch.IsAssignableFrom(baseType) && baseType != fromBranch))
            {
                yield return baseType;
            }

            foreach (Type ancestorType in GetAncestorsOf(baseType, fromBranch))
            {
                yield return ancestorType;
            }
        }

        private void ConstructBehaviours(LnxBehaviour[] behaviours)
        {
            Dictionary<Type, List<LnxBehaviour>> byType = behaviours
                .GroupBy(b => b.GetType())
                .ToDictionary(g => g.Key, g => g.ToList());

            Type[] behaviourTypes = byType.Keys.ToArray();
            foreach (Type behaviourType in behaviourTypes)
            {
                // Debug.Log($"[{behaviourType.Name}] Resolving Construction Dependencies");
                if (byType.GetValueOrDefault(behaviourType, null) == null) continue;
                foreach (Type branchBehaviourType in _dependencyGraph.EnumerateBranchFromBottomUpTo(behaviourType))
                {
                    // Debug.Log($"[{behaviourType.Name}] [OnBranch:{branchBehaviourType.Name}] Found");
                    if (byType.GetValueOrDefault(branchBehaviourType, null) == null) continue;
                    foreach (LnxBehaviour behaviour in byType[branchBehaviourType])
                    {
                        // Debug.Log($"[{behaviourType.Name}] [OnBranch:{branchBehaviourType.Name}] Trigger construction obj:{behaviour.gameObject.name}");
                        behaviour.TriggerConstruction();
                    }
                    byType[branchBehaviourType] = null;
                }
            }
        }

        public T FetchFirst<T>(bool includeInactive = DefaultIncludeInactive)
        where T : class
        {
            return GetComponentInChildren<T>(includeInactive);
        }

        public Component FetchFirst(Type type, bool includeInactive = DefaultIncludeInactive)
        {
            return GetComponentInChildren(type, includeInactive);
        }

        public T[] FetchAll<T>(bool includeInactive = DefaultIncludeInactive)
        where T : class
        {
            return GetComponentsInChildren<T>(includeInactive);
        }

        public Component[] FetchAll(Type type, bool includeInactive = DefaultIncludeInactive)
        {
            return GetComponentsInChildren(type, includeInactive);
        }
    }
}