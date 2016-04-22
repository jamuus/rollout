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
        ammunition[(int)Weapons.homingLauncher] = 20;

        //access the weapons
        basicGun = GetComponent<BasicGun>();
        homingLauncher = GetComponent<HomingLauncher>();
    }

    public void Update()
	{	if (gameObject.GetComponent<PlayerControl> ().gameStateId == 0) {
			//Checks if the player is trying to fire a weapon
			if ((Input.GetButtonDown ("Fire1") && gameObject.name == ("player1")) || (Input.GetButtonDown ("Fire2") && gameObject.name == ("player2"))) {
				music = GameObject.Find ("Music");
				SoundManager manager = (SoundManager)music.GetComponent (typeof(SoundManager));
				manager.Shoot (gameObject);

				//Checks if the weapon has ammunition
				if (ammunition [activeWeapon] != 0) {
					//fire the weapon and reduce ammunition as needed
					switch (activeWeapon) {
					case (int)Weapons.basicGun:
						basicGun.Fire ();
						break;

					case (int)Weapons.homingLauncher:
						homingLauncher.Fire (otherPlayer);
						ammunition [(int)Weapons.homingLauncher] -= 1;
						break;
					}
				}
			}
		}
    }

    private void Shoot()
    {
        //Get the velocity of the player2
        velocity = GetComponent<Rigidbody>().velocity;

        //If the player isn't moving just hard code it
        if (velocity.magnitude == 0) velocity = new Vector3(1f, 0f, 0f);

        //Call shoot
        Shoot(velocity);
    }

    public void Shoot(Vector3 velocity)
    {
        //Spawn the projectile outside of the player in the direction you are aiming
        projectilePosition = transform.position + velocity.normalized;
        var spawnedProjectile = (Projectile)Instantiate(projectile, projectilePosition, transform.rotation);

        spawnedProjectile.Initialise(velocity);
    }


    public void FixedUpdate()
    {
        //ADD CODE FOR SWITCHING WEAPONS AND ADDING AMMUNITION ON PICKUP
    }

    public void AddAmmo(string weaponString, int amount)
    {
        var weaponNum = (Weapons)Enum.Parse(typeof(Weapons), weaponString, true);
        ammunition[(int)weaponNum] += amount;
    }

    public void ChangeActiveWeapon(string weaponString)
    {
        var weaponNum = (Weapons)Enum.Parse(typeof(Weapons), weaponString, true);
        activeWeapon = (int)weaponNum;
    }

    public int ConvertID(int ID)
    {
        int convertedID = ID - 100;
        return convertedID;
    }
}
