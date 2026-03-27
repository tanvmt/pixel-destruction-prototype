using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace PixelDestruction.UI
{
    public class UIButtonAnim : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Vector3 originalScale;

        private void Awake()
        {
            originalScale = transform.localScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            transform.DOKill(); 
            
            transform.DOScale(originalScale * 0.9f, 0.1f).SetUpdate(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            transform.DOKill();
            
            transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);
        }
    }
}