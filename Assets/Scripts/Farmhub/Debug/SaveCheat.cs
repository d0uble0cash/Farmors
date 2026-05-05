using UnityEngine;
using UnityEngine.InputSystem;

public class SaveCheat : MonoBehaviour
{
    private void Update()
    {
        if (SaveSystem.I == null)
            return;

        // Save game (press S)
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            SaveSystem.I.Save();
            Debug.Log("Manual save triggered.");
        }

        // Load game (press L)
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            SaveSystem.I.Load();
            Debug.Log("Manual load triggered.");
        }

        // Delete save (press Delete)
        if (Keyboard.current.deleteKey.wasPressedThisFrame)
        {
            SaveSystem.I.DeleteSave();
            Debug.Log("Save deleted.");
        }

        // Check save exists (press H)
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            Debug.Log(SaveSystem.I.HasSave()
                ? "Save file exists."
                : "No save file found.");
        }
    }
}