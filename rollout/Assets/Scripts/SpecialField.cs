using UnityEngine;
using System.Collections;

// Field has to have Is Trigger option Enabled on their collider.

// Behaviour Options which specify the behaviour of the fields:
// 0 : damagePlayer
// 1 : healPlayer
// 2 : destroyPlayer
// 3 : powerUp
// 4 : weapon only


public class SpecialField : MonoBehaviour
{
    public int behaviourOption = 0; // damage health
    public int magnitude = 1; //behaviour's magnitude
    public bool isVolatile = false; // is field destroyed when triggered
    public int powerUpID;
    public int weaponID = -1;
    private PowerUp powerUp;
	private bool active = true;
    private ProjectileControl projectileControl;
    private int ammoAmount;
	public GameObject music;
    // Use this for initialization
    void Start ()
    {
        renderColor(behaviourOption);
		setRandomPowerup ();
    }

    // Update is called once per frame
    void Update ()
    {

    }

    void OnTriggerEnter(Collider col)
    {
		//If the field affects the player
		if (col.gameObject.tag == "Player" && this.active == true) {
            //Get the player object
            GameObject player = col.gameObject;

            switch (behaviourOption) {
            case 0:
                damagePlayer(player);
                break;
            case 1:
                healPlayer(player);
                break;
            case 2:
                destroyPlayer(player);
                break;
			case 3:
                //50% chance to add powerup or weapon
				if (Random.Range (0, 1) == 0) {
					GameObject container = GameObject.Find ("Container");
					powerUp = container.GetComponent<InitialisePowerUp> ().powerUps [powerUpID];
					givePowerUp (player);
					//AddWeapon(player, -1);
					Debug.LogFormat("here1");
				} else {
					AddWeapon (player, -1);
					Debug.LogFormat("here 2");
				}
                break;
            case 4:
                AddWeapon(player, weaponID);
                break;
            default:
                print("Wrong field ID");
                break;
            }
			StartCoroutine(fieldTimeout(10));

			if (isVolatile) {

                Destroy(gameObject);
            }
        }
    }

    public void AddWeapon(GameObject player, int randomWepID)
    {
        //get a random weapon ID

        if (randomWepID < 100)
            randomWepID = Random.Range(101, 104);

        //determine the right amount of ammo to add for the weapon
        switch(randomWepID)
        {
            //homing launcher
            case 101:
                ammoAmount = player.GetComponent<HomingLauncher>().ammunition;
                break;
            //grenade thrower
            case 102:
                ammoAmount = player.GetComponent<GrenadeThrower>().ammunition;
                break;
            case 103:
                ammoAmount = player.GetComponent<MachineGun>().ammunition;
                break;
            case 104:
                ammoAmount = player.GetComponent<Shotgun>().ammunition;
                break;
        }

        projectileControl = player.GetComponent<ProjectileControl>();
        projectileControl.AddAmmo(randomWepID, ammoAmount);
        print("Weapon " + randomWepID + " Added to " + player.name);
		if (!music || music.transform.parent.gameObject != player) music = player.transform.Find("sound").gameObject;
		SoundManager manager = (SoundManager)music.GetComponent (typeof(SoundManager));
		manager.PickPowerUp ();
        //Add the weapon to the app
		if (player.GetComponent<PlayerControl>().sphero != null)
			player.GetComponent<PlayerControl>().sphero.PowerUps.Add(new SpheroPowerUp((SpheroPowerUpType)randomWepID));
    }

    public void setPowerUpID(int id)
	{
		this.powerUpID = id;
	}

    void renderColor(int option)
    {
		Color color = new Color();
        Renderer rend = GetComponent<Renderer>();
        //rend.material.shader = Shader.Find("Standard");

		switch (option) {
        case 0:
            color = new Color(.8f, .1f, .8f, 0.6f); //red
            break;
        case 1:
            color = new Color(.1f, .8f, .1f, 1f); // green
            break;
        case 2:
            color = new Color(.8f, .3f, .1f, 1f); // aggressive red
            break;
        case 3:
            color = new Color(.3f, .5f, .3f, 1f); // yellow
            break;
        case 4:
            color = new Color(.25f, .25f, .8f, 1f); // blue
            break;
        }

        //Set the colour
		rend.material.SetColor("_EmissionColor", new Color(0f,0f,0f,0f));
		rend.material.SetColor("_Color", color);
    }

	void renderColor(float r, float g, float b, float a)
	{
		Color color = new Color (r, g, b, a);
		Renderer rend = GetComponent<Renderer>();
		//rend.material.SetColor("_EmissionColor", Color.black);
				rend.material.SetColor("_EmissionColor", new Color(0.5f,0.5f,0.5f,0.2f));
		rend.material.SetColor("_Color", color);
	}

	void renderColor(float alpha)
	{
		Renderer rend = GetComponent<Renderer> ();
		Color color = rend.material.color;
		color.a = alpha;
		rend.material.SetColor("_Color", color);
	}

    //Behaviour functions
    void damagePlayer(GameObject player)
    {
        UniversalHealth health = player.GetComponent<UniversalHealth> ();
        health.damagePlayer (magnitude);
        //player.GetComponent<PlayerControl>().sphero.Health -= magnitude;
    }

    void healPlayer(GameObject player)
    {
        UniversalHealth health = player.GetComponent<UniversalHealth> ();
        health.healPlayer (magnitude);
        // TODO healing sphero.
    }

    void destroyPlayer(GameObject player)
    {
        Destroy(player);
    }

    void givePowerUp(GameObject player)
    {
        player.GetComponent<PlayerControl>().AddPowerUp(powerUp);
    }

	IEnumerator fieldTimeout(float seconds)
	{
		active = false;
		print(Time.time);
		renderColor (0.2f); //alpha
		yield return new WaitForSeconds (seconds);
		renderColor (behaviourOption); //alpha
		print(Time.time);
		setRandomPowerup ();
		active = true;
	}

	void setRandomPowerup (){
        powerUpID = (int)Random.Range (0, 6);
	}
}

