using UnityEngine;

// Shows a CharacterHealth value by clipping the bar fill (a RectMask2D) to the frame's inner window.
public sealed class HealthBarView : MonoBehaviour
{
    [SerializeField] private CharacterHealth health;
    [SerializeField] private RectTransform fillMask;
    [Tooltip("Pixels of frame border that cover the fill at each end of the bar.")]
    [SerializeField, Min(0f)] private float frameInset = 3f;

    private void OnEnable()
    {
        if (health != null)
        {
            health.OnHealthChanged += Refresh;
            Refresh(health.CurrentHealth, health.MaxHealth);
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnHealthChanged -= Refresh;
        }
    }

    private void Refresh(float current, float max)
    {
        float fraction = max > 0f ? Mathf.Clamp01(current / max) : 0f;
        float barWidth = ((RectTransform)transform).rect.width;
        float windowWidth = Mathf.Max(0f, barWidth - 2f * frameInset);

        // Ceil keeps at least one pixel visible while any health remains.
        float maskWidth = frameInset + Mathf.Ceil(fraction * windowWidth);
        fillMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maskWidth);
    }
}
