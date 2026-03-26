using UnityEngine;
using System.Collections.Generic;
using PixelDestruction.Core;

namespace PixelDestruction.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PixelObject : MonoBehaviour
    {
        private PixelNode[,] grid;
        private int width;
        private int height;

        private Rigidbody2D rb;
        public int spawnId = -1;

        [SerializeField] private int activePixelCount = 0;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 1f;
        }

        public void Initialize(PixelNode[,] initialGrid, int w, int h)
        {
            grid = initialGrid;
            width = w;
            height = h;

            activePixelCount = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y] != null && !grid[x, y].IsDestroyed)
                    {
                        activePixelCount++;
                    }
                }
            }

            UpdateCenterOfMass();
        }

        public void RemoveNodeFast(PixelNode node)
        {
            if (node != null && !node.IsDestroyed)
            {
                node.IsDestroyed = true;
                
                if (grid != null && node.GridX >= 0 && node.GridX < width && node.GridY >= 0 && node.GridY < height)
                {
                    grid[node.GridX, node.GridY] = null;
                }
                
                activePixelCount--;

                if (activePixelCount <= 0)
                {
                    PoolManager.Instance.Despawn(gameObject);
                }
            }
        }

        public void DestroyPixels(List<PixelNode> nodesToDestroy)
        {
            bool anyDestroyed = false;
            foreach (var node in nodesToDestroy)
            {
                if (node != null && !node.IsDestroyed)
                {
                    node.IsDestroyed = true;
                    grid[node.GridX, node.GridY] = null;
                    anyDestroyed = true;
                    activePixelCount--;

                    node.transform.SetParent(PoolManager.Instance.transform);

                    Rigidbody2D nodeRb = node.gameObject.GetComponent<Rigidbody2D>();
                    if (nodeRb == null)
                    {
                        nodeRb = node.gameObject.AddComponent<Rigidbody2D>();
                    }

                    BoxCollider2D nodeCol = node.gameObject.GetComponent<BoxCollider2D>();
                    if (nodeCol != null)
                    {
                        nodeCol.size *= 0.7f;
                    }

                    nodeRb.mass = 0.05f;
                    nodeRb.velocity = rb.velocity;
                }
            }
            
            if (activePixelCount <= 0)
            {
                PoolManager.Instance.Despawn(gameObject);
                return;
            }

            if (anyDestroyed)
            {
                CheckConnectedComponents();
            }
        }

        private void CheckConnectedComponents()
        {
            List<List<PixelNode>> components = new List<List<PixelNode>>();
            bool[,] visited = new bool[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    PixelNode node = grid[x, y];
                    if (node != null && !node.IsDestroyed && !visited[x, y])
                    {
                        List<PixelNode> component = new List<PixelNode>();
                        BFS(x, y, visited, component);
                        components.Add(component);
                    }
                }
            }

            if (components.Count == 0)
            {
                PoolManager.Instance.Despawn(gameObject);
            }
            else if (components.Count > 1)
            {
                SplitObject(components); 
            }
            else
            {
                UpdateCenterOfMass();
            }
        }

        private void BFS(int startX, int startY, bool[,] visited, List<PixelNode> component)
        {
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(new Vector2Int(startX, startY));
            visited[startX, startY] = true;

            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            while (queue.Count > 0)
            {
                Vector2Int curr = queue.Dequeue();
                component.Add(grid[curr.x, curr.y]);

                for (int i = 0; i < 4; i++)
                {
                    int nx = curr.x + dx[i];
                    int ny = curr.y + dy[i];

                    if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                    {
                        PixelNode neighbor = grid[nx, ny];
                        if (neighbor != null && !neighbor.IsDestroyed && !visited[nx, ny])
                        {
                            visited[nx, ny] = true;
                            queue.Enqueue(new Vector2Int(nx, ny));
                        }
                    }
                }
            }
        }

        private void SplitObject(List<List<PixelNode>> components)
        {
            for (int i = 1; i < components.Count; i++)
            {
                List<PixelNode> newComponent = components[i];
                CreateNewPixelObject(newComponent);
                
                foreach (var node in newComponent)
                {
                    grid[node.GridX, node.GridY] = null;
                    activePixelCount--;
                }
            }

            UpdateCenterOfMass();
        }

        private void CreateNewPixelObject(List<PixelNode> componentNodes)
        {
            GameObject newObj = PoolManager.Instance.Spawn("PixelObject", transform.position, transform.rotation);
            
            Rigidbody2D newRb = newObj.GetComponent<Rigidbody2D>();
            newRb.velocity = rb.velocity;
            newRb.angularVelocity = rb.angularVelocity;
            
            PixelObject newPixelObj = newObj.GetComponent<PixelObject>();

            newPixelObj.spawnId = this.spawnId;
            if (this.spawnId != -1 && LevelManager.Instance != null)
            {
                LevelManager.Instance.RegisterFragment(this.spawnId);
            }

            PixelNode[,] newGrid = new PixelNode[width, height];
            
            foreach (var node in componentNodes)
            {
                newGrid[node.GridX, node.GridY] = node;
                node.transform.SetParent(newObj.transform, true); 
            }

            newPixelObj.Initialize(newGrid, width, height);
        }

        public int GetRemainingPixelCount()
        {
            return activePixelCount;
        }

        public void UpdateCenterOfMass()
        {
            if (grid == null) return;

            int count = 0;
            Vector2 sumLocalPos = Vector2.zero;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y] != null && !grid[x, y].IsDestroyed)
                    {
                        sumLocalPos += (Vector2)grid[x, y].transform.localPosition;
                        count++;
                    }
                }
            }

            if (count > 0)
            {
                rb.centerOfMass = sumLocalPos / count;
            }
        }

        private void OnDisable()
        {
            if (spawnId != -1 && LevelManager.Instance != null)
            {
                LevelManager.Instance.UnregisterFragment(spawnId);
            }

            grid = null;
            spawnId = -1;
            activePixelCount = 0;
            
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
    }
}
