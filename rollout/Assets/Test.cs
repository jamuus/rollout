using UnityEngine;
using System.Collections;

using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Test : MonoBehaviour
{

    // Use this for initialization
    void Awake ()
    {
        /*var message = new Server.Message(Server.MessageType.RollSphero);
        message.AddContent(180.0f);
        message.AddContent(0.25f);
        message.AddContent("SPHERO-BOO");

        Server.OpenConnection("127.0.0.1", 7777);

        Server.SendEndianness();
        Server.Send(message);*/

        Server.Name = "Rollout Server";
        Server.StartListening(7777);

        Sphero boo = new Sphero();
        boo.DeviceName = "tty.Sphero-BOO-AMP-SPP";
        boo.Health = 96.4f;
        boo.Shield = 44.5f;
        boo.Weapons.Add(new SpheroWeapon(SpheroWeaponType.RailGun));
        boo.BatteryVoltage = 7.2f;
        SpheroManager.Instances[boo.DeviceName] = boo;
    }

    void LateUpdate()
    {
        SpheroManager.SendStateToControllers();
    }

    void OnApplicationQuit()
    {
        //Server.CloseConnection();

        Server.StopListening();
    }
}
