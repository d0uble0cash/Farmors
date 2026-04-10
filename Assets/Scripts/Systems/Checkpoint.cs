using UnityEngine;
using UnityEngine.SceneManagement;

/// Attach to a Bench or Portal GameObject in the world.
/// When the player interacts with it:
///   - Saves the checkpoint position + current scene
///   - Optionally lets the player warp back to the farm scene
///
/// Interact by pressing the "Interact" action (wire in PlayerInput) OR
/// call Checkpoint.TryActivate(player) from a trigger/collision script.
public class Checkpoint : MonoBehaviour
{
    public enum CheckpointType { Bench, Portal }

    [Header("Setup")]
    public CheckpointType type = CheckpointType.Bench;
    public string checkpointID;         // unique ID e.g. "cave_bench_01" — set in Inspector

    [Header("Farm Return (Portal only)")]
    public bool canReturnToFarm = true;
    public string farmSceneName = "FarmScene";  // exact scene name in Build Settings

    [Header("Visuals")]
    public Animator checkpointAnimator;
    public string activateAnimTrigger = "activate";

    public static string  LastCheckpointID    { get; private set; }
    public static Vector3 LastCheckpointPos   { get; private set; }
    public static string  LastCheckpointScene { get; private set; }

    private const string KeyID    = "Checkpoint_ID";
    private const string KeyX     = "Checkpoint_X";
    private const string KeyY     = "Checkpoint_Y";
    private const string KeyScene = "Checkpoint_Scene";

    private void Start()
    {
        // Restore static data from PlayerPrefs on scene load
        if (PlayerPrefs.HasKey(KeyID))
        {
            LastCheckpointID    = PlayerPrefs.GetString(KeyID);
            LastCheckpointScene = PlayerPrefs.GetString(KeyScene);
            LastCheckpointPos   = new Vector3(
                PlayerPrefs.GetFloat(KeyX),
                PlayerPrefs.GetFloat(KeyY),
                0f
            );
        }
    }

    // Call this when the player presses interact near this checkpoint.
    public void Activate(Player player)
    {
        // Save position
        LastCheckpointID    = checkpointID;
        LastCheckpointPos   = transform.position;
        LastCheckpointScene = SceneManager.GetActiveScene().name;

        // Persist to PlayerPrefs
        PlayerPrefs.SetString(KeyID,    checkpointID);
        PlayerPrefs.SetString(KeyScene, LastCheckpointScene);
        PlayerPrefs.SetFloat(KeyX,      transform.position.x);
        PlayerPrefs.SetFloat(KeyY,      transform.position.y);
        PlayerPrefs.Save();

        Debug.Log($"[Checkpoint] Saved: {checkpointID} in {LastCheckpointScene}");

        // Play activate animation
        checkpointAnimator?.SetTrigger(activateAnimTrigger);

        // Portals: offer farm return
        if (type == CheckpointType.Portal && canReturnToFarm)
        {
            ReturnToFarm(player);
        }

        // TODO: play rest/save sound, show "Game Saved" UI briefly
    }

    /// Switches the player to Top-Down mode and loads the farm scene.
    /// The return checkpoint data is already saved, so returning to the
    /// dungeon later respawns at this portal.
    private void ReturnToFarm(Player player)
    {
        player.SwitchMode(Player.GameMode.TopDown);
        SceneManager.LoadScene(farmSceneName);
        // The farm scene should read LastCheckpointID / LastCheckpointScene
        // to know where to send the player back when they re-enter the dungeon.
    }

    // Teleports the player to the last saved checkpoint.
    // If no checkpoint saved, does nothing (fallback to your default spawn).
    public static void RespawnPlayer(Player player)
    {
        if (string.IsNullOrEmpty(LastCheckpointID))
        {
            Debug.Log("[Checkpoint] No checkpoint saved yet.");
            return;
        }

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == LastCheckpointScene)
        {
            // Same scene — just teleport
            player.transform.position = LastCheckpointPos;
            Debug.Log($"[Checkpoint] Respawned at {LastCheckpointID}");
        }
        else
        {
            // Different scene — load it, then PlayerSpawner will position the player
            // (see PlayerSpawner below)
            SceneManager.LoadScene(LastCheckpointScene);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (type == CheckpointType.Bench && other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null) Activate(player);
        }
    }
}
