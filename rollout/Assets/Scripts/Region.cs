using UnityEngine;
using System.Collections.Generic;

public abstract class Region : MonoBehaviour
{
    public List<Vector2>    Points;
    public float            Rate;

    protected float lastEffectCall;

    void Start()
    {
        lastEffectCall = Time.time;
    }

    public bool Contains(Vector3 v)
    {
        int j = Points.Count - 1;
        bool inside = false;

        for (int i = 0; i < Points.Count; j = i++)
        {
            if (((Points[i].y <= v.z && v.z < Points[j].y) || (Points[j].y <= v.z && v.z < Points[i].y)) &&
                (v.x < (Points[j].x - Points[i].x) * (v.z - Points[i].y) / (Points[j].y - Points[i].y) + Points[i].x))
            {
                inside = !inside;
            }
        }

        return inside;
    }

    public abstract void ApplyEffect(PlayerControl player);
}