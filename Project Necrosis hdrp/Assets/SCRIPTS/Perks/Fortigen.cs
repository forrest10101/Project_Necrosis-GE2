using UnityEngine;

public class Fortigen : PerkEffect
{
    // PERK SYSTEM, written by Evan Montgomery

    [Min(1f)] public float extraHealth = 20f;                           // Extra health

    public override string PerkName => "Fortigen";                      // Perk name
    public override int Cost => 2500;                                   // Perk price

    public override bool Apply(PerkSystem system)
    {
        if (system.playerHealth == null) return false;                  // Return if player health isn't being used
        system.playerHealth.AddMaximumHealth(extraHealth);              // Increase player health
        return true;
    }
}
