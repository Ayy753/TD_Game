using UnityEngine;

public abstract class StatusEffect
{
    public static string Name { get; set; }
    public static string Description { get; set; }
    public float RemainingDuration { get; set; }
    public abstract void OnTick();
    public abstract void StrengthenEffect(StatusEffect newEffect);
}

public class Poison : StatusEffect
{
    public float DamagePerTick { get; set; }

    public Poison(float damagePerTick, float duration)
    {
        DamagePerTick = damagePerTick;
        RemainingDuration = duration;
    }

    public override void OnTick()
    {
        //  lower duration, apply damage?
    }

    public override void StrengthenEffect(StatusEffect newEffect)
    {
        float damageRatio = ((Poison)newEffect).DamagePerTick / DamagePerTick;

        DamagePerTick += DamagePerTick * damageRatio;
        RemainingDuration += RemainingDuration * damageRatio;
    }
}
