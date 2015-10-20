using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
    public float speed;
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(speed * movement);
    }
}
