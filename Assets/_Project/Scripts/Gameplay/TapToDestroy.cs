using UnityEngine;
using System.Collections.Generic;

namespace PixelDestruction.Gameplay
{
    public class TapToDestroy : MonoBehaviour
    {
        [SerializeField] private float damageRadius = 0.5f;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
                
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                Vector2 clickPos = new Vector2(worldPos.x, worldPos.y);
                
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(clickPos, damageRadius);
                
                Dictionary<PixelObject, List<PixelNode>> objectsHit = new Dictionary<PixelObject, List<PixelNode>>();
                
                foreach (var col in hitColliders)
                {
                    PixelNode node = col.GetComponent<PixelNode>();
                    if (node != null && !node.IsDestroyed)
                    {
                        PixelObject parentObj = node.GetComponentInParent<PixelObject>();
                        if (parentObj != null)
                        {
                            if (!objectsHit.ContainsKey(parentObj))
                                objectsHit[parentObj] = new List<PixelNode>();
                                
                            objectsHit[parentObj].Add(node);
                        }
                    }
                }
                
                foreach (var kvp in objectsHit)
                {
                    kvp.Key.DestroyPixels(kvp.Value);
                }
            }
        }
    }
}
