using System;
using UnityEngine;
using Zenject;

namespace ArchitectureSandbox.ZenjectConventions
{
    public class EntityInstaller : MonoInstaller<EntityInstaller>
    {
        public override void InstallBindings()
        {
            BindHierarchyLookupsOn(b => b.AllNonAbstractClasses().DerivingFrom<Component>());
            BindHierarchyLookupsOn(b => b.AllInterfaces());
        }

        private void BindHierarchyLookupsOn(
            Func<ConventionSelectTypesBinder, ConventionFilterTypesBinder> bindingFilter)
        {
            Container.Bind(b => bindingFilter(b))
            .FromMethod(ctx => ctx.Container.Resolve<Context>().gameObject.GetComponentInChildren(ctx.ObjectType))
            //.FromComponentInChildren()
            //.FromComponentOnRoot()
            .WhenInjectedInto<MonoBehaviour>();
        }
    }
}