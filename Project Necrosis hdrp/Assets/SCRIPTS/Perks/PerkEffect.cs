using UnityEngine;

public abstract class PerkEffect : MonoBehaviour
{
    // PERK SYSTEM, written by Evan Montgomery

    public GameObject uiIcon;                                       // Perk UI icon

    public abstract string PerkName { get; }                        // Find perk name
    public abstract int Cost { get; }                               // Perk cost
    public abstract bool Apply(PerkSystem system);                  // Apply perk
}
