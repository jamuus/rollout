// using UnityEngine;

// using System;
// using System.Collections.Generic;
// using System.Net;
// using System.Net.Sockets;
// using System.Threading;

// public class Test : MonoBehaviour
// {
//     private static Queue<Action>    actionQueue;
//     private static System.Object    lockObject = new System.Object();

//     public static void QueueOnMainThread(Action action)
//     {
//         lock(lockObject)
//         {
//             actionQueue.Enqueue(action);
//         }
//     }


//     // Use this for initialization
//     void Awake ()
//     {
//         /*var message = new Server.Message(Server.MessageType.RollSphero);
//         message.AddContent(180.0f);
//         message.AddContent(0.25f);
//         message.AddContent("SPHERO-BOO");

//         Server.OpenConnection("127.0.0.1", 7777);

//         Server.SendEndianness();
//         Server.Send(message);*/

//         actionQueue = new Queue<Action>();

//         SpheroManager.Initialise();

//         Server.Name = "Rollout Server";
//         Server.StartListening(7777);

//         Sphero boo = new Sphero();
//         boo.DeviceName = "tty.Sphero-BOO-AMP-SPP";
//         boo.Health = 96.4f;
//         boo.Shield = 44.5f;
//         boo.Weapons.Add(new SpheroWeapon(SpheroWeaponType.RailGun));
//         boo.BatteryVoltage = 7.2f;
//         boo.UnityObject = SpheroManager.boo; // TODO This needs to be automated.
//         boo.UnityProjectileControl = SpheroManager.boo.GetComponent<ProjectileControl>();
//         SpheroManager.Instances[boo.DeviceName] = boo;
//     }

//     void Update()
//     {
//         lock (lockObject)
//         {
//             foreach (Action action in actionQueue)
//                 action();

//             actionQueue.Clear();
//         }
//     }

//     void LateUpdate()
//     {
//         SpheroManager.SendStateToControllers();
//     }

//     void OnApplicationQuit()
//     {
//         //Server.CloseConnection();

//         Server.StopListening();
//     }
// }
