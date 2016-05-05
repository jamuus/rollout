using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
    private float explosionRadius;
    private float explosionPower;
    private float maxDamage;
    private float minDamage;
    //private UniversalHealth health;

    // Use this for initialization
    void Start()
    {

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

        print("Explosion Initialised");

        //Get all nearby objects
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, explosionRadius);
        print("found " + nearbyObjects.Length + " objects");

        //Apply explodiness
        ParticleSystem particles = GetComponent<ParticleSystem>();
        print("Particle System: " + particles.maxParticles);
        particles.Stop();
        particles.Play();

        foreach (Collider body in nearbyObjects)
        {
            print("found collider");
            GameObject objectInExplosion = body.gameObject;
            Rigidbody rb = objectInExplosion.GetComponent<Rigidbody>();

            //Apply physical force
            if (rb != null)
                rb.AddExplosionForce(explosionPower, transform.position, explosionRadius);

            if (objectInExplosion.name == "player1" || objectInExplosion.name == "player2")
            {
                distance = Vector3.Distance(transform.position, body.transform.position);

                print("Explosion Force Applied");
                //Give damage
                //Damage is calculated as a linear interpolation between the max and min damage wrt distance from explosion centre
                UniversalHealth health = objectInExplosion.GetComponent<UniversalHealth>();
                proportionalDistance = distance / explosionRadius;
                damage = (int)Mathf.Round(Mathf.Lerp(maxDamage, minDamage, proportionalDistance));
                if (objectInExplosion.CompareTag("Player"))
                {
                    GameObject shield = objectInExplosion.transform.Find("shield").gameObject;
                    if (shield.activeSelf) shield.GetComponent<Shield>().shieldCharge(damage);
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
