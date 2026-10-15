using UnityEngine;
using System.Collections;

public sealed class CharacterPresentation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer characterRenderer;
    [SerializeField] private SpriteRenderer hitFlashRenderer;
    [SerializeField] private Rigidbody2D body;
    [SerializeField, Min(0.01f)] private float hitFlashDuration = 0.12f;

    private Coroutine hitFlashRoutine;
    private bool isDead;

    private static readonly int IsMovingId = Animator.StringToHash("IsMoving");
    private static readonly int IsDeadId = Animator.StringToHash("IsDead");

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (characterRenderer == null) characterRenderer = GetComponent<SpriteRenderer>();
        if (body == null && transform.parent != null) body = transform.parent.GetComponent<Rigidbody2D>();
        if (hitFlashRenderer == null)
        {
            Transform overlay = transform.Find("HitFlashOverlay");
            if (overlay != null) hitFlashRenderer = overlay.GetComponent<SpriteRenderer>();
        }
        if (hitFlashRenderer != null) hitFlashRenderer.enabled = false;
    }

    private void Update()
    {
        if (animator != null && body != null && !isDead)
            animator.SetBool(IsMovingId, body.linearVelocity.sqrMagnitude > 0.04f);

        if (hitFlashRoutine != null && hitFlashRenderer != null && characterRenderer != null)
            hitFlashRenderer.sprite = characterRenderer.sprite;
    }

    public void PlayHitFlash()
    {
        if (isDead || characterRenderer == null || hitFlashRenderer == null) return;
        if (hitFlashRoutine != null) StopCoroutine(hitFlashRoutine);
        hitFlashRoutine = StartCoroutine(HitFlash());
    }

    public void SetDead()
    {
        if (isDead) return;
        isDead = true;
        if (hitFlashRoutine != null) StopCoroutine(hitFlashRoutine);
        if (hitFlashRenderer != null) hitFlashRenderer.enabled = false;
        if (animator != null)
        {
            animator.SetBool(IsMovingId, false);
            animator.SetBool(IsDeadId, true);
        }
        if (body != null)
        {
            body.linearVelocity = Vector2.zero;
            body.simulated = false;
        }
    }

    private IEnumerator HitFlash()
    {
        hitFlashRenderer.sprite = characterRenderer.sprite;
        hitFlashRenderer.enabled = true;
        yield return new WaitForSeconds(hitFlashDuration);
        if (hitFlashRenderer != null) hitFlashRenderer.enabled = false;
        hitFlashRoutine = null;
    }
}