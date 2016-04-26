using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    private int id;
    private Vector3 velocity;
    private float fireRate;

    public int ID()
    {
        return id;
    }

    public Vector3 Velocity()
    {
        return velocity;
    }

    public float FireRate()
    {
        return fireRate;
    }
}
