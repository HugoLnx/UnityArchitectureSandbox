using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using System.Reflection;

namespace LnxArch
{
    public abstract class LnxBehaviour : MonoBehaviour
    {
        private LnxEntity _entity;

        public LnxEntity Entity => _entity ??= LnxEntityLookup(this);
        public LnxEntity ParentEntity => ParentLnxEntityLookup(this);

        public static LnxEntity GetLnxEntity(MonoBehaviour behaviour)
        {
            LnxBehaviour lnxBehaviour = behaviour as LnxBehaviour;
            return lnxBehaviour?.Entity ?? LnxEntityLookup(behaviour);
        }

        public static LnxEntity GetParentLnxEntity(MonoBehaviour behaviour)
        {
            return ParentLnxEntityLookup(behaviour);
        }

        private static LnxEntity LnxEntityLookup(MonoBehaviour behaviour)
        {
            LnxEntity entity = behaviour.GetComponentInParent<LnxEntity>(includeInactive: true);
            Assert.IsNotNull(entity, $"{behaviour.GetType().Name} must be inside a LnxEntity to be a LnxBehaviour or have an Autofetch method.");
            return entity;
        }

        private static LnxEntity ParentLnxEntityLookup(MonoBehaviour behaviour)
        {
            LnxEntity ownerEntity = GetLnxEntity(behaviour);
            return ownerEntity.transform.parent.GetComponentInParent<LnxEntity>(includeInactive: true);
        }

    }
}