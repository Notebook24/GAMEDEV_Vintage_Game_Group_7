using UnityEngine;
using Unity.AI.Navigation;

[RequireComponent(typeof(NavMeshSurface))]
public class LevelGenerator : MonoBehaviour
{
    // prefabs
    public GameObject wallPrefab;
    public GameObject dotPrefab;
    public GameObject redGhostPrefab;
    public GameObject greenGhostPrefab;
    public GameObject pinkGhostPrefab;
    public GameObject orangeGhostPrefab;
    public GameObject playerPrefab;

    public float tileSize = 2.5f;

    // 0 empty 1 wall 2 dot 3 player
    // ghosts - 4 red 5 green 6 pink 7 orange
    // inspiration from sokobot map generation
    private int[,] mazeGrid = new int[,] // 28 x 28
    {
        {1, 1,1,1,1,1,  1,  1,1,1,1,1,1, 1 ,1,1,1,1,1,1,    1,    1,1,1,1,1,    1},
        {1, 2,2,2,2,2,  2,  2,2,2,2,2,2, 1 ,2,2,2,2,2,2,    2,    2,2,2,2,2,    1},
        {1, 2,1,1,1,1,  2,  1,1,1,1,1,2, 1 ,2,1,1,1,1,1,    2,    1,1,1,1,2,    1},
        {1, 2,1,1,1,1,  2,  1,1,1,1,1,2, 1 ,2,1,1,1,1,1,    2,    1,1,1,1,2,    1},
        {1, 2,1,1,1,1,  2,  1,1,1,1,1,2, 1 ,2,1,1,1,1,1,    2,    1,1,1,1,2,    1},

        {1, 2,2,2,2,2,  2,  2,2,2,2,2,2, 2 ,2,2,2,2,2,2,    2,    2,2,2,2,2,    1},
        {1, 2,1,1,1,1,  2,  1,2,1,1,1,1, 1 ,1,1,1,1,2,1,    2,    1,1,1,1,2,    1},
        {1, 2,2,2,2,2,  2,  1,2,2,2,2,1, 1 ,1,2,2,2,2,1,    2,    2,2,2,2,2,    1},
        {1, 1,1,1,1,1,  2,  1,1,1,1,0,0, 1 ,0,0,1,1,1,1,    2,    1,1,1,1,1,    1},
        {1, 0,0,0,0,1,  2,  1,0,0,0,0,0, 0 ,0,0,0,0,0,1,    2,    1,0,0,0,0,    1},

        {1, 0,0,0,0,1,  2,  1,0,0,0,0,0, 0 ,0,0,0,0,0,1,    2,    1,0,0,0,0,    1},
        {1, 0,0,0,0,1,  2,  1,0,0,1,1,0, 4 ,0,1,1,0,0,1,    2,    1,0,0,0,0,    1},
        {1, 0,0,0,0,1,  2,  0,0,0,1,0,5, 6 ,7,0,1,0,0,0,    2,    1,0,0,0,0,    1},//middle of the map removed gaps
        {1, 0,0,0,0,1,  2,  1,0,0,1,1,1, 1 ,1,1,1,0,0,1,    2,    1,0,0,0,0,    1},
        {1, 0,0,0,0,1,  2,  1,0,0,0,0,0, 0 ,0,0,0,0,0,1,    2,    1,0,0,0,0,    1},

        {1, 0,0,0,0,1,  2,  1,0,0,0,0,0, 0 ,0,0,0,0,0,1,    2,    1,0,0,0,0,    1},
        {1, 0,0,0,0,1,  2,  1,0,0,0,0,1, 1 ,1,0,0,0,0,1,    2,    1,0,0,0,0,    1},
        {1, 1,1,1,1,1,  2,  1,0,0,1,1,1, 1 ,1,1,1,0,0,1,    2,    1,1,1,1,1,    1},
        {1, 2,2,2,2,2,  2,  2,2,2,2,2,2, 1 ,2,2,2,2,2,2,    2,    2,2,2,2,2,    1},
        {1, 2,1,1,1,1,  2,  1,1,1,1,1,2, 1 ,2,1,1,1,1,1,    2,    1,1,1,1,2,    1},

        {1, 2,2,2,1,1,  2,  2,2,2,2,2,2, 3 ,2,2,2,2,2,2,    2,    1,1,2,2,2,    1},
        {1, 1,1,2,1,1,  2,  1,1,2,1,1,1, 1 ,1,1,1,2,1,1,    2,    1,1,2,1,1,    1},
        {1, 2,2,2,2,2,  2,  1,1,2,2,2,2, 1 ,2,2,2,2,1,1,    2,    2,2,2,2,2,    1},
        {1, 2,1,1,1,1,  1,  1,1,1,1,1,2, 1 ,2,1,1,1,1,1,    1,    1,1,1,1,2,    1},
        {1, 2,1,1,1,1,  1,  1,1,1,1,1,2, 1 ,2,1,1,1,1,1,    1,    1,1,1,1,2,    1},

        {1, 2,2,2,2,2,  2,  2,2,2,2,2,2, 1 ,2,2,2,2,2,2,    2,    2,2,2,2,2,    1},
        {1, 1,1,1,1,1,  1,  1,1,1,1,1,1, 1 ,1,1,1,1,1,1,    1,    1,1,1,1,1,    1}
    };

