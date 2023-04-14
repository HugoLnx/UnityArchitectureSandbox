using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LnxArch
{
    public static class AutofetchService
    {
        public static void AutofetchAllExistingEntites()
        {
            LnxEntity[] allEntities = GameObject.FindObjectsByType<LnxEntity>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            AutofetchEntities(allEntities);
        }

        public static void StartAutofetchChainAt(GameObject gameObject)
        {
            LnxEntity rootEntity = gameObject.GetComponentsInParent<LnxEntity>(includeInactive: true)
                .Where(entity => !entity.WasAutofetched)
                .LastOrDefault();
            GameObject chainRoot = rootEntity?.gameObject ?? gameObject;
            AutofetchChildrenOf(gameObject);
        }

        public static void AutofetchChildrenOf(GameObject gameObject)
        {
            AutofetchChildrenOf(new GameObject[] { gameObject });
        }

        public static void AutofetchChildrenOf(IEnumerable<GameObject> gameObjects)
        {
            IEnumerable<LnxEntity> entities =
                gameObjects
                .SelectMany(
                    obj => obj.GetComponentsInChildren<LnxEntity>(includeInactive: true)
                )
                .Distinct();
            AutofetchEntities(entities);
        }

        public static void AutofetchEntities(IEnumerable<LnxEntity> entities)
        {
            HashSet<LnxEntity> pendingEntities =
                entities
                .Where(entity => !entity.WasAutofetched)
                .ToHashSet();

            IEnumerable<LnxEntity> rootEntities =
                pendingEntities
                .Select(entity =>
                    entity
                    .GetComponentsInParent<LnxEntity>(includeInactive: true)
                    .Where(entity => pendingEntities.Contains(entity))
                    .Last()
                )
                .Distinct();

            IEnumerable<MonoBehaviour> pendingBehaviours =
                rootEntities
                .SelectMany(entity => entity.GetComponentsInChildren<MonoBehaviour>(includeInactive: true))
                .Distinct();

            AutofetchBehaviours(pendingBehaviours);
            
            foreach (LnxEntity pendingEntity in pendingEntities)
            {
                pendingEntity.WasAutofetched = true;
            }
        }

        private static void AutofetchBehaviours(IEnumerable<MonoBehaviour> allBehaviours)
        {
            AutofetchExecutor executor = new();
            IEnumerable<(MonoBehaviour, AutofetchType)> behaviours =
                DependencyResolver.Instance.FilterAutofetchBehavioursAndOrderForCalling(allBehaviours);
            foreach ((MonoBehaviour behaviour, AutofetchType type) in behaviours)
            {
                executor.ExecuteAutofetchOn(behaviour, type);
            }
        }
    }
}