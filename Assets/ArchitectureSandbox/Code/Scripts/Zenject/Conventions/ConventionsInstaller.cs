using System;
using UnityEngine;
using Zenject;

namespace ArchitectureSandbox.ZenjectConventions
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class FromAncestorAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class FromChildAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class SiblingAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class SingletonAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class DontTryToInjectAttribute : Attribute {}

    public class ConventionsInstaller : MonoInstaller<ConventionsInstaller>
    {
        public override void InstallBindings()
        {
            BindHierarchyLookupsOn(b => b.AllClasses().DerivingFrom<Component>());
            BindHierarchyLookupsOn(b => b.AllInterfaces());
        }

        private void BindHierarchyLookupsOn(
            Func<ConventionSelectTypesBinder, ConventionFilterTypesBinder> bindingFilter)
        {
            Container.Bind(b => bindingFilter(b))
            .WithId("Sibling")
            .FromComponentSibling()
            .WhenInjectedInto<MonoBehaviour>();

            Container.Bind(b => bindingFilter(b))
            .WithId("FromAncestor")
            .FromComponentInParents()
            .WhenInjectedInto<MonoBehaviour>();

            Container.Bind(b => bindingFilter(b))
            .FromComponentInChildren()
            .WhenInjectedInto<MonoBehaviour>();

            // if (singletonBinding)
            // {
            //     Container.Bind(b => bindingFilter(b))
            //     .WithId("Lol")
            //     .AsSingle()
            //     .WhenInjectedInto<MonoBehaviour>();
            // }
        }
    }
}