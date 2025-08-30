using UnityEngine;

public class HapticManager : MonoBehaviour
{
    public static void Haptic(int type)
    {
        if(type == 0)
        {
            MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
        }
        else if (type == 1)
        {
            MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);
        }
        else if (type == 2)
        {
            MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.HeavyImpact);
        }
    }
}