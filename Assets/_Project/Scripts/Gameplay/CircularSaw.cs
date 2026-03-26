using UnityEngine;
using System.Collections.Generic;
using PixelDestruction.Core;

namespace PixelDestruction.Gameplay
{
    public class CircularSaw : MonoBehaviour, IWeapon
    {
        [Header("Damage Settings")]
        [SerializeField] private float damagePerSecond = 100f;
        [SerializeField] private float damageRadius = 1.0f;
        
        [Header("Animation Settings")]
        [SerializeField] private float rotationSpeed = 500f;

        private Collider2D[] hitBuffer = new Collider2D[200];
        
        private float baseRotationSpeed;
        private float baseDamagePerSecond;

        private void Awake()
        {
            baseRotationSpeed = rotationSpeed;
            baseDamagePerSecond = damagePerSecond;
        }

        private void Start()
        {
            if (Systems.UpgradeManager.Instance != null)
            {
                Systems.UpgradeManager.Instance.RegisterWeapon(this);
            }
        }

        private void OnDestroy()
        {
            if (Systems.UpgradeManager.Instance != null)
            {
                Systems.UpgradeManager.Instance.UnregisterWeapon(this);
            }
        }

        private void Update()
        {
            Transform targetToSpin = transform;
            targetToSpin.Rotate(0, 0, -rotationSpeed * Time.deltaTime);

            int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, damageRadius, hitBuffer);
            
            Dictionary<PixelObject, List<PixelNode>> objectsHit = new Dictionary<PixelObject, List<PixelNode>>();

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D col = hitBuffer[i];
                PixelNode node = col.GetComponent<PixelNode>();

                if (node != null && !node.IsDestroyed)
                {
                    float speedMultiplier = baseRotationSpeed > 0 ? (Mathf.Abs(rotationSpeed) / baseRotationSpeed) : 1f;
                    float effectiveDPS = baseDamagePerSecond * speedMultiplier;

                    node.Health -= effectiveDPS * Time.deltaTime;

                    if (node.Health <= 0)
                    {
                        PixelObject parentObj = node.GetComponentInParent<PixelObject>();
                        if (parentObj != null)
                        {
                            if (!objectsHit.ContainsKey(parentObj))
                                objectsHit[parentObj] = new List<PixelNode>();

                            objectsHit[parentObj].Add(node);
                            
                            if (PoolManager.Instance != null)
                            {
                                Color particleColor = node.OriginalColor;
                                particleColor.a = 1f; 

                                GameObject vfxObj = PoolManager.Instance.Spawn("SawHitVFX", node.transform.position, Quaternion.identity);
                                
                                ParticleSystem ps = vfxObj.GetComponent<ParticleSystem>();
                                if (ps != null)
                                {
                                    var main = ps.main;
                                    main.startColor = particleColor; 
                                }
                            }
                        }
                    }
                }
            }

            foreach (var kvp in objectsHit)
            {
                if (kvp.Key != null && kvp.Value.Count > 0)
                {
                    kvp.Key.DestroyPixels(kvp.Value);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
            Gizmos.DrawWireSphere(transform.position, damageRadius);
        }

        public void UpgradeDamage(float addAmount)
        {
            baseDamagePerSecond += addAmount;
        }

        public void UpgradeSpeed(float multiplier)
        {
            rotationSpeed *= multiplier;
        }

        public void UpgradeSize(float multiplier)
        {
            damageRadius *= multiplier;
            
            Transform targetToScale = transform;
            targetToScale.localScale *= multiplier;
        }

        public WeaponType GetWeaponType() => WeaponType.CircularSaw;

        public void ApplyUpgrade(UpgradeType type, float value)
        {
            switch (type)
            {
                case UpgradeType.FlatDamage:
                    UpgradeDamage(value);
                    break;
                case UpgradeType.MultiplySpeed:
                    UpgradeSpeed(value);
                    break;
                case UpgradeType.MultiplySize:
                    UpgradeSize(value);
                    break;
            }
        }
    }

}
