using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
    public float speed;
	public Vector3 velocity;
	public string horizontalAxis;
	public string verticalAxis;
	void Start()
	{
		velocity = GetComponent<Rigidbody> ().velocity;
	}

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis(horizontalAxis);
        float moveVertical = Input.GetAxis(verticalAxis);
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
		velocity = rb.velocity;

        rb.AddForce(speed * movement);
    }
}
