using UnityEngine;

namespace PixelDestruction.Gameplay
{
    [RequireComponent(typeof(BoxCollider2D))] 
    public class PixelNode : MonoBehaviour
    {
        public int GridX { get; set; }
        public int GridY { get; set; }
        public bool IsDestroyed { get; set; }
        
        [SerializeField] private float health = 100f;
        public float Health 
        { 
            get => health; 
            set => health = value; 
        }
    }
}
