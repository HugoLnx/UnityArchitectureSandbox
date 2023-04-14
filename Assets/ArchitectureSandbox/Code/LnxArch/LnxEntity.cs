using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LnxArch
{
    [DefaultExecutionOrder (-9997)]
    public class LnxEntity : MonoBehaviour
    {
        private static LnxSceneAutofetcher s_sceneAutofetcher;
        public bool WasAutofetched { get; set; }
        private const bool DefaultIncludeInactive = true;

        private void Awake()
        {
            if (s_sceneAutofetcher == null)
            {
                s_sceneAutofetcher = new GameObject("LnxSceneAutofetcher").AddComponent<LnxSceneAutofetcher>();
            }
            else
            {
                if (WasAutofetched) return;
                AutofetchService.StartAutofetchChainAt(this.gameObject);
                WasAutofetched = true;
            }
        }

        public T FetchFirst<T>(bool includeInactive = DefaultIncludeInactive)
        where T : class
        {
            return GetComponentInChildren<T>(includeInactive);
        }

        public Component FetchFirst(Type type, bool includeInactive = DefaultIncludeInactive)
        {
            return GetComponentInChildren(type, includeInactive);
        }

        public T[] FetchAll<T>(bool includeInactive = DefaultIncludeInactive)
        where T : class
        {
            return GetComponentsInChildren<T>(includeInactive);
        }

        public Component[] FetchAll(Type type, bool includeInactive = DefaultIncludeInactive)
        {
            return GetComponentsInChildren(type, includeInactive);
        }
    }
}