using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    // prefabs
    public GameObject wallPrefab;
    public GameObject dotPrefab;
    public GameObject ghostPrefab;
    public GameObject playerPrefab;

    public float tileSize = 2.5f;

    // 0 empty 1 wall 2 dot 3 ghost 4 player
    // inspiration from sokobot map generation
    private int[,] mazeGrid = new int[,] // 28 x 28
    {
        {1, 1,1,1,1,1,  1,  1,1,1,1,1,1, 1 ,1,1,1,1,1,1,    1,    1,1,1,1,1,    1},
        {1, 2,2,2,2,2,  2,  2,2,2,2,2,2, 1 ,2,2,2,2,2,2,    2,    2,2,2,2,2,    1},
        {1, 2,1,1,1,1,  2,  1,1,1,1,1,2, 1 ,2,1,1,1,1,1,    2,    1,1,1,1,2,    1},
        {1, 2,1,1,1,1,  2,  1,1,1,1,1,2, 1 ,2,1,1,1,1,1,    2,    1,1,1,1,2,    1},
        {1, 2,2,2,2,2,  2,  2,2,2,2,2,2, 2 ,2,2,2,2,2,2,    2,    2,2,2,2,2,    1}, 

        {1, 2,1,1,1,1,  2,  1,2,1,1,1,1, 1 ,1,1,1,1,2,1,    2,    1,1,1,1,2,    1},
        {1, 2,2,2,2,2,  2,  1,2,2,2,2,1, 1 ,1,2,2,2,2,1,    2,    2,2,2,2,2,    1},
        {1, 1,1,1,1,1,  2,  1,1,1,1,0,0, 1 ,0,0,1,1,1,1,    2,    1,1,1,1,1,    1},
        {1, 0,0,0,0,1,  2,  1,0,0,0,0,0, 0 ,0,0,0,0,0,1,    2,    1,0,0,0,0,    1},
        {1, 0,0,0,0,1,  2,  1,0,0,0,0,0, 0 ,0,0,0,0,0,1,    2,    1,0,0,0,0,    1},

        {1, 0,0,0,0,1,  2,  1,0,0,1,1,0, 3 ,0,1,1,0,0,1,    2,    1,0,0,0,0,    1}, 
        {1, 0,0,0,0,1,  2,  0,0,0,1,0,3, 3 ,3,0,1,0,0,0,    2,    1,0,0,0,0,    1},//middle of the map removed gaps
        {1, 0,0,0,0,1,  2,  1,0,0,1,1,1, 1 ,1,1,1,0,0,1,    2,    1,0,0,0,0,    1},
        {1, 0,0,0,0,1,  2,  1,0,0,0,0,0, 0 ,0,0,0,0,0,1,    2,    1,0,0,0,0,    1},
        {1, 0,0,0,0,1,  2,  1,0,0,0,0,0, 0 ,0,0,0,0,0,1,    2,    1,0,0,0,0,    1},
        
        {1, 1,1,1,1,1,  2,  1,0,0,1,1,1, 1 ,1,1,1,0,0,1,    2,    1,1,1,1,1,    1},
        {1, 2,2,2,2,2,  2,  2,2,2,2,2,2, 1 ,2,2,2,2,2,2,    2,    2,2,2,2,2,    1},
        {1, 2,1,1,1,1,  2,  1,1,1,1,1,2, 1 ,2,1,1,1,1,1,    2,    1,1,1,1,2,    1},
        {1, 2,2,2,1,1,  2,  2,2,2,2,2,2, 4 ,2,2,2,2,2,2,    2,    1,1,2,2,2,    1},
        {1, 1,1,2,1,1,  2,  1,1,2,1,1,1, 1 ,1,1,1,2,1,1,    2,    1,1,2,1,1,    1},
        
        {1, 2,2,2,2,2,  2,  1,1,2,2,2,2, 1 ,2,2,2,2,1,1,    2,    2,2,2,2,2,    1},
        {1, 2,1,1,1,1,  1,  1,1,1,1,1,2, 1 ,2,1,1,1,1,1,    1,    1,1,1,1,2,    1},
        {1, 2,2,2,2,2,  2,  2,2,2,2,2,2, 1 ,2,2,2,2,2,2,    2,    2,2,2,2,2,    1},
        {1, 1,1,1,1,1,  1,  1,1,1,1,1,1, 1 ,1,1,1,1,1,1,    1,    1,1,1,1,1,    1}
    };

    void Start()
    {
        GenerateMaze();
    }

    void GenerateMaze()
    {
        for (int x = 0; x<mazeGrid.GetLength(0); x++)
        {
            for (int z = 0; z<mazeGrid.GetLength(1); z++)
            {
                int tileType = mazeGrid[x, z];

                Vector3 basePos = new Vector3(x * tileSize, 0, z*tileSize);
                GameObject spawnedObj;

                switch (tileType)
                {
                    case 1: // Wall
                        // Y = 2.0f so a 4-unit tall cube rests on the floor
                        spawnedObj = Instantiate(wallPrefab, basePos + new Vector3(0, 2.0f, 0), Quaternion.identity, transform);
                        spawnedObj.transform.localScale = new Vector3(2.5f, 4.0f, 2.5f);
                        break;
                    
                    case 2: // Dot
                        // Y = 0.5f so it floats near the ground
                        spawnedObj = Instantiate(dotPrefab, basePos + new Vector3(0, 0.5f, 0), Quaternion.identity, transform);
                        spawnedObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                        break;
                    
                    case 3: // Ghost
                        // Y = 1.2f so a 2.4-unit tall cylinder rests on the floor
                        spawnedObj = Instantiate(ghostPrefab, basePos + new Vector3(0, 1.2f, 0), Quaternion.identity);
                        // Unity cylinders are 2m default. Scaling Y by 1.2 makes it 2.4m tall
                        spawnedObj.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                        break;
                    
                    case 4: // Player
                        // Y = 0.75f so a 1.5-unit tall capsule rests on the floor
                        spawnedObj = Instantiate(playerPrefab, basePos + new Vector3(0, 0.75f, 0), Quaternion.identity);
                        // Unity capsules are 2m default. Scaling Y by 0.75 makes it 1.5m tall
                        spawnedObj.transform.localScale = new Vector3(1.0f, 0.75f, 1.0f);
                        break;
                }
            }
        }
    }
}