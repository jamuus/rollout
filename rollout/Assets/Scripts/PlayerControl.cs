using UnityEngine;
using System.Collections;
using Rewired;

public class PlayerControl : MonoBehaviour
{
    public float speed;
    public Vector3 velocity;
    public string horizontalAxis;
    public string verticalAxis;
    private Player player;

    void Start()
    {
        velocity = GetComponent<Rigidbody> ().velocity;
    }

    void Awake()
    {
        Server.OpenConnection("127.0.0.1", 7777);
        Server.SendEndianness();

        player = ReInput.players.GetPlayer(0);
    }

    void FixedUpdate()
    {
        Sphero sphero;
        if (SpheroManager.Instances.TryGetValue("tty.Sphero-YBR-AMP-SPP", out sphero)) {
            // move ingame sphero
            float moveHorizontal = sphero.Position.x;
            float moveVertical = sphero.Position.y;

            Rigidbody rb = GetComponent<Rigidbody>();
            Vector3 position = new Vector3(moveHorizontal, 0.5f, moveVertical);
            rb.position = position;

            // Debug.Log(string.Format("{0}, {1}", moveHorizontal, moveVertical));

            // get controller input and omve sphero
            float X = player.GetAxis("Horizontalx");
            float Y = player.GetAxis("Verticalx");

            float force = Mathf.Sqrt(Mathf.Pow(X, 2)
                                     + Mathf.Pow(Y, 2));
            float direction = Vector2.Angle(new Vector2(0, 1), new Vector2(X, Y));
            if (X < 0) direction = -direction + 360.0f;
            // direction = Mathf.Rad2Deg * direction;

            sphero.Roll(direction, force);
        }
    }

    // Debug.Log(string.Format("{0}, {1}", controllerHorizontal, controllerVertical));
    // Debug.Log(string.Format("{0}", player));


    void OnApplicationQuit()
    {
        Server.CloseConnection();
    }

}