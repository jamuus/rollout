using UnityEngine;
using System.Collections;
using System;

public struct PowerUp
{
    public string name;
    public string description;
    public int value;
    public bool instant;
}

public class InitialisePowerUp:MonoBehaviour
{
    public PowerUp[] powerUps = new PowerUp[5];
    
    void Start()
    {
        PowerUp powerUp;
        powerUp.name = "Boost";
        powerUp.description = "Doubles movement speed";
        powerUp.value = 2;
        powerUp.instant = false;
        powerUps[0] = powerUp;

        powerUp.name = "Damage Enemy";
        powerUp.description = "Deals damage to enemy player";
        powerUp.value = 20;
        powerUp.instant = false;
        powerUps[1] = powerUp;

        powerUp.name = "Stun enemy";
        powerUp.description = "Freezes enemy";
        powerUp.value = 3;
        powerUp.instant = false;
        powerUps[2] = powerUp;

        powerUp.name = "Slow Down enemy";
        powerUp.description = "Slows down enemy player";
        powerUp.value = 2;
        powerUp.instant = false;
        powerUps[3] = powerUp;

        powerUp.name = "Regeneration";
        powerUp.description = "Player regenerates health continuously";
        powerUp.value = 1;
        powerUp.instant = false;
        powerUps[4] = powerUp;

    }

}
