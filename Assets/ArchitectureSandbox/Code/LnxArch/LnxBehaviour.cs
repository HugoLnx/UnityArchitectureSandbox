using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using System.Reflection;

namespace LnxArch
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class AutoFetchAttribute : Attribute
    {}

    public interface IFetchAttribute
    {
        Component FetchOne(LnxBehaviour behaviour, LnxEntity entity, Type type);
        IEnumerable<Component> FetchMany(LnxBehaviour behaviour, LnxEntity entity, Type type);
    }
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FromEntityAttribute : Attribute, IFetchAttribute
    {
        public Component FetchOne(LnxBehaviour _, LnxEntity entity, Type type)
        {
            return entity.FetchFirst(type);
        }
        public IEnumerable<Component> FetchMany(LnxBehaviour _, LnxEntity entity, Type type)
        {
            return entity.FetchAll(type);
        }
    }
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FromParentEntityAttribute : Attribute, IFetchAttribute
    {
        public Component FetchOne(LnxBehaviour _, LnxEntity entity, Type type)
        {
            LnxEntity parentEntity = entity.transform.parent.GetComponentInParent<LnxEntity>();
            return parentEntity.FetchFirst(type);
        }
        public IEnumerable<Component> FetchMany(LnxBehaviour _, LnxEntity entity, Type type)
        {
            LnxEntity parentEntity = entity.transform.parent.GetComponentInParent<LnxEntity>();
            return parentEntity.FetchAll(type);
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FromLocalAncestorAttribute : Attribute, IFetchAttribute
    {
        public Component FetchOne(LnxBehaviour behaviour, LnxEntity _, Type type)
        {
            return behaviour.GetComponentInParent(type, includeInactive: true);
        }
        public IEnumerable<Component> FetchMany(LnxBehaviour behaviour, LnxEntity _, Type type)
        {
            return behaviour.GetComponentsInParent(type, includeInactive: true);
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FromLocalChildAttribute : Attribute, IFetchAttribute
    {
        public Component FetchOne(LnxBehaviour behaviour, LnxEntity _, Type type)
        {
            return behaviour.GetComponentInChildren(type, includeInactive: true);
        }
        public IEnumerable<Component> FetchMany(LnxBehaviour behaviour, LnxEntity _, Type type)
        {
            return behaviour.GetComponentsInChildren(type, includeInactive: true);
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FromLocalAttribute : Attribute, IFetchAttribute
    {
        public Component FetchOne(LnxBehaviour behaviour, LnxEntity _, Type type)
        {
            return behaviour.GetComponent(type);
        }
        public IEnumerable<Component> FetchMany(LnxBehaviour behaviour, LnxEntity _, Type type)
        {
            return behaviour.GetComponents(type);
        }
    }

    public abstract class LnxBehaviour : MonoBehaviour
    {
        protected LnxEntity _entity { get; private set; }
        private bool _lnxConstructed = false;

        private MethodInfo _constructMethod;
        private HashSet<Type> _constructDependencyTypes;

        public MethodInfo ConstructMethod => _constructMethod ??= GetConstructorMethodOn(this.GetType());

        public HashSet<Type> ConstructDependencyTypes => _constructDependencyTypes ??= GetParameterTypesOn(ConstructMethod);

        private static MethodInfo GetConstructorMethodOn(Type type)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(m => m.GetCustomAttribute<AutoFetchAttribute>(false) != null)
            .FirstOrDefault();
        }

        private static HashSet<Type> GetParameterTypesOn(MethodInfo method)
        {
            return method.GetParameters()
            .Select(param => param.ParameterType)
            .ToHashSet();
        }

        public static HashSet<Type> GetDependencyTypes(Type type)
        {
            return GetParameterTypesOn(GetConstructorMethodOn(type));
        }

        // private void Awake()
        // {
        //     _entity = GetComponentInParent<LnxEntity>();
        //     Assert.IsNotNull(_entity, "LnxBehaviour must be inside an LnxEntity");

        //     ResolveConstruction();
        // }
        // protected abstract void ResolveConstruction();
        protected static void AssertNotNull<T, K>(object v)
        {
            Debug.Assert(v != null && !v.Equals(null), $"{typeof(T)}'s dependency {typeof(K)} received no value on construction.");
        }

        public void TriggerConstruction()
        {
            if (_lnxConstructed) return;
            _lnxConstructed = true;

            _entity = GetComponentInParent<LnxEntity>();
            Assert.IsNotNull(_entity, "LnxBehaviour must be inside an LnxEntity");

            Debug.Log($"[{GetType().Name}] TriggerConstruction");
            ResolveConstruction();
        }

        private void ResolveConstruction()
        {
            if (ConstructMethod == null) return;
            InvokeWithResolvedParameters(ConstructMethod, ResolveParameter);
        }

        private object ResolveParameter(ParameterInfo param)
        {
            IFetchAttribute fetchAttribute = Attribute.GetCustomAttributes(param)
                .Where(attr => typeof(IFetchAttribute).IsAssignableFrom(attr.GetType()))
                .Select(attr => (IFetchAttribute) attr)
                .FirstOrDefault() ?? new FromEntityAttribute();
            
            object fetched;
            Type paramType = param.ParameterType;
            Type fetchType;
            bool isValidEnumerable = false;
            if (paramType.IsArray)
            {
                fetchType = paramType.GetElementType();
                fetched = ForceCastToArray(fetchType, fetchAttribute.FetchMany(this, _entity, fetchType));
                isValidEnumerable = true;
            } else if (paramType.IsGenericType && paramType.GetGenericTypeDefinition() == typeof(List<>))
            {
                fetchType = paramType.GetGenericArguments()[0];
                fetched = ForceCastToList(fetchType, fetchAttribute.FetchMany(this, _entity, fetchType));
                isValidEnumerable = true;
            } else {
                fetchType = paramType;
                fetched = fetchAttribute.FetchOne(this, _entity, fetchType);
            }
            Debug.Log($"[Fetched:{GetType().Name}] {paramType.Name} {param.Name} = {fetched.GetType()}");
            if (!param.HasDefaultValue)
            {
                string errorPrefix = $"[LnxArch:AutoFetch:{GetType().Name}#{ConstructMethod.Name}({paramType.Name} {param.Name})]";
                Assert.IsTrue(
                    !typeof(IEnumerable<>).IsAssignableFrom(paramType)
                    || isValidEnumerable,
                    $"{errorPrefix} Only types derived from Component, Component[] or List<Component> can be auto fetched.");
                //Assert.IsTrue(
                //    typeof(Component).IsAssignableFrom(fetchType),
                //    $"{errorPrefix} {fetchType} doesn't inherits Component.");
                Assert.IsNotNull(fetched,
                    $"{errorPrefix} Could not fulfill that dependency");
            }
            return fetched;
        }

        private static List<K> EnumerableCastToList<T, K>(IEnumerable<T> enumerable)
        {
            return enumerable.Cast<K>().ToList();
        }
        private static K[] EnumerableCastToArray<T, K>(IEnumerable<T> enumerable)
        {
            return enumerable.Cast<K>().ToArray();
        }
        private static object ForceCastToList<T>(Type targetType, IEnumerable<T> enumerable)
        {
            MethodInfo castMethod = typeof(LnxBehaviour)
            .GetMethod(nameof(LnxBehaviour.EnumerableCastToList), BindingFlags.NonPublic | BindingFlags.Static)
            .MakeGenericMethod(typeof(T), targetType);
            return castMethod.Invoke(null, new object[] { enumerable });
        }
        private static object ForceCastToArray<T>(Type targetType, IEnumerable<T> enumerable)
        {
            MethodInfo castMethod = typeof(LnxBehaviour)
            .GetMethod(nameof(LnxBehaviour.EnumerableCastToArray), BindingFlags.NonPublic | BindingFlags.Static)
            .MakeGenericMethod(typeof(T), targetType);
            return castMethod.Invoke(null, new object[] { enumerable });
        }

        private void InvokeWithResolvedParameters(MethodInfo targetMethod, Func<ParameterInfo, object> resolveParameter)
        {
            //ParameterInfo[] parameters = targetMethod.GetParameters();

            //object[] resolvedParameters = new object[parameters.Length];

            //for (int i = 0; i < parameters.Length; i++)
            //{
            //    object resolvedParameter = resolveParameter(parameters[i]);
            //    resolvedParameters[i] = resolvedParameter;
            //}
            object[] resolvedParameters = targetMethod
                .GetParameters()
                .Select(param => resolveParameter(param))
                .ToArray();

            targetMethod.Invoke(this, resolvedParameters);
        }
    }
}