using CG.Game;
using CG.Ship.Hull;
using CG.Ship.Object;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using VoidManager;
using VoidManager.MPModChecks;
using VoidManager.Utilities;

namespace HomunculusSafety
{
    public class VoidManagerPlugin : VoidManager.VoidPlugin
    {
        public VoidManagerPlugin()
        {
            VoidManager.Events.Instance.HostStartSession += (_, _) =>
            {
                HomunculusManager.StopTimer(null);
                HomunculusManager.homunculus = null;
            };
            VoidManager.Events.Instance.MasterClientSwitched += (player, e) =>
            {
                if (PhotonNetwork.IsMasterClient && !GameSessionManager.InHub)
                {
                    HomunculusManager.homunculus = null;

                    //Get list of all homunculus
                    List<ResourceContainer> homunculusList = UnityEngine.Object.FindObjectsOfType<ResourceContainer>().Where(obj => obj.name.Contains("Item_Homunculus")).ToList();
                    if (homunculusList.Count > 1)
                    {
                        ResourceContainer homunculus = null;

                        //Find any homunculus in the ship computer
                        foreach (ResourceContainer resourceContainer in homunculusList)
                        {
                            if ((resourceContainer.Carrier as CarryablesSocket)?.name?.Contains("CentralShipComputer") == true)
                            {
                                homunculus = resourceContainer;
                            }
                        }
                        if (homunculus != null)
                        {
                            //Keep this homunculus
                            homunculusList.Remove(homunculus);
                            HomunculusManager.homunculus = homunculus;
                        }

                        //Destroy everything else
                        for (int i = homunculusList.Count-1; i >= 0; i--)
                        {
                            var h = homunculusList.ElementAt(i);
                            h.Carrier?.TryReleaseCarryable();
                            Tools.DelayDo(() => h.Destroy(), 1000); //Wait for items to be ejected
                        }

                        //Make sure there's at least one homunculus
                        if (homunculus == null)
                        {
                            HomunculusAndBiomassSocket socket = ClientGame.Current?.PlayerShip?.GameObject?.GetComponentInChildren<HomunculusAndBiomassSocket>();
                            socket.DispenseHomunculusNow();
                            Messaging.Echo("New Homunculus dispensed", false);
                        }
                    }
                    //Make sure there's at least one homunculus
                    else if (homunculusList.Count == 0)
                    {
                        HomunculusAndBiomassSocket socket = ClientGame.Current?.PlayerShip?.GameObject?.GetComponentInChildren<HomunculusAndBiomassSocket>();
                        socket.DispenseHomunculusNow();
                        Messaging.Echo("New Homunculus dispensed", false);
                    }
                    else
                    {
                        HomunculusManager.homunculus = homunculusList.First();
                    }

                    HomunculusManager.homunculus.OnDropped += HomunculusManager.StartTimer;
                    HomunculusManager.homunculus.OnCarried += HomunculusManager.StopTimer;
                }
            };
        }

        public override MultiplayerType MPType => MultiplayerType.Host;

        public override string Author => MyPluginInfo.PLUGIN_AUTHORS;

        public override string Description => MyPluginInfo.PLUGIN_DESCRIPTION;

        public override string ThunderstoreID => MyPluginInfo.PLUGIN_THUNDERSTORE_ID;

        public override SessionChangedReturn OnSessionChange(SessionChangedInput input)
        {
            return new SessionChangedReturn() { SetMod_Session = true };
        }
    }
}
