using UnityEngine;
using TMPro;

namespace PixelDestruction.UI
{
    [ExecuteAlways] 
    [RequireComponent(typeof(TMP_Text))]
    public class CurvedTextTMP : MonoBehaviour
    {
        [Header("Curve Settings")]
        public AnimationCurve textCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));
        
        public float curveMultiplier = 10f;

        private TMP_Text tmpText;

        private void Awake()
        {
            tmpText = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            if (tmpText == null) return;

            tmpText.ForceMeshUpdate();
            TMP_TextInfo textInfo = tmpText.textInfo;

            float boundsMinX = tmpText.bounds.min.x;
            float boundsMaxX = tmpText.bounds.max.x;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible) continue;

                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                for (int j = 0; j < 4; j++)
                {
                    Vector3 origPos = vertices[vertexIndex + j];
                    
                    float t = (origPos.x - boundsMinX) / (boundsMaxX - boundsMinX);
                    
                    origPos.y += textCurve.Evaluate(t) * curveMultiplier;
                    
                    vertices[vertexIndex + j] = origPos;
                }
            }
            
            for (int i = 0; i < textInfo.materialCount; i++)
            {
                if (textInfo.meshInfo[i].mesh != null)
                {
                    textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                    tmpText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                }
            }
        }
    }
}