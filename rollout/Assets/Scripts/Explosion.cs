using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
    private float explosionRadius;
    private float explosionPower;
    private float maxDamage;
    private float minDamage;
    //private UniversalHealth health;
	private AudioSource music;
	public AudioClip bang;
    // Use this for initialization
    void Start()
    {

    }


	public void SetSteroPan(AudioSource source, float x){
		float gridX = x;
		float posX = (gridX)/18.0f;
		source.panStereo = posX;
		float vol = (float) System.Math.Abs (posX) + 0.25f;
		if (vol > 1.0f) vol = 1.0f;
		else if (vol < 0.25f) vol = 0.25f;
		source.volume = vol;
	}

    public void Initialise(float givenRadius, float givenPower, float givenMaxDamage, float givenMinDamage)
    {
        //Set parameters
        explosionRadius = givenRadius;
        explosionPower = givenPower;
        maxDamage = givenMaxDamage;
        minDamage = givenMinDamage;
        float distance, proportionalDistance;
        int damage;
		music = GetComponent<AudioSource>();
        print("Explosion Initialised");

        //Get all nearby objects
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, explosionRadius);
        print("found " + nearbyObjects.Length + " objects");

        //Apply explodiness
        ParticleSystem particles = GetComponent<ParticleSystem>();
        print("Particle System: " + particles.maxParticles);
        particles.Stop();
        particles.Play();
		float x = particles.transform.position.x;
		SetSteroPan (music, x);
		music.PlayOneShot(bang);
		Debug.LogFormat ("{0} explosion", x);
		foreach (Collider body in nearbyObjects)
        {
            print("found collider");
            GameObject objectInExplosion = body.gameObject;
            Rigidbody rb = objectInExplosion.GetComponent<Rigidbody>();

            //Apply physical force
            if (rb != null)
                rb.AddExplosionForce(explosionPower, transform.position, explosionRadius);

			if (objectInExplosion.CompareTag("Enemy") || objectInExplosion.CompareTag("Player"))
            {
                distance = Vector3.Distance(transform.position, body.transform.position);

                print("Explosion Force Applied");
                //Give damage
                //Damage is calculated as a linear interpolation between the max and min damage wrt distance from explosion centre
                UniversalHealth health = objectInExplosion.GetComponent<UniversalHealth>();
                proportionalDistance = distance / explosionRadius;
                damage = (int)Mathf.Round(Mathf.Lerp(maxDamage, minDamage, proportionalDistance));

                if (objectInExplosion.CompareTag("Player") || objectInExplosion)
                {
                    GameObject shield = objectInExplosion.transform.Find("shield").gameObject;
                    if (shield.activeSelf) shield.GetComponent<Shield>().shieldCharge(damage);
					else health.damagePlayer (damage);
                }
            }
        }
        //Destroy(this.gameObject);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
