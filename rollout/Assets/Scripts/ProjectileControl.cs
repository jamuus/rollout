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
        //Remember to set the fire rates (the delay between each shot in seconds)
    //3) Add the weapon to the enum, ammo and fire rate arrays below and create a variable
    //4) Include the weapon firing and ammo reduction in the switch statement in Update

    //intialise the weapon structures
    enum Weapons : int { basicGun, homingLauncher, grenadeThrower };
    private int[] ammunition = new int[3];
    private int[] maxAmmo = new int[3];
    private float[] fireRates = new float[3];
    private int activeWeapon;

    //weapon variables
    private BasicGun basicGun;
    private HomingLauncher homingLauncher;
    private GrenadeThrower grenadeThrower;

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
        activeWeapon = (int)Weapons.basicGun;
        ammunition[(int)Weapons.basicGun] = -1;
        ammunition[(int)Weapons.homingLauncher] = 0;
        ammunition[(int)Weapons.grenadeThrower] = 0;

        //access the weapons
        basicGun = GetComponent<BasicGun>();
        fireRates[ConvertID(100)] = basicGun.fireRate;
        maxAmmo[ConvertID(100)] = -1;

        homingLauncher = GetComponent<HomingLauncher>();
        fireRates[ConvertID(101)] = homingLauncher.fireRate;
        maxAmmo[ConvertID(101)] = homingLauncher.maxAmmo;
		try {
	        grenadeThrower = GetComponent<GrenadeThrower>();
	        fireRates[ConvertID(102)] = grenadeThrower.fireRate;
	        maxAmmo[ConvertID(102)] = grenadeThrower.maxAmmo;
		} catch(Exception e) {
		}
    }

    public void Update()
    {
        //Only update if the game is active
        if (gameObject.GetComponent<PlayerControl>().gameStateId != 0) return;

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
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeActiveWeapon("grenadeThrower");
        }
    }

    //For shooting in game
    private void Shoot()
    {
        float sinceLastShot = Time.time - shootTime;
        //Checks if the weapon has ammunition and shoots according to fire rate
        if (ammunition[activeWeapon] != 0 && sinceLastShot >= (float)fireRates[activeWeapon])
        {
            //play the shooting sound
			music = gameObject.transform.Find("sound").gameObject;
			SoundManager manager = (SoundManager) music.GetComponent(typeof(SoundManager));
			manager.Shoot ();

            //fire the weapon and reduce ammunition as needed
            switch (activeWeapon)
            {
                case (int)Weapons.basicGun:
                    basicGun.Fire();
                    break;

                case (int)Weapons.homingLauncher:
                    homingLauncher.Fire(otherPlayer);
                    ReduceAmmo(101, 1);
                    break;

                case (int)Weapons.grenadeThrower:
                    grenadeThrower.Fire();
                    ReduceAmmo(102, 1);
                    break;
            }

            //set the time since last shot
            shootTime = Time.time;
        }
    }

    //For shooting from the app
    public void Shoot(Vector3 velocity)
    {
        float sinceLastShot = Time.time - shootTime;
        //Checks if the weapon has ammunition and shoots according to fire rate
        if (ammunition[activeWeapon] != 0 && sinceLastShot >= (float)fireRates[activeWeapon])
        {
            //play the shooting sound
			music = gameObject.transform.GetChild(0).gameObject;
			SoundManager manager = (SoundManager) music.GetComponent(typeof(SoundManager));
			manager.Shoot ();

            //fire the weapon and reduce ammunition as needed
            switch (activeWeapon)
            {
                case (int)Weapons.basicGun:
                    basicGun.Fire(velocity);
                    break;

                case (int)Weapons.homingLauncher:
                    homingLauncher.Fire(otherPlayer);
                    ReduceAmmo(101, 1);
                    break;

                case (int)Weapons.grenadeThrower:
                    grenadeThrower.Fire(velocity);
                    ReduceAmmo(102, 1);
                    break;
            }

            //set the time since last shot
            shootTime = Time.time;
        }
    }


    public void FixedUpdate()
    {
        //ADD CODE FOR SWITCHING WEAPONS AND ADDING AMMUNITION ON PICKUP
    }

    public void AddAmmo(int ID, int amount)
    {
        //if the ammo to add will exceed the max ammo limit, set it to the max
        if (ammunition[ConvertID(ID)] + amount <= maxAmmo[ConvertID(ID)])
            ammunition[ConvertID(ID)] += amount;
        else
            ammunition[ConvertID(ID)] = maxAmmo[ConvertID(ID)];

        print("Weapon " + ID + " now has " + ammunition[ConvertID(ID)] + " ammo ");
    }

    public void ReduceAmmo(int ID, int amount)
    {
        ammunition[ConvertID(ID)] -= amount;

        if (ammunition[ConvertID(ID)] <= 0)
            ChangeActiveWeapon(100);

    }

    public void ChangeActiveWeapon(int ID)
    {
        //toggle between basic gun and other weapons
        /*if(activeWeapon == 0) { activeWeapon = ConvertID(ID); }
        else { activeWeapon = 0; }*/
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
