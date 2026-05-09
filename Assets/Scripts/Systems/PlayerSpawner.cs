using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    public Transform defaultSpawnPoint;
    public bool isFarmScene = false;

    private void Start()
    {
        Player player = FindFirstObjectByType<Player>();
        if (player == null) return;

        string thisScene = SceneManager.GetActiveScene().name;

        if (GameState.I != null &&
            GameState.I.lastCheckpointScene == thisScene &&
            !string.IsNullOrEmpty(GameState.I.lastCheckpointScene))
        {
            player.transform.position = new Vector3(
                GameState.I.lastCheckpointX,
                GameState.I.lastCheckpointY,
                0f
            );
        }
        else if (defaultSpawnPoint != null)
        {
            player.transform.position = defaultSpawnPoint.position;
        }

        if (isFarmScene)
            player.SwitchMode(Player.GameMode.TopDown);
        else
            player.SwitchMode(Player.GameMode.Platformer);
    }
}