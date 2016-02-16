using System;
using System.Collections.Generic;

using UnityEngine;

public class MainThread : MonoBehaviour
{
    private static Queue<Action>        actionQueue;
    private static System.Object        lockObject;

    // Initialisation performed here.
    void Awake()
    {
        lockObject  = new System.Object();
        actionQueue = new Queue<Action>();

        // Load PowerUps from file.
        PowerUpManager.Initialise();

        // This must be called before server is started.
        SpheroManager.Initialise();

        // Initialise server and start listening for controllers & node.
        Server.Name = "Rollout Server";
        Server.StartListening(7777);

        // Add a test Sphero for debugging.
        Sphero boo = new Sphero();
        boo.DeviceName = "tty.Sphero-BOO-AMP-SPP";
        boo.Health = 96.4f;
        boo.Shield = 44.5f;
        boo.Weapons.Add(new SpheroWeapon(SpheroWeaponType.RailGun));
        boo.BatteryVoltage = 7.2f;
        boo.UnityObject = SpheroManager.boo; // TODO This needs to be automated.
        boo.UnityProjectileControl = SpheroManager.boo.GetComponent<ProjectileControl>();
        SpheroManager.Instances[boo.DeviceName] = boo;

        SpheroManager.boo.sphero = boo;
    }

    // Each tick, check if there are any actions that have been queued, and if
    // so perform them.
    void Update()
    {
        lock (lockObject)
        {
            foreach (Action action in actionQueue)
                action();
            actionQueue.Clear();
        }
    }

    // After everything for current tick has been processed, send state to
    // controller.
    void LateUpdate()
    {
        SpheroManager.SendStateToControllers();
    }

    // Shutdown.
    void OnApplicationQuit()
    {
        Server.StopListening();
    }

    // Add an action to complete on the main thread in the next tick.
    public static void EnqueueAction(Action action)
    {
        lock (lockObject)
        {
            actionQueue.Enqueue(action);
        }
    }
}