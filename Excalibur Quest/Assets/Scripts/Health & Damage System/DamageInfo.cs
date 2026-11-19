using UnityEngine;

public struct DamageInfo
{
    public int amount;
    public Vector3 hitPoint;
    public Vector3 hitDirection;
    public GameObject attacker;

    public DamageInfo(
        int amount,
        Vector3 hitPoint,
        Vector3 hitDirection,
        GameObject attacker)
    {
        this.amount = amount;
        this.hitPoint = hitPoint;
        this.hitDirection = hitDirection;
        this.attacker = attacker;
    }
}
