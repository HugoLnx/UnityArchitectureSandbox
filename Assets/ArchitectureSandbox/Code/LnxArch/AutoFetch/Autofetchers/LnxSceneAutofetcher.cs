using UnityEngine;

namespace LnxArch
{
    [DefaultExecutionOrder (-9999)]
    public class LnxSceneAutofetcher : MonoBehaviour
    {
        private void Awake()
        {
            AutofetchService.AutofetchAllExistingEntites();
        }
    }
}