using UnityEngine;

[System.Serializable]
public class DamageData
{
    public int damageAmount;
    public float hitStopDuration;
}

public struct DamageInfo
{
    public int amount;
    public Vector3 hitPoint;
    public Vector3 hitDirection;
    public GameObject attacker;
    public float hitStopDuration;

    public DamageInfo(
        int amount,
        Vector3 hitPoint,
        Vector3 hitDirection,
        GameObject attacker,
        float hitStopDuration)
    {
        this.amount = amount;
        this.hitPoint = hitPoint;
        this.hitDirection = hitDirection;
        this.attacker = attacker;
        this.hitStopDuration = hitStopDuration;
    }
}
