using UnityEngine;

public class Reflexium : PerkEffect
{
    // PERK SYSTEM, written by Evan Montgomery

    [Min(1f)] public float reloadSpeedMultiplier = 2f;                              // Increased reload speed

    public override string PerkName => "Reflexium";                                 // Perk name
    public override int Cost => 3000;                                               // Perk price

    public override bool Apply(PerkSystem system)
    {
        system.ReloadSpeedMultiplier = Mathf.Max(1f, reloadSpeedMultiplier);        // Apply increased reload speed
        return true;
    }
}
