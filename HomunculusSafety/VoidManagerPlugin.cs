using CG.Ship.Object;
using Photon.Pun;
using System.Linq;
using VoidManager.MPModChecks;

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
            VoidManager.Events.Instance.MasterClientSwitched += (_, _) =>
            {
                if (PhotonNetwork.IsMasterClient)
                    HomunculusManager.homunculus = UnityEngine.Object.FindObjectsOfType<ResourceContainer>().FirstOrDefault(obj => obj.name.Contains("Item_Homunculus"));
                HomunculusManager.homunculus.OnDropped += HomunculusManager.StartTimer;
                HomunculusManager.homunculus.OnCarried += HomunculusManager.StopTimer;
                if (HomunculusManager.homunculus.Carrier == null)
                    HomunculusManager.StartTimer(null);
            };
        }

        public override MultiplayerType MPType => MultiplayerType.Host;

        public override string Author => "18107";

        public override string Description => "Recreates the homunculus if it gets lost";
    }
}
