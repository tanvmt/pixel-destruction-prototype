using UnityEngine;

namespace PixelDestruction.Gameplay
{
    public class PixelSpawner : MonoBehaviour
    {
        public static PixelSpawner Instance { get; private set; }

        [Header("Prefabs")]
        [SerializeField] private GameObject pixelObjectPrefab;
        [SerializeField] private GameObject pixelNodePrefab;

        [Header("Fallback Generation Settings")]
        [SerializeField] private int width = 5;
        [SerializeField] private int height = 5;
        [SerializeField] private float spacing = 0.5f;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void SpawnObjectFromTexture(Texture2D sourceTexture, Vector3 spawnPosition)
        {
            GameObject obj = Instantiate(pixelObjectPrefab, spawnPosition, Quaternion.identity);
            PixelObject pixelObj = obj.GetComponent<PixelObject>();

            int finalWidth = sourceTexture != null ? sourceTexture.width : width;
            int finalHeight = sourceTexture != null ? sourceTexture.height : height;

            PixelNode[,] grid = new PixelNode[finalWidth, finalHeight];

            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();

            for (int x = 0; x < finalWidth; x++)
            {
                for (int y = 0; y < finalHeight; y++)
                {
                    Color pixelColor = Color.white;

                    if (sourceTexture != null)
                    {
                        pixelColor = sourceTexture.GetPixel(x, y);
                        Debug.Log("Pixel Color: " + pixelColor);
                        if (pixelColor.a < 0.1f) continue;
                    }

                    Vector3 localPos = new Vector3(x * spacing, y * spacing, 0);
                    GameObject nodeObj = Instantiate(pixelNodePrefab, obj.transform);
                    nodeObj.transform.localPosition = localPos;
                    
                    MeshRenderer mr = nodeObj.GetComponent<MeshRenderer>();
                    if (mr != null) 
                    {
                        mr.GetPropertyBlock(propBlock);
                        propBlock.SetColor("_Color", pixelColor); 
                        mr.SetPropertyBlock(propBlock);
                    }
                    
                    PixelNode node = nodeObj.GetComponent<PixelNode>();
                    node.GridX = x;
                    node.GridY = y;
                    
                    grid[x, y] = node;
                }
            }

            Vector3 centerOffset = new Vector3((finalWidth - 1) * spacing / 2f, (finalHeight - 1) * spacing / 2f, 0);
            for (int x = 0; x < finalWidth; x++)
            {
                for (int y = 0; y < finalHeight; y++)
                {
                    if (grid[x, y] != null)
                    {
                        grid[x, y].transform.localPosition -= centerOffset;
                    }
                }
            }

            pixelObj.Initialize(grid, finalWidth, finalHeight);
        }
    }
}
