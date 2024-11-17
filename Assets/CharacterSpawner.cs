using UnityEngine;
using StarterAssets;

public class CharacterSpawner : MonoBehaviour
{
    public GameObject playerPrefab; // The First Person Controller prefab
    public MazeGenerator mazeGenerator; // Reference to the MazeGenerator script

    void Start()
    {
        if (mazeGenerator == null)
        {
            Debug.LogError("MazeGenerator reference not set in CharacterSpawner.");
            return;
        }

        // Ensure the maze has been generated
        mazeGenerator.GenerateMaze();

        // Find the start section and its spawn point
        GameObject startSection = mazeGenerator.startSection.prefab;
        if (startSection == null)
        {
            Debug.LogError("Start section prefab is not set in the MazeGenerator.");
            return;
        }

        // Ensure the start section is instantiated in the scene
        GameObject instantiatedStartSection = GameObject.Find(startSection.name + "(Clone)");
        if (instantiatedStartSection == null)
        {
            Debug.LogError("Start section instance not found in the scene.");
            return;
        }

        Transform spawnPoint = instantiatedStartSection.transform.Find("SpawnPoint");
        if (spawnPoint == null)
        {
            Debug.LogError("SpawnPoint not found in the start section prefab.");
            return;
        }

        // Instantiate the player at the spawn point
        GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        if (player == null)
        {
            Debug.LogError("Failed to instantiate player prefab.");
        }
        else
        {
            Debug.Log("Player instantiated successfully at " + spawnPoint.position);
        }
    }
}