    void Awake()
    {
        NavMeshSurface surface = GetComponent<NavMeshSurface>();
        if (surface == null)
        {
            surface = gameObject.AddComponent<NavMeshSurface>();
        }

        GenerateMaze();
        surface.BuildNavMesh();
    }

    void GenerateMaze()
    {
        for (int x = 0; x < mazeGrid.GetLength(0); x++)
        {
            for (int z = 0; z < mazeGrid.GetLength(1); z++)
            {
                int tileType = mazeGrid[x, z];

                Vector3 basePos = new Vector3(x * tileSize, 0, z * tileSize);
                GameObject spawnedObj;

                switch (tileType)
                {
                    case 1: // Wall
                        spawnedObj = Instantiate(wallPrefab, basePos + new Vector3(0, 2.0f, 0), Quaternion.identity, transform);
                        // spawnedObj.transform.localScale = new Vector3(2.5f, 4.0f, 2.5f);
                        break;

                    case 2: // Dot
                        spawnedObj = Instantiate(dotPrefab, basePos + new Vector3(0, 0.5f, 0), Quaternion.identity, transform);
                        // spawnedObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                        break;

                    case 3: // Player
                        spawnedObj = Instantiate(playerPrefab, basePos + new Vector3(0, 0.75f, 0), Quaternion.identity);
                        // Unity capsules are 2m default. Scaling Y by 0.75 makes it 1.5m tall
                        // spawnedObj.transform.localScale = new Vector3(1.0f, 0.75f, 1.0f);
                        break;

                    case 4: // Red
                        spawnedObj = Instantiate(redGhostPrefab, basePos, Quaternion.identity);
                        // Unity cylinders are 2m default. Scaling Y by 1.2 makes it 2.4m tall
                        // spawnedObj.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                        break;

                    case 5: // green
                        spawnedObj = Instantiate(greenGhostPrefab, basePos + new Vector3(0, 1.25f, 0), Quaternion.identity);
                        // Unity cylinders are 2m default. Scaling Y by 1.2 makes it 2.4m tall
                        // spawnedObj.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                        break;

                    case 6: // pink
                        spawnedObj = Instantiate(pinkGhostPrefab, basePos + new Vector3(0, 1.25f, 0), Quaternion.identity);
                        // Unity cylinders are 2m default. Scaling Y by 1.2 makes it 2.4m tall
                        // spawnedObj.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                        break;

                    case 7: // orange
                        spawnedObj = Instantiate(orangeGhostPrefab, basePos + new Vector3(0, 1.25f, 0), Quaternion.identity);
                        // Unity cylinders are 2m default. Scaling Y by 1.2 makes it 2.4m tall
                        // spawnedObj.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                        break;

                }
            }
        }

    }
}