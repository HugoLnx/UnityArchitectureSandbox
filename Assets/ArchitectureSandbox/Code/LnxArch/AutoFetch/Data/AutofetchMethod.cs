using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LnxArch
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class AutoFetchAttribute : Attribute
    {}

    public readonly struct AutofetchMethod
    {
        public MethodInfo Info { get; }
        public AutoFetchAttribute AutofetchAttribute { get; }
        public AutofetchParameter[] Parameters { get; }

        public AutofetchMethod(MethodInfo method, AutoFetchAttribute autofetchAttribute, AutofetchParameter[] parameters)
        {
            Info = method;
            AutofetchAttribute = autofetchAttribute;
            Parameters = parameters;
        }

        public static AutofetchMethod BuildFrom(MethodInfo method)
        {
            // TODO: Verify if method has generics and throw and exception if it does
            return new AutofetchMethod(
                method: method,
                autofetchAttribute: method.GetCustomAttribute<AutoFetchAttribute>(false),
                parameters: method.GetParameters().Select(AutofetchParameter.BuildFrom).ToArray()
            );
        }

        public IEnumerable<Type> FindTypeDependencies()
        {
            return Parameters
                .Select(param => param.ComponentType)
                .Distinct();
        }

        public void InvokeWithResolvedParameters(MonoBehaviour behaviour, Func<AutofetchParameter, object> resolveParameter)
        {
            object[] resolvedParameters = this
                .Parameters
                .Select(param => resolveParameter(param))
                .ToArray();

            this.Info.Invoke(behaviour, resolvedParameters);
        }
    }
}