using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LnxArch;
using UnityEngine;

namespace ArchitectureSandbox.LnxArchSandbox
{
    public class GunBehaviour : LnxBehaviour
    {
        [Autofetch]
        private void Prepare(
            [FromParentEntity]
            PlayerLnxArchBehaviour player,

            [FromParentEntity]
            Transform parentEntityTransform,

            [FromLocal]
            Transform localTransform,

            [FromLocalChild]
            Collider childCollider,

            [FromLocalAncestor]
            Collider ancestorCollider,

            Collider[] colliders,

            List<Collider> collidersList,

            [FromParentEntity]
            Component[] allComponents
        )
        {
            Debug.Log($"Player: {player.gameObject.name}");
            Debug.Log($"PlayerTransform: {parentEntityTransform.gameObject.name}");
            Debug.Log($"LocalTransform: {localTransform.gameObject.name}");
            Debug.Log($"childCollider: {childCollider.gameObject.name}");
            Debug.Log($"ancestorCollider: {ancestorCollider.gameObject.name}");
            Debug.Log($"colliders: {string.Join(", ", colliders.Select(c => c.gameObject.name))}");
            Debug.Log($"collidersList: {string.Join(", ", collidersList.Select(c => c.gameObject.name))}");
            Debug.Log($"allComponents: {string.Join(", ", allComponents.Select(c => $"({c.GetType().Name}|{c.gameObject.name})"))}");
        }
    }
}
