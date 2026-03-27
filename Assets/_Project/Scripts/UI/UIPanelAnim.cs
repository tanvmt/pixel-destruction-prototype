using UnityEngine;
using DG.Tweening;

namespace PixelDestruction.UI
{
    public class UIPanelAnim : MonoBehaviour
    {
        private void OnEnable()
        {
            transform.localScale = Vector3.zero; 

            transform.DOScale(Vector3.one, 0.4f)
                     .SetEase(Ease.OutBack)
                     .SetUpdate(true);
        }
    }
}