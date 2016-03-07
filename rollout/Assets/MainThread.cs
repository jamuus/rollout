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

        Debug.LogFormat(".NET/Mono Version: {0}.", System.Environment.Version);

#if SOFTWARE_MODE
        Debug.LogFormat("WARNING: Running in software mode.");
#endif

        // Load PowerUps from file.
        PowerUpManager.Initialise();

        // This must be called before server is started.
        SpheroManager.Initialise();

        SpectatorManager.Initialise();

        // Initialise server and start listening for controllers & node.
        Server.Name = "Rollout Server";
        Server.StartListening(7777);

#if SOFTWARE_MODE
        Debug.LogFormat("WARNING: Running in software mode.");


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

        Sphero ybr = new Sphero();
        ybr.DeviceName = "tty.Sphero-YBR-AMP-SPP";
        ybr.Health = 96.4f;
        ybr.Shield = 44.5f;
        ybr.Weapons.Add(new SpheroWeapon(SpheroWeaponType.RailGun));
        ybr.BatteryVoltage = 7.2f;
        ybr.UnityObject = SpheroManager.ybr; // TODO This needs to be automated.
        ybr.UnityProjectileControl = SpheroManager.ybr.GetComponent<ProjectileControl>();
        SpheroManager.Instances[ybr.DeviceName] = ybr;

        SpheroManager.ybr.sphero = ybr;
#endif

        // Testing events.
        SpectatorManager.VoteWinnerDetermined += OnEventWinnerDetermined;
    }

    private void OnEventWinnerDetermined(object sender, VoteEventArgs args)
    {
        Debug.LogFormat("VOTE WINNER: {0}, VOTES: {1}.", args.Id, args.Votes);
        MainThread.EnqueueAction(() =>
        {
            SpectatorManager.EventManager.triggerEvent(args.Id);
        });
    }

    // Each tick, check if there are any actions that have been queued, and if
    // so perform them.
    void Update()
    {
        lock (lockObject) {
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
        lock (lockObject) {
            actionQueue.Enqueue(action);
        }
    }
}
