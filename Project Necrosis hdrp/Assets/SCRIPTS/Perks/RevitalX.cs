using UnityEngine;

public class RevitalX : PerkEffect
{
    // PERK SYSTEM, written by Evan Montgomery

    [Min(1f)] public float healingSpeedMultiplier = 2f;                     // Increased healing speed

    public override string PerkName => "Revital-X";                         // Perk name
    public override int Cost => 500;                                        // Perk price

    public override bool Apply(PerkSystem system)
    {
        if (system.playerHealth == null) return false;
        system.playerHealth.ApplyHealingSpeed(healingSpeedMultiplier);      // Apply increased healing speed
        return true;
    }
}
