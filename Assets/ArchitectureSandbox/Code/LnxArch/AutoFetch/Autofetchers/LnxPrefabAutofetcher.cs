using System;
using System.Collections;
using UnityEngine;

namespace LnxArch
{
    [DefaultExecutionOrder(-9998)]
    public class LnxPrefabAutofetcher : MonoBehaviour
    {
        private void Awake()
        {
            AutofetchService.StartAutofetchChainAt(this.gameObject);
        }
    }
}