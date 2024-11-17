using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SectionPrefab
{
    public GameObject prefab; // Prefab for the section
    public int maxAppearances; // Maximum number of appearances for this prefab
}

public class MazeGenerator : MonoBehaviour
{
    public SectionPrefab startSection; // Prefab for the start of the maze
    public SectionPrefab endSection; // Prefab for the end of the maze
    public SectionPrefab deadEndSection; // Prefab for a dead end section
    public SectionPrefab[] hallwaySections; // Array to hold your hallway prefabs
    public SectionPrefab[] tJunctionSections; // Array to hold your T-junction prefabs
    public int numberOfTJunctions = 2; // Number of T-junctions to place
    public int mazeLength = 20; // Total number of sections to generate

    private Dictionary<GameObject, int> sectionCounts; // Dictionary to count appearances of each section
    private Transform lastConnectionPointA1;
    private Transform lastConnectionPointA2;

    public bool IsMazeGenerated { get; private set; } // Flag to indicate if the maze generation is complete

    void Start()
    {
        sectionCounts = new Dictionary<GameObject, int>();
        foreach (var section in hallwaySections)
        {
            sectionCounts[section.prefab] = 0;
        }
        foreach (var section in tJunctionSections)
        {
            sectionCounts[section.prefab] = 0;
        }
        GenerateMaze();
    }

    public void GenerateMaze() // Changed access modifier to public
    {
        IsMazeGenerated = false; // Set to false at the start of generation
        lastConnectionPointA1 = null;
        lastConnectionPointA2 = null;

        // Place the start section at the beginning
        PlaceStartSection(startSection.prefab);

        // Place T-junctions at random positions in the maze
        for (int i = 0; i < numberOfTJunctions; i++)
        {
            GameObject tJunction = GetRandomTJunction();
            if (tJunction != null)
            {
                PlaceTJunction(tJunction);
            }
        }

        // Generate the remaining middle sections
        for (int i = 0; i < mazeLength - 2 - numberOfTJunctions; i++) // -2 because we already placed start and will place end, -numberOfTJunctions for T-junctions
        {
            GameObject section = GetRandomSection();
            if (section != null)
            {
                PlaceSection(section);
            }
        }

        // Place the end section at the end
        PlaceEndSection(endSection.prefab);

        IsMazeGenerated = true; // Set to true once generation is complete
    }

    GameObject GetRandomSection()
    {
        List<GameObject> availableSections = new List<GameObject>();
        foreach (var section in hallwaySections)
        {
            if (sectionCounts[section.prefab] < section.maxAppearances)
            {
                availableSections.Add(section.prefab);
            }
        }
        if (availableSections.Count == 0)
        {
            return null; // No sections left to place
        }
        GameObject selectedSection = availableSections[Random.Range(0, availableSections.Count)];
        sectionCounts[selectedSection]++;
        return selectedSection;
    }

    GameObject GetRandomTJunction()
    {
        List<GameObject> availableTJunctions = new List<GameObject>();
        foreach (var section in tJunctionSections)
        {
            if (sectionCounts[section.prefab] < section.maxAppearances)
            {
                availableTJunctions.Add(section.prefab);
            }
        }
        if (availableTJunctions.Count == 0)
        {
            return null; // No T-junctions left to place
        }
        GameObject selectedTJunction = availableTJunctions[Random.Range(0, availableTJunctions.Count)];
        sectionCounts[selectedTJunction]++;
        return selectedTJunction;
    }

    void PlaceStartSection(GameObject sectionPrefab)
    {
        GameObject section = Instantiate(sectionPrefab);
        section.transform.position = Vector3.zero;
        lastConnectionPointA1 = section.transform.Find("ConnectionPointB1");
        lastConnectionPointA2 = section.transform.Find("ConnectionPointB2");

        if (lastConnectionPointA1 == null || lastConnectionPointA2 == null)
        {
            Debug.LogError("ConnectionPointB1 or ConnectionPointB2 not found on start section prefab: " + sectionPrefab.name);
        }
    }

    void PlaceSection(GameObject sectionPrefab)
    {
        GameObject section = Instantiate(sectionPrefab);

        if (lastConnectionPointA1 != null && lastConnectionPointA2 != null)
        {
            Transform newConnectionPointA1 = section.transform.Find("ConnectionPointA1");
            Transform newConnectionPointA2 = section.transform.Find("ConnectionPointA2");
            if (newConnectionPointA1 != null && newConnectionPointA2 != null)
            {
                // Calculate position offset
                Vector3 positionOffset = lastConnectionPointA1.position - newConnectionPointA1.position;
                section.transform.position += positionOffset;

                // Calculate rotation offset
                Vector3 directionA1 = newConnectionPointA1.position - newConnectionPointA2.position;
                Vector3 directionB1 = lastConnectionPointA1.position - lastConnectionPointA2.position;
                Quaternion rotationOffset = Quaternion.FromToRotation(directionA1, directionB1);
                section.transform.rotation = rotationOffset * section.transform.rotation;

                // Adjust position again after rotation
                positionOffset = lastConnectionPointA1.position - newConnectionPointA1.position;
                section.transform.position += positionOffset;

                // Update last connection points
                lastConnectionPointA1 = section.transform.Find("ConnectionPointB1");
                lastConnectionPointA2 = section.transform.Find("ConnectionPointB2");

                if (lastConnectionPointA1 == null || lastConnectionPointA2 == null)
                {
                    Debug.LogError("ConnectionPointB1 or ConnectionPointB2 not found on section prefab: " + sectionPrefab.name);
                }
            }
            else
            {
                Debug.LogError("ConnectionPointA1 or ConnectionPointA2 not found on section prefab: " + sectionPrefab.name);
            }
        }
    }

    void PlaceTJunction(GameObject tJunctionPrefab)
    {
        GameObject tJunction = Instantiate(tJunctionPrefab);

        if (lastConnectionPointA1 != null && lastConnectionPointA2 != null)
        {
            Transform newConnectionPointA1 = tJunction.transform.Find("ConnectionPointA1");
            Transform newConnectionPointA2 = tJunction.transform.Find("ConnectionPointA2");
            if (newConnectionPointA1 != null && newConnectionPointA2 != null)
            {
                // Calculate position offset
                Vector3 positionOffset = lastConnectionPointA1.position - newConnectionPointA1.position;
                tJunction.transform.position += positionOffset;

                // Calculate rotation offset
                Vector3 directionA1 = newConnectionPointA1.position - newConnectionPointA2.position;
                Vector3 directionB1 = lastConnectionPointA1.position - lastConnectionPointA2.position;
                Quaternion rotationOffset = Quaternion.FromToRotation(directionA1, directionB1);
                tJunction.transform.rotation = rotationOffset * tJunction.transform.rotation;

                // Adjust position again after rotation
                positionOffset = lastConnectionPointA1.position - newConnectionPointA1.position;
                tJunction.transform.position += positionOffset;

                // Update last connection points
                lastConnectionPointA1 = tJunction.transform.Find("ConnectionPointB1");
                lastConnectionPointA2 = tJunction.transform.Find("ConnectionPointB2");

                if (lastConnectionPointA1 == null || lastConnectionPointA2 == null)
                {
                    Debug.LogError("ConnectionPointB1 or ConnectionPointB2 not found on T-junction prefab: " + tJunctionPrefab.name);
                }

                // Handle branching paths from T-junction
                Transform connectionPointC1 = tJunction.transform.Find("ConnectionPointC1");
                Transform connectionPointC2 = tJunction.transform.Find("ConnectionPointC2");
                if (connectionPointC1 != null && connectionPointC2 != null)
                {
                    GenerateBranch(connectionPointC1, connectionPointC2);
                }
            }
            else
            {
                Debug.LogError("ConnectionPointA1 or ConnectionPointA2 not found on T-junction prefab: " + tJunctionPrefab.name);
            }
        }
    }

    void PlaceEndSection(GameObject sectionPrefab)
    {
        GameObject section = Instantiate(sectionPrefab);

        if (lastConnectionPointA1 != null && lastConnectionPointA2 != null)
        {
            Transform newConnectionPointA1 = section.transform.Find("ConnectionPointA1");
            Transform newConnectionPointA2 = section.transform.Find("ConnectionPointA2");
            if (newConnectionPointA1 != null && newConnectionPointA2 != null)
            {
                // Calculate position offset
                Vector3 positionOffset = lastConnectionPointA1.position - newConnectionPointA1.position;
                section.transform.position += positionOffset;

                // Calculate rotation offset
                Vector3 directionA1 = newConnectionPointA1.position - newConnectionPointA2.position;
                Vector3 directionB1 = lastConnectionPointA1.position - lastConnectionPointA2.position;
                Quaternion rotationOffset = Quaternion.FromToRotation(directionA1, directionB1);
                section.transform.rotation = rotationOffset * section.transform.rotation;

                // Adjust position again after rotation
                positionOffset = lastConnectionPointA1.position - newConnectionPointA1.position;
                section.transform.position += positionOffset;
            }
            else
            {
                Debug.LogError("ConnectionPointA1 or ConnectionPointA2 not found on end section prefab: " + sectionPrefab.name);
            }
        }
    }

    void GenerateBranch(Transform connectionPointC1, Transform connectionPointC2)
    {
        Transform currentConnectionPointC1 = connectionPointC1;
        Transform currentConnectionPointC2 = connectionPointC2;

        for (int i = 0; i < 1; i++) // Only place one dead end section per branch
        {
            PlaceDeadEnd(currentConnectionPointC1, currentConnectionPointC2);
        }
    }

    void PlaceDeadEnd(Transform currentConnectionPointC1, Transform currentConnectionPointC2)
    {
        GameObject deadEnd = Instantiate(deadEndSection.prefab);
        Transform newDeadEndConnectionPointA1 = deadEnd.transform.Find("ConnectionPointA1");
        Transform newDeadEndConnectionPointA2 = deadEnd.transform.Find("ConnectionPointA2");

        if (newDeadEndConnectionPointA1 != null && newDeadEndConnectionPointA2 != null)
        {
            // Calculate position offset
            Vector3 positionOffset = currentConnectionPointC1.position - newDeadEndConnectionPointA1.position;
            deadEnd.transform.position += positionOffset;

            // Calculate rotation offset
            Vector3 directionA1 = newDeadEndConnectionPointA1.position - newDeadEndConnectionPointA2.position;
            Vector3 directionC1 = currentConnectionPointC1.position - currentConnectionPointC2.position;
            Quaternion rotationOffset = Quaternion.FromToRotation(directionA1, directionC1);
            deadEnd.transform.rotation = rotationOffset * deadEnd.transform.rotation;

            // Adjust position again after rotation
            positionOffset = currentConnectionPointC1.position - newDeadEndConnectionPointA1.position;
            deadEnd.transform.position += positionOffset;
        }
        else
        {
            Debug.LogError("ConnectionPointA1 or ConnectionPointA2 not found on dead end section prefab: " + deadEnd.name);
        }
    }
}
