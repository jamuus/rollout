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
        velocity = GetComponent<Rigidbody> ().velocity;
    }

    void Awake()
    {
        Server.OpenConnection("127.0.0.1", 7777);
        Server.SendEndianness();
    }

    void FixedUpdate()
    {
        var sphero = SpheroManager.Instances["tty.Sphero-YBR-AMP-SPP"];
        float moveHorizontal = sphero.Position.x ;
        float moveVertical = sphero.Position.y;

        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 position = new Vector3(moveHorizontal, 0.5f, moveVertical);
        rb.position = position;
    }

    void OnApplicationQuit()
    {
        Server.CloseConnection();
    }
}
