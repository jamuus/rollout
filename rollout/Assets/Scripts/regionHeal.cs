using UnityEngine;
using System.Collections;

public class RegionHeal : Region {

	public int magnitude = 0;

	// Use this for initialization
	public override void ApplyEffect(PlayerControl player)
	{
		if (Time.time > lastEffectCall + Rate)
		{
			player.GetComponent<UniversalHealth>().healPlayer(magnitude);
			lastEffectCall = Time.time;
		}
	}

}
