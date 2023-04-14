using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LnxArch
{
    public enum FetchCollectionWrap { None, Array, List }
    public readonly struct AutofetchParameter
    {
        public Type Type { get; }
        public Type ComponentType { get; }
        private FetchCollectionWrap CollectionWrap { get; }
        public ParameterInfo Info { get; }
        public IFetchAttribute[] FetchAttributes { get; }

        public bool HasListWrapping => CollectionWrap == FetchCollectionWrap.List;
        public bool HasArrayWrapping => CollectionWrap == FetchCollectionWrap.Array;
        public bool HasValidCollectionWrap => CollectionWrap != FetchCollectionWrap.None;

        public AutofetchParameter(Type type, Type componentType,
            FetchCollectionWrap collectionWrap, ParameterInfo parameter,
            IFetchAttribute[] fetchAttributes)
        {
            Type = type;
            ComponentType = componentType;
            CollectionWrap = collectionWrap;
            Info = parameter;
            FetchAttributes = fetchAttributes;
        }

        public static AutofetchParameter BuildFrom(ParameterInfo parameter)
        {
            FetchCollectionWrap collectionWrap = GetCollectionType(parameter.ParameterType);
            return new AutofetchParameter(
                type: parameter.ParameterType,
                componentType: GetInnerTypeFrom(parameter.ParameterType, collectionWrap),
                collectionWrap: collectionWrap,
                parameter: parameter,
                fetchAttributes: GetFetchAttributesOf(parameter)
            );
        }

        private static Type GetInnerTypeFrom(Type type, FetchCollectionWrap wrap)
        {
            return wrap switch
            {
                FetchCollectionWrap.Array => type.GetElementType(),
                FetchCollectionWrap.List => type.GetGenericArguments()[0],
                _ => type
            };
        }

        private static FetchCollectionWrap GetCollectionType(Type type)
        {
            if (type.IsArray)
            {
                return FetchCollectionWrap.Array;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                return FetchCollectionWrap.List;
            }
            return FetchCollectionWrap.None;
        }

        private static IFetchAttribute[] GetFetchAttributesOf(ParameterInfo parameter)
        {
            IEnumerable<IFetchAttribute> explicitDeclaredAttributes = Attribute.GetCustomAttributes(parameter)
                .Where(attr => typeof(IFetchAttribute).IsAssignableFrom(attr.GetType()))
                .Select(attr => (IFetchAttribute) attr)
                .OrderBy(attr => attr.Order);

            if (explicitDeclaredAttributes.Any())
            {
                return explicitDeclaredAttributes.ToArray();
            }
            else
            {
                return new IFetchAttribute[] { new FromEntityAttribute() };
            }
        }
    }

    public interface IFetchAttribute
    {
        int Order { get; set; }
        Component FetchOne(MonoBehaviour behaviour, LnxEntity entity, Type type);
        IEnumerable<Component> FetchMany(MonoBehaviour behaviour, LnxEntity entity, Type type);
    }
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FromEntityAttribute : Attribute, IFetchAttribute
    {
        public int Order { get; set; }

        public FromEntityAttribute(int order = 0)
        {
            this.Order = order;
        }

        public Component FetchOne(MonoBehaviour _, LnxEntity entity, Type type)
        {
            return entity.FetchFirst(type);
        }
        public IEnumerable<Component> FetchMany(MonoBehaviour _, LnxEntity entity, Type type)
        {
            return entity.FetchAll(type);
        }
    }
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FromParentEntityAttribute : Attribute, IFetchAttribute
    {
        public int Order { get; set; }

        public FromParentEntityAttribute(int order = 0)
        {
            Order = order;
        }

        public Component FetchOne(MonoBehaviour _, LnxEntity entity, Type type)
        {
            LnxEntity parentEntity = entity.transform.parent.GetComponentInParent<LnxEntity>(includeInactive: true);
            return parentEntity.FetchFirst(type);
        }
        public IEnumerable<Component> FetchMany(MonoBehaviour _, LnxEntity entity, Type type)
        {
            LnxEntity parentEntity = entity.transform.parent.GetComponentInParent<LnxEntity>(includeInactive: true);
            return parentEntity.FetchAll(type);
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FromLocalAncestorAttribute : Attribute, IFetchAttribute
    {
        public int Order { get; set; }

        public FromLocalAncestorAttribute(int order = 0)
        {
            Order = order;
        }

        public Component FetchOne(MonoBehaviour behaviour, LnxEntity _, Type type)
        {
            return behaviour.GetComponentInParent(type, includeInactive: true);
        }
        public IEnumerable<Component> FetchMany(MonoBehaviour behaviour, LnxEntity _, Type type)
        {
            return behaviour.GetComponentsInParent(type, includeInactive: true);
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FromLocalChildAttribute : Attribute, IFetchAttribute
    {
        public int Order { get; set; }

        public FromLocalChildAttribute(int order = 0)
        {
            Order = order;
        }

        public Component FetchOne(MonoBehaviour behaviour, LnxEntity _, Type type)
        {
            return behaviour.GetComponentInChildren(type, includeInactive: true);
        }
        public IEnumerable<Component> FetchMany(MonoBehaviour behaviour, LnxEntity _, Type type)
        {
            return behaviour.GetComponentsInChildren(type, includeInactive: true);
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FromLocalAttribute : Attribute, IFetchAttribute
    {
        public int Order { get; set; }

        public FromLocalAttribute(int order = 0)
        {
            Order = order;
        }

        public Component FetchOne(MonoBehaviour behaviour, LnxEntity _, Type type)
        {
            return behaviour.GetComponent(type);
        }
        public IEnumerable<Component> FetchMany(MonoBehaviour behaviour, LnxEntity _, Type type)
        {
            return behaviour.GetComponents(type);
        }
    }

}