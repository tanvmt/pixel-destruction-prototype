using UnityEngine;

namespace PixelDestruction.Gameplay
{
    [RequireComponent(typeof(BoxCollider2D))] 
    public class PixelNode : MonoBehaviour
    {
        public int GridX { get; set; }
        public int GridY { get; set; }
        public bool IsDestroyed { get; set; }
        
        public float Health { get; set; }

        [SerializeField] private TrailRenderer trail;

        public Color OriginalColor { get; set; } = Color.white;
        
        private void Awake()
        {
            if (trail != null) trail.emitting = false; 
        }
        
        private void OnEnable()
        {
            IsDestroyed = false;
            
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null) Destroy(rb);
            
            BoxCollider2D col = GetComponent<BoxCollider2D>();
            if (col != null) col.size = Vector2.one;

            if (trail != null)
            {
                trail.emitting = false; 
                trail.Clear();
            }
        }

        public void ActivateTrail()
        {
            if (trail != null && !trail.emitting)
            {
                trail.Clear(); 
                trail.emitting = true;
            }
        }
        
        private void OnDisable()
        {
            if (trail != null)
            {
                trail.emitting = false; 
                trail.Clear();
            }
        }
    }
}
