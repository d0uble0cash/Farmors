using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SeedSelectionCheat : MonoBehaviour
{
    [SerializeField] private List<ItemDefinition> seeds = new();

    private void Update()
    {
        if (SeedSelection.I == null || seeds.Count == 0)
            return;

        // Loop through seeds and map to number keys (1–9)
        for (int i = 0; i < seeds.Count && i < 9; i++)
        {
            if (IsNumberKeyPressed(i + 1))
            {
                SeedSelection.I.SelectSeed(seeds[i]);
            }
        }
    }

    private bool IsNumberKeyPressed(int number)
    {
        return number switch
        {
            1 => Keyboard.current.digit1Key.wasPressedThisFrame,
            2 => Keyboard.current.digit2Key.wasPressedThisFrame,
            3 => Keyboard.current.digit3Key.wasPressedThisFrame,
            4 => Keyboard.current.digit4Key.wasPressedThisFrame,
            5 => Keyboard.current.digit5Key.wasPressedThisFrame,
            6 => Keyboard.current.digit6Key.wasPressedThisFrame,
            7 => Keyboard.current.digit7Key.wasPressedThisFrame,
            8 => Keyboard.current.digit8Key.wasPressedThisFrame,
            9 => Keyboard.current.digit9Key.wasPressedThisFrame,
            _ => false
        };
    }
}