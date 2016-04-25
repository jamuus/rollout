using UnityEngine;

public class TextureScroll : MonoBehaviour
{
    public Vector2 speed;

    private Renderer renderer;

    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        renderer.material.SetTextureOffset("_MainTex", Time.time * speed);
    }
}
