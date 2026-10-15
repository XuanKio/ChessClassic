using UnityEngine;
using UnityEngine.UI;

namespace ChessClassic.Presentation
{
    /// <summary>Resolution-independent, non-interactive dot or capture ring.</summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class MoveHintGraphic : MaskableGraphic
    {
        private bool capture;

        public void SetCapture(bool value)
        {
            capture = value;
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            const int segments = 64;
            var rect = rectTransform.rect;
            float size = Mathf.Min(rect.width, rect.height);
            float outer = size * (capture ? .48f : .165f);
            float inner = capture ? size * .405f : 0;
            for (int i = 0; i <= segments; i++)
            {
                float angle = 2 * Mathf.PI * i / segments;
                var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                mesh.AddVert(rect.center + direction * outer, color, Vector2.zero);
                mesh.AddVert(rect.center + direction * inner, color, Vector2.zero);
                if (i == 0) continue;
                int current = i * 2;
                mesh.AddTriangle(current - 2, current, current + 1);
                mesh.AddTriangle(current - 2, current + 1, current - 1);
            }
        }
    }
}
