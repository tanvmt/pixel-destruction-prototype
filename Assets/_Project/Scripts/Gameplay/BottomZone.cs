using UnityEngine;
using PixelDestruction.Systems;

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

        private void OnTriggerEnter2D(Collider2D other)
        {
            PixelNode node = other.GetComponent<PixelNode>();
            if (node != null && node.transform.parent == null)
            {
                if (XpManager.Instance != null)
                {
                    XpManager.Instance.AddXp(xpPerPixel);
                }
                Destroy(node.gameObject);
                return;
            }

            PixelObject pixelObj = other.GetComponentInParent<PixelObject>();
            if (pixelObj != null)
            {
                int remainingPixels = pixelObj.GetRemainingPixelCount();
                
                if (XpManager.Instance != null)
                {
                    XpManager.Instance.AddXp(remainingPixels * xpPerPixel);
                }
                
                Destroy(pixelObj.gameObject);

            }
        }
    }
}
