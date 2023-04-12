using UnityEngine;


namespace LnxArch
{
    [CreateAssetMenu(menuName = "LnxArch/EventChannel/LnxVoidEventChannel", fileName = "VoidEventChannel")]
    public class LnxVoidEventChannel : LnxEventChannel<object>
    {
    }

    [CreateAssetMenu(menuName = "LnxArch/EventChannel/LnxBoolEventChannel", fileName = "BoolEventChannel")]
    public class LnxBoolEventChannel : LnxEventChannel<bool>
    {
    }
}