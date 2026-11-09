using UnityEngine;

public class Ballistix : PerkEffect
{
    // PERK SYSTEM, written by Evan Montgomery

    [Min(1f)] public float damageMultiplier = 2f;                               // Damage multiplier

    public override string PerkName => "Ballistix";                             // Perk name
    public override int Cost => 2000;                                           // Perk price

    public override bool Apply(PerkSystem system)
    {
        system.DamageMultiplier = Mathf.Max(1f, damageMultiplier);              // Increase bullet damage
        return true;
    }
}
