using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// PERK SYSTEM, written by Evan Montgomery

// Add required perk components
[RequireComponent(typeof(RevitalX), typeof(Fortigen), typeof(Reflexium))]
[RequireComponent(typeof(Ballistix))]
public class PerkSystem : MonoBehaviour
{
    public static PerkSystem Instance { get; private set; }                                         // Access central system

    public PlayerHealth playerHealth;                                                               // Player health script
    public PointsManager pointsManager;                                                             // Player points script
    public Camera playerCamera;                                                                     // Player camera
    public TMP_Text promptText;                                                                     // Purchase prompt
    public KeyCode buyKey = KeyCode.E;                                                              // Purchase key
    [Min(0.1f)] public float interactionDistance = 3f;                                              // Purchase range
    public LayerMask interactionMask = ~0;                                                          // Detectable layers

    public float ReloadSpeedMultiplier { get; internal set; } = 1f;                                 // Reload speed boost
    public float DamageMultiplier { get; internal set; } = 1f;                                      // Bullet damage boost

    private readonly HashSet<Type> owned = new HashSet<Type>();                                     // Purchased perk types
    private readonly Dictionary<Type, PerkEffect> perks = new Dictionary<Type, PerkEffect>();       // Available perk scripts

    private void Awake()
    {
        if (Instance != null && Instance != this)                                                   // Check for duplicates
        {
            Debug.LogError("Only one PerkSystem should be in the scene.", this);
            enabled = false;                                                                        // Disable duplicate system
            return;
        }

        Instance = this;                                                                            // Register central system

        foreach (PerkEffect perk in GetComponents<PerkEffect>())                                    // Find attached perks
        {
            perks[perk.GetType()] = perk;                                                           // Store perk reference
            if (perk.uiIcon != null) perk.uiIcon.SetActive(false);                                  // Hide perk icon
        }

        SetPrompt("");                                                                              // Clear starting prompt
    }

    private void Start()
    {
        if (pointsManager == null) pointsManager = PointsManager.Instance;                          // Find points manager
        if (playerCamera == null) playerCamera = Camera.main;                                       // Find main camera
    }

    private void Update()
    {
        SetPrompt("");                                                                              // Clear previous prompt
        if (Time.timeScale == 0f || playerCamera == null) return;                                   // Skip paused or missing camera

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);         // Aim from camera

        // Check first solid object
        if (!Physics.Raycast(ray, out RaycastHit hit, interactionDistance,
            interactionMask, QueryTriggerInteraction.Ignore)) return;

        PerkMachine machine = hit.collider.GetComponentInParent<PerkMachine>();                     // Find machine script
        if (machine == null || !machine.isActiveAndEnabled || machine.perk == null) return;         // Skip invalid machines
        if (!perks.TryGetValue(machine.perk.GetType(), out PerkEffect perk)) return;                // Find matching perk

        if (owned.Contains(perk.GetType()))                                                         // Check existing ownership
        {
            SetPrompt(perk.PerkName + " owned");                                                    // Show owned message
            return;
        }

        if (pointsManager == null || pointsManager.uiManager == null)                               // Check required managers
        {
            SetPrompt("Perk points manager or UI manager is missing");
            return;
        }

        bool affordable = pointsManager.Points >= perk.Cost;                                        // Check available points

        SetPrompt(affordable
            ? "[" + buyKey + "] Buy " + perk.PerkName + " - " + perk.Cost + " points"
            : perk.PerkName + " - " + perk.Cost + " points (not enough points)");                   // Show price or insufficient points

        if (Input.GetKeyDown(buyKey) && TryBuy(perk))                                               // Attempt perk purchase
            SetPrompt(perk.PerkName + " owned");                                                    // Confirm successful purchase
    }

    public bool TryBuy(PerkEffect requestedPerk)
    {
        // Check system and request
        if (!isActiveAndEnabled || Time.timeScale == 0f || requestedPerk == null)
            return false;

        if (!perks.TryGetValue(requestedPerk.GetType(), out PerkEffect perk)) return false;         // Find registered perk

        if (owned.Contains(perk.GetType()) || pointsManager == null ||
            pointsManager.uiManager == null || pointsManager.Points < perk.Cost) return false;      // Check ownership, managers and points

        if (!perk.Apply(this)) return false;                                                        // Apply perk effect

        pointsManager.RemovePoints(perk.Cost);                                                      // Deduct purchase cost
        owned.Add(perk.GetType());                                                                  // Record purchased perk
        if (perk.uiIcon != null) perk.uiIcon.SetActive(true);                                       // Show perk icon
        return true;                                                                                // Purchase successful
    }

    private void SetPrompt(string text)
    {
        if (promptText != null) promptText.text = text;                                             // Update prompt text
    }

    private void OnDisable()
    {
        if (Instance == this) SetPrompt("");                                                        // Clear disabled system prompt
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;                                                      // Clear central reference
    }
}