using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    public float speed;
    public Vector3 velocity;
    public string horizontalAxis;
    public string verticalAxis;

	void Start()
	{
		velocity = GetComponent<Rigidbody>().velocity;
	}

    void FixedUpdate()
    {
        //Get the horizontal and vertical components of the input
        float moveHorizontal = Input.GetAxis(horizontalAxis);
        float moveVertical = Input.GetAxis(verticalAxis);

        //Get the sphero object
        Rigidbody rb = GetComponent<Rigidbody>();

        //Work out what direction to move in and how fast to do it
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        velocity = rb.velocity;

        //Apply the force to the sphero
        rb.AddForce(speed * movement);
    }
}
