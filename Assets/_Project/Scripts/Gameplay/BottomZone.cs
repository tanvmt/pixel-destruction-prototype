using UnityEngine;
using PixelDestruction.Systems;
using PixelDestruction.Core;

namespace PixelDestruction.Gameplay
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class BottomZone : MonoBehaviour
    {
        [Header("XP Settings")]
        [Tooltip("XP gained per pixel falling into the bottom zone")]
        [SerializeField] private int xpPerPixel = 1;

        private void Awake()
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            PixelNode node = other.GetComponent<PixelNode>();
            
            if (node != null) 
            {
                PixelObject pixelObj = node.transform.parent != null ? node.GetComponentInParent<PixelObject>() : null;

                if (XpManager.Instance != null)
                {
                    XpManager.Instance.AddXp(xpPerPixel);
                }

                if (pixelObj != null)
                {
                    pixelObj.RemoveNodeFast(node); 
                }
                else
                {
                    node.IsDestroyed = true;
                }

                PoolManager.Instance.Despawn(node.gameObject);
            }
        }
    }
}
