using UnityEngine;
using System.Collections.Generic;

public class RegionManager : MonoBehaviour
{
    public List<Region>     Regions;
    public PlayerControl    Player1;
    public PlayerControl    Player2;

    void Update()
    {
        foreach (Region region in Regions)
        {
            if (region.Contains(Player1.transform.position))
                region.ApplyEffect(Player1);
            if (region.Contains(Player2.transform.position))
                region.ApplyEffect(Player2);
        }
    }
}