using UnityEngine;

public class DamageSlowRegion : Region
{
    public override void ApplyEffect(PlayerControl player)
    {
        //if (Time.time > lastEffectCall + Rate)
        {
            Debug.LogFormat("INSIDE");
            //player.GetComponent<UniversalHealth>().damagePlayer(2);
            lastEffectCall = Time.time;
        }
    }
}