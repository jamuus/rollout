using UnityEngine;
using System.Collections;

public class Player2Control : MonoBehaviour
{
    public float speed;
	public Vector3 velocity;
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal2");
        float moveVertical = Input.GetAxis("Vertical2");
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
		velocity = rb.velocity;
        rb.AddForce(speed * movement);
    }
}
