using UnityEngine;
using System.Collections.Generic;

public abstract class Region : MonoBehaviour
{
    public float            Rate;

    protected float lastEffectCall;

    private Bounds                  bounds;
    private HashSet<PlayerControl>  currentPlayers;
    private HashSet<PlayerControl>  previousPlayers;

    void Start()
    {
        lastEffectCall  = Time.time;
        bounds = GetComponent<Renderer>().bounds;
        currentPlayers = new HashSet<PlayerControl>();
        previousPlayers = new HashSet<PlayerControl>();
    }

    public bool Contains(PlayerControl player)
    {
        bool previousContains = previousPlayers.Contains(player);
        previousPlayers = currentPlayers;

        bool contains = bounds.Contains(player.transform.position);

        if (contains)
            currentPlayers.Add(player);
        else if (previousContains)
            currentPlayers.Remove(player);

        if (previousContains && !contains)
            OnPlayerLeave(player);

        if (!previousContains && contains)
            OnPlayerEnter(player);

        return contains;
    }

    public abstract void ApplyEffect(PlayerControl player);

    public virtual void OnPlayerLeave(PlayerControl player)
    {
        // Do nothing.
    }

    public virtual void OnPlayerEnter(PlayerControl player)
    {
        // Do nothing.
    }
}