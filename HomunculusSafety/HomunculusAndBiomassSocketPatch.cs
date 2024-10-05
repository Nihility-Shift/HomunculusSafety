using CG.Ship.Hull;
using HarmonyLib;
using System.Reflection;

namespace HomunculusSafety
{
    [HarmonyPatch(typeof(HomunculusAndBiomassSocket), "DispenseHomunculusNow")]
    internal class HomunculusAndBiomassSocketPatch
    {
        private static readonly FieldInfo socketModeField = AccessTools.Field(typeof(HomunculusAndBiomassSocket), "_socketMode");

        static bool Prefix(HomunculusAndBiomassSocket __instance)
        {
            //Prevent a second homunculus being created
            if (HomunculusManager.homunculus != null)
            {
                socketModeField.SetValue(__instance, (byte)1); //HomunculusAndBiomassSocket.SocketMode.BiomassInletMode
                return false;
            }
            return true;
        }

        static void Postfix(HomunculusAndBiomassSocket __instance)
        {
            if (__instance.Payload == null) return;
            HomunculusManager.homunculus = __instance.Payload;
            if (HomunculusManager.UrgencyMarker != null)
            {
                HomunculusManager.UrgencyMarker.isFollowingTransform = true;
                HomunculusManager.UrgencyMarker.FollowTarget = HomunculusManager.GetUMT(HomunculusManager.homunculus).transform;
                HomunculusManager.UrgencyMarker = null;
            }

            HomunculusManager.homunculus.OnDropped += HomunculusManager.StartTimer;
            HomunculusManager.homunculus.OnCarried += HomunculusManager.StopTimer;
        }
    }
}
