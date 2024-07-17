using CG.Game;
using CG.Objects;
using CG.Ship.Hull;
using Gameplay.Carryables;
using System;
using UnityEngine;
using VoidManager.Utilities;

namespace HomunculusSafety
{
    internal class HomunculusManager
    {
        internal static AbstractCarryableObject homunculus;
        private static float time = 0f;
        private const float homunculusTimeoutSeconds = 15f;

        internal static void StartTimer(ICarrier carrier)
        {
            time = Time.time + homunculusTimeoutSeconds;
            VoidManager.Events.Instance.LateUpdate += CheckTimer;
        }

        internal static void StopTimer(ICarrier carrier)
        {
            VoidManager.Events.Instance.LateUpdate -= CheckTimer;
        }

        internal static void CheckTimer(object sender, EventArgs e)
        {
            if (time > Time.time) return;
            VoidManager.Events.Instance.LateUpdate -= CheckTimer;

            //Destroy the current homunculus
            Messaging.Echo("Homunculus lost", false);
            homunculus.Destroy();
            homunculus = null;

            //Create a new homunculus
            HomunculusAndBiomassSocket socket = ClientGame.Current.PlayerShip.GetComponentInChildren<HomunculusAndBiomassSocket>();
            Tools.DelayDo(tryDispenseHomunculus, 1000); //For dramatic effect, delay not needed

            void tryDispenseHomunculus()
            {
                if (socket.Payload == null)
                {
                    socket.DispenseHomunculusNow();
                    Messaging.Echo("New Homunculus dispensed", false);
                }
                else
                {
                    //If the biomass port is full, wait a second and try again
                    Tools.DelayDo(tryDispenseHomunculus, 1000);
                }
            }
        }
    }
}
