using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LnxArch
{
    public sealed class DependencyResolver
    {
        private static DependencyResolver _instance;
        private Dictionary<Type, AutofetchType> _mapAutofetchType;

        public static DependencyResolver Instance => _instance ??= new DependencyResolver();

        private DependencyResolver()
        {
            TypesPreProcessor preprocessor = new();
            _mapAutofetchType = preprocessor.MapAutofetchType;
            DependencyGraph<AutofetchType> dependencyGraph = preprocessor.DependencyGraph;
            dependencyGraph.DoBreadthFirstTraversalForEachParentlessNode((AutofetchType type, int depth) => {
                type.Priority = Mathf.Max(depth, type.Priority);
            });
        }

        public IEnumerable<(MonoBehaviour, AutofetchType)> FilterAutofetchBehavioursAndOrderForCalling(IEnumerable<MonoBehaviour> behaviours)
        {
            return behaviours
            .Select(behaviour => (behaviour, type: AutoFetchTypeForBehaviour(behaviour)))
            .Where(selected => selected.type != null)
            .OrderByDescending(selected => selected.type.Priority);
        }

        private AutofetchType AutoFetchTypeForBehaviour(MonoBehaviour behaviour)
        {
            return _mapAutofetchType.GetValueOrDefault(behaviour.GetType(), null);
        }
    }
}