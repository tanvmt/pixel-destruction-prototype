using UnityEngine;

namespace PixelDestruction.Gameplay
{
    public class PixelSpawner : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject pixelObjectPrefab;
        [SerializeField] private GameObject pixelNodePrefab;

        [Header("Generation Settings")]
        [SerializeField] private int width = 5;
        [SerializeField] private int height = 5;
        [SerializeField] private float spacing = 0.5f;

        private void Start()
        {
            SpawnObject();
        }

        public void SpawnObject()
        {
            GameObject obj = Instantiate(pixelObjectPrefab, transform.position, Quaternion.identity);
            PixelObject pixelObj = obj.GetComponent<PixelObject>();

            PixelNode[,] grid = new PixelNode[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 localPos = new Vector3(x * spacing, y * spacing, 0);
                    GameObject nodeObj = Instantiate(pixelNodePrefab, obj.transform);
                    nodeObj.transform.localPosition = localPos;
                    
                    PixelNode node = nodeObj.GetComponent<PixelNode>();
                    node.GridX = x;
                    node.GridY = y;
                    
                    grid[x, y] = node;
                }
            }

            Vector3 centerOffset = new Vector3((width - 1) * spacing / 2f, (height - 1) * spacing / 2f, 0);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid[x, y].transform.localPosition -= centerOffset;
                }
            }

            pixelObj.Initialize(grid, width, height);
        }
    }
}
