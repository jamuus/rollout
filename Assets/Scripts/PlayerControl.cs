using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
    public float speed;
	public Vector3 velocity;
	void Start()
	{
		velocity = GetComponent<Rigidbody> ().velocity;
	}

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
		velocity = rb.velocity;

        rb.AddForce(speed * movement);
    }
}
