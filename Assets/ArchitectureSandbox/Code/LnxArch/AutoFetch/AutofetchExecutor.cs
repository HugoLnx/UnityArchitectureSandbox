using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace LnxArch
{
    public class AutofetchExecutor
    {
        public void ExecuteAutofetchOn(MonoBehaviour behaviour, AutofetchType type)
        {
            LnxEntity entity = LnxBehaviour.GetLnxEntity(behaviour);
            Assert.IsNotNull(entity, "Behaviours with Autofetch methods must be inside an LnxEntity");

            foreach (AutofetchMethod method in type.AutofetchMethods)
            {
                method.InvokeWithResolvedParameters(behaviour, param => ResolveParameter(param, method, behaviour, entity));
            }
        }

        private object ResolveParameter(AutofetchParameter param, AutofetchMethod method, MonoBehaviour behaviour, LnxEntity entity)
        {
            object fetched = null;
            foreach (IFetchAttribute fetchAttribute in param.FetchAttributes)
            {
                fetched = TryFetchDependencyWith(fetchAttribute, param, behaviour, entity);
                if (fetched != null) break;
            }

            Debug.Log($"[Fetched:{behaviour.GetType().Name}] {param.Type.Name} {param.Info.Name} = {fetched?.GetType()}");
            if (!param.Info.HasDefaultValue)
            {
                string errorPrefix = $"[LnxArch:AutoFetch:{behaviour.GetType().Name}#{method.Info.Name}({param.Type.Name} {param.Info.Name})]";
                Assert.IsTrue(
                    !typeof(IEnumerable<>).IsAssignableFrom(param.Type)
                    || param.HasValidCollectionWrap,
                    $"{errorPrefix} Only types derived from Component, Component[] or List<Component> can be auto fetched.");
                //Assert.IsTrue(
                //    typeof(Component).IsAssignableFrom(fetchType),
                //    $"{errorPrefix} {fetchType} doesn't inherits Component.");
                Assert.IsNotNull(fetched,
                    $"{errorPrefix} Could not fulfill that dependency");
            }
            return fetched;
        }

        private object TryFetchDependencyWith(IFetchAttribute fetchAttribute, AutofetchParameter param, MonoBehaviour behaviour, LnxEntity entity)
        {
            object fetched;
            if (param.HasArrayWrapping)
            {
                fetched = ForceCastToArray(
                    param.ComponentType,
                    fetchAttribute.FetchMany(behaviour, entity, param.ComponentType)
                );
            }
            else if (param.HasListWrapping)
            {
                fetched = ForceCastToList(
                    param.ComponentType,
                    fetchAttribute.FetchMany(behaviour, entity, param.ComponentType)
                );
            }
            else
            {
                fetched = fetchAttribute.FetchOne(behaviour, entity, param.ComponentType);
            }
            return fetched;
        }
        private static object ForceCastToList<T>(Type targetType, IEnumerable<T> enumerable)
        {
            MethodInfo castMethod =
            typeof(AutofetchExecutor)
            .GetMethod(nameof(AutofetchExecutor.EnumerableCastToList), BindingFlags.NonPublic | BindingFlags.Static)
            .MakeGenericMethod(typeof(T), targetType);
            return castMethod.Invoke(null, new object[] { enumerable });
        }
        private static object ForceCastToArray<T>(Type targetType, IEnumerable<T> enumerable)
        {
            MethodInfo castMethod =
            typeof(AutofetchExecutor)
            .GetMethod(nameof(AutofetchExecutor.EnumerableCastToArray), BindingFlags.NonPublic | BindingFlags.Static)
            .MakeGenericMethod(typeof(T), targetType);
            return castMethod.Invoke(null, new object[] { enumerable });
        }

        private static List<K> EnumerableCastToList<T, K>(IEnumerable<T> enumerable)
        {
            return enumerable.Cast<K>().ToList();
        }
        private static K[] EnumerableCastToArray<T, K>(IEnumerable<T> enumerable)
        {
            return enumerable.Cast<K>().ToArray();
        }

        private static void AssertNotNull<T, K>(object v)
        {
            Debug.Assert(v != null && !v.Equals(null), $"{typeof(T)}'s dependency {typeof(K)} received no value on construction.");
        }
    }
}