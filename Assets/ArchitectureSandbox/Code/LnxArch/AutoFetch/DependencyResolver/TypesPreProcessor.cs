using System;
using System.Collections.Generic;
using System.Linq;

namespace LnxArch
{
    public class TypesPreProcessor
    {
        private IEnumerable<Type> _allTypes;
        public Dictionary<Type, AutofetchType> _mapAutofetchType;
        private Dictionary<Type, IEnumerable<Type>> _mapToAssignable;
        private DependencyGraph<AutofetchType> _dependencyGraph;

        public Dictionary<Type, AutofetchType> MapAutofetchType =>
            _mapAutofetchType ??=
            AllTypes
            .Select(t => AutofetchType.TryToBuildFrom(t))
            .Where(t => t != null)
            .ToDictionary(t => t.Type, t => t);

        public DependencyGraph<AutofetchType> DependencyGraph =>
            _dependencyGraph ??= BuildDependencyGraphForAllTypesWithAutofetch();

        private IEnumerable<Type> AllTypes =>
            _allTypes ??=
            AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes());

        private Dictionary<Type, IEnumerable<Type>> MapToAllAssignable =>
            _mapToAssignable ??=
            AllTypes
            .Where(t => t.IsInterface || AutofetchType.BaseType.IsAssignableFrom(t))
            .Select(interfaceType => {
                var assignableTypes = AllAutofetchTypesAssignableTo(interfaceType);
                return (interfaceType, assignableTypes);
            })
            .Where(tuple => tuple.assignableTypes.Any())
            .ToDictionary(tuple => tuple.interfaceType, tuple => tuple.assignableTypes);

        private IEnumerable<AutofetchType> TypesWithAutofetch => MapAutofetchType.Values;

        private IEnumerable<Type> AllAutofetchTypesAssignableTo(Type interfaceType)
        {
            return TypesWithAutofetch
            .Select(typeWithAutofetch => typeWithAutofetch.Type)
            .Where(type => type != interfaceType && interfaceType.IsAssignableFrom(type));
        }

        public DependencyGraph<AutofetchType> BuildDependencyGraphForAllTypesWithAutofetch()
        {
            DependencyGraph<AutofetchType> dependencyGraph = new();
            foreach (AutofetchType typeWithAutofetch in TypesWithAutofetch)
            {
                // Debug.Log($"[GraphBuild:{behaviourType.Name}] Dependencies...");
                foreach (AutofetchType dependency in GetAutofetchDependencies(typeWithAutofetch))
                {
                    // Debug.Log($"[GraphBuild:{behaviourType.Name}][Dependency:{dependencyType.Name}] Add {behaviourType.Name} depends on {dependencyType.Name}");
                    dependencyGraph.AddPair(origin: typeWithAutofetch, dependency: dependency);
                }
            }
            return dependencyGraph;
        }

        private IEnumerable<AutofetchType> GetAutofetchDependencies(AutofetchType typeWithAutofetch)
        {
            return typeWithAutofetch
            .FindTypeDependencies()
            .SelectMany(GetAssignableAutofetchTypes)
            .Distinct()
            .Select(t => MapAutofetchType.GetValueOrDefault(t, null))
            .Where(t => t != null);
        }

        private IEnumerable<Type> GetAssignableAutofetchTypes(Type type)
        {
            if (AutofetchType.BaseType.IsAssignableFrom(type))
            {
                yield return type;
            }
            IEnumerable<Type> assignables = MapToAllAssignable.GetValueOrDefault(type, null);
            if (assignables == null)
            {
                yield break;
            }
            foreach (Type assignableType in assignables)
            {
                yield return assignableType;
            }
        }
    }
}