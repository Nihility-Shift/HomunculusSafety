using CG.Ship.Hull;
using HarmonyLib;

namespace HomunculusSafety
{
    [HarmonyPatch(typeof(HomunculusAndBiomassSocket), "DispenseHomunculusNow")]
    internal class HomunculusAndBiomassSocketPatch
    {
        static bool Prefix()
        {
            //Prevent a second homunculus being created
            if (HomunculusManager.homunculus != null)
            {
                return false;
            }
            return true;
        }

        static void Postfix(HomunculusAndBiomassSocket __instance)
        {
            HomunculusManager.homunculus = __instance.Payload;
            HomunculusManager.homunculus.OnDropped += HomunculusManager.StartTimer;
            HomunculusManager.homunculus.OnCarried += HomunculusManager.StopTimer;
        }
    }
}
