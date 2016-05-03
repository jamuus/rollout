using UnityEngine;
using System.Collections;

public class RegionPoints : Region {
	private int[] points;

	void Start() {
		GameObject.FindGameObjectsWithTag("player");
	}

	public override void ApplyEffect(PlayerControl player)
	{
		
	}
}
