using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LnxArch
{
    public class AutofetchType
    {
        public static Type BaseType = typeof(MonoBehaviour);
        public Type Type { get; private set; }
        public AutofetchMethod[] AutofetchMethods { get; private set; }
        public AutoFetchAttribute MainAttribute { get; private set; }
        public int Priority { get; set; }

        public AutofetchType(Type type, AutofetchMethod[] autofetchMethods, AutoFetchAttribute mainAttribute)
        {
            Type = type;
            AutofetchMethods = autofetchMethods;
            MainAttribute = mainAttribute;
        }

        public static AutofetchType TryToBuildFrom(Type type)
        {
            if (!IsSupported(type)) return null;
            IEnumerable<AutofetchMethod> autofetchMethods = GetAutoFetchMethodsOf(type);
            if (!autofetchMethods.Any()) return null;
            return new AutofetchType(
                type: type,
                autofetchMethods: autofetchMethods.ToArray(),
                mainAttribute: autofetchMethods.First().AutofetchAttribute
            );
        }

        public static bool IsSupported(Type type)
        {
            return type.IsClass
            && !type.IsAbstract
            && BaseType.IsAssignableFrom(type);
        }

        public IEnumerable<Type> FindTypeDependencies()
        {
            return AutofetchMethods
                .SelectMany(m => m.FindTypeDependencies())
                .Distinct();
        }

        private static IEnumerable<AutofetchMethod> GetAutoFetchMethodsOf(Type type)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
            .Select(AutofetchMethod.BuildFrom)
            .Where(m => m.AutofetchAttribute != null);
        }
    }
}