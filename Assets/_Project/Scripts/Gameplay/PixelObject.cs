using UnityEngine;
using System.Collections.Generic;

namespace PixelDestruction.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PixelObject : MonoBehaviour
    {
        public PixelNode[,] grid;
        public int width;
        public int height;

        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        public void Initialize(PixelNode[,] initialGrid, int w, int h)
        {
            grid = initialGrid;
            width = w;
            height = h;

            UpdateCenterOfMass();
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
    }
}
