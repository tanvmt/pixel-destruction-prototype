using UnityEngine;
using System.Collections.Generic;
using PixelDestruction.Core;

namespace PixelDestruction.Gameplay
{
    public class TapToDestroy : MonoBehaviour
    {
        [Header("Tap Config")]
        [SerializeField] private float damageRadius = 0.5f;
        [SerializeField] private float maxDamage = 100f;
        [SerializeField] private float minDamage = 10f;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
                
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                Vector2 clickPos = new Vector2(worldPos.x, worldPos.y);

                if (PoolManager.Instance != null)
                {
                    PoolManager.Instance.Spawn("TapFlashVFX", worldPos, Quaternion.identity);
                }
                
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(clickPos, damageRadius);
                
                Dictionary<PixelObject, List<PixelNode>> objectsHit = new Dictionary<PixelObject, List<PixelNode>>();
                
                foreach (var col in hitColliders)
                {
                    PixelNode node = col.GetComponent<PixelNode>();
                    if (node != null && !node.IsDestroyed)
                    {
                        float distance = Vector2.Distance(clickPos, node.transform.position);
                        float damageFalloff = Mathf.Clamp01(1f - (distance / damageRadius));
                        float appliedDamage = Mathf.Lerp(minDamage, maxDamage, damageFalloff);
                        
                        node.Health -= appliedDamage;

                        if (node.Health <= 0)
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
                }
                
                foreach (var kvp in objectsHit)
                {
                    kvp.Key.DestroyPixels(kvp.Value);
                }
            }
        }
    }
}
