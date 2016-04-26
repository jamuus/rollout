using UnityEngine;
using System.Collections;

public class HomingLauncher : MonoBehaviour
{
    private int id = 101;
    public HomingMissile homingMissile;
    private GameObject otherPlayer;
    private Vector3 velocity;
    private Quaternion projectileRotation;
    private Vector3 projectilePosition;
    public int ammunition = 2;
    public int maxAmmo = 5;
    public float fireRate = 1;


    public void Fire(GameObject givenOtherPlayer)
    {
        otherPlayer = givenOtherPlayer;
        
        //if the other player is alive, set the direction of the missile towards it, otherwise set it in direction of movement
        if (otherPlayer != null)
        {
            //Set the spawn position and rotation to be towards the other player
            velocity = otherPlayer.transform.position - transform.position;
            projectileRotation = Quaternion.LookRotation(otherPlayer.transform.position - transform.position, Vector3.up);
        }
        else
        {
            //Set the spawn position and rotation to be in the direction of movement
            velocity = GetComponent<Rigidbody>().velocity;
            projectileRotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);
        }

        projectilePosition = transform.position + (velocity.normalized * 2);
        var spawnedMissile = (HomingMissile)Instantiate(homingMissile, projectilePosition, projectileRotation);
        spawnedMissile.Initialise(velocity, otherPlayer);
    }
    
}
