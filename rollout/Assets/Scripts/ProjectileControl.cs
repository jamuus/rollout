using UnityEngine;
using System.Collections;
using System;

public class ProjectileControl : MonoBehaviour
{
    private Vector3 velocity;
    public Projectile projectile;
    private GameObject music;

    //***TO ADD WEAPONS***
    //1) Write the weapon behaviour in a separate script
    //2) Add the weapon script as a component to each player, using projectile prefabs as needed
    //3) Add the weapon to the enum and ammo array below and create a variable
    //4) Include the weapon firing and ammo reduction in the switch statement in Update

    //intialise the weapon structures
    enum Weapons : int { basicGun, homingLauncher };
    private int[] ammunition = new int[2];
    private int activeWeapon;

    //weapon variables
    private BasicGun basicGun;
    private HomingLauncher homingLauncher;

    //variables to aid firing
    private Vector3 projectilePosition;
    private GameObject otherPlayer;
    private Quaternion projectileRotation;
    private float shootTime;


    void Start()
    {
        //find the other player
        if (gameObject.name == "player1") {
            otherPlayer = GameObject.Find("player2");
        } else if (gameObject.name == "player2") {
            otherPlayer = GameObject.Find("player1");
        }

        //Set the initial weapon to the basic gun
        activeWeapon = (int)Weapons.homingLauncher;
        ammunition[(int)Weapons.basicGun] = -1;
        ammunition[(int)Weapons.homingLauncher] = 20;

        //access the weapons
        basicGun = GetComponent<BasicGun>();
        homingLauncher = GetComponent<HomingLauncher>();
    }

    public void Update()
    {
        //Checks if the player is trying to fire a weapon
        if ((Input.GetButton("Fire1") && gameObject.name == ("player1")) || (Input.GetButton("Fire2") && gameObject.name == ("player2")))
        {
            Shoot();
        }

        //change the active weapon based on key press
        //will not be in the final game, testing purposes only
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeActiveWeapon("basicGun");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeActiveWeapon("homingLauncher");
        }
    }

    //For shooting in game
    private void Shoot()
    {
        //Checks if the weapon has ammunition
        if (ammunition[activeWeapon] != 0)
        {
            //play the shooting sound
            music = GameObject.Find("Music");
            SoundManager manager = (SoundManager)music.GetComponent(typeof(SoundManager));
            manager.Shoot(gameObject);

            //fire the weapon and reduce ammunition as needed
            //GET CONTINUOUS FIRE WORKING WITH FIRE RATE
            switch (activeWeapon)
            {
                case (int)Weapons.basicGun:
                    print(basicGun.fireRate);
                    if (Time.time - shootTime >= basicGun.fireRate)
                    {
                        basicGun.Fire();
                        shootTime = Time.time;
                    }         
                    break;

                case (int)Weapons.homingLauncher:
                    homingLauncher.Fire(otherPlayer);
                    ammunition[(int)Weapons.homingLauncher] -= 1;
                    break;
            }
            print("Shooting done");
        }
    }

    //For shooting from the app
    //public void Shoot(Vector3 velocity)
    //{
    //    if (ammunition[activeWeapon] != 0)
    //    {

    //        //play the shooting sound
    //        music = GameObject.Find("Music");
    //        SoundManager manager = (SoundManager)music.GetComponent(typeof(SoundManager));
    //        manager.Shoot(gameObject);

    //        //fire the weapon and reduce ammunition as needed
    //        switch (activeWeapon)
    //        {
    //            case (int)Weapons.basicGun:
    //                basicGun.Fire();
    //                break;

    //            case (int)Weapons.homingLauncher:
    //                homingLauncher.Fire(otherPlayer);
    //                ammunition[(int)Weapons.homingLauncher] -= 1;
    //                break;
    //        }
    //    }
    //}


    public void FixedUpdate()
    {
        //ADD CODE FOR SWITCHING WEAPONS AND ADDING AMMUNITION ON PICKUP
    }

    public void AddAmmo(int ID, int amount)
    {
        ammunition[ConvertID(ID)] += amount;
    }

    public void ChangeActiveWeapon(int ID)
    {
        activeWeapon = ConvertID(ID);
    }

    public void ChangeActiveWeapon(string weaponString)
    {
        activeWeapon = (int)(Weapons)Enum.Parse(typeof(Weapons), weaponString, true);
    }

    public int ConvertID(int ID)
    {
        int convertedID = ID - 100;
        return convertedID;
    }
}

