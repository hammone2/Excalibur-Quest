using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatDirector : MonoBehaviour
{
    public static EnemyCombatDirector Instance { get; private set; }

    [Header("Combat Limits")]
    [SerializeField] private int maxAttackers = 2;
    [SerializeField] private int maxFlankers = 3;

    private readonly List<AIBrain> attackers = new();
    private readonly List<AIBrain> flankers = new();

    private void Awake()
    {
        Instance = this;
    }

    public bool RequestAttackSlot(AIBrain enemy)
    {
        CleanupLists();

        if (attackers.Contains(enemy))
            return true;

        if (attackers.Count >= maxAttackers)
            return false;

        attackers.Add(enemy);
        return true;
    }

    public void ReleaseAttackSlot(AIBrain enemy)
    {
        attackers.Remove(enemy);
    }

    public bool RequestFlankSlot(AIBrain enemy)
    {
        CleanupLists();

        if (flankers.Contains(enemy))
            return true;

        if (flankers.Count >= maxFlankers)
            return false;

        flankers.Add(enemy);
        return true;
    }

    public void ReleaseFlankSlot(AIBrain enemy)
    {
        flankers.Remove(enemy);
    }

    private void CleanupLists()
    {
        attackers.RemoveAll(e => e == null);
        flankers.RemoveAll(e => e == null);
    }
}
