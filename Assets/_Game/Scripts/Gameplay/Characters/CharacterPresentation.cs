using System.Collections;
using UnityEngine;

// Visual reactions only: animation and the sprite material's hit-flash property.
public sealed class CharacterPresentation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer characterRenderer;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private CharacterHealth health;
    [SerializeField, Min(0.01f)] private float hitFlashDuration = 0.12f;

    private Coroutine hitFlashRoutine;
    private MaterialPropertyBlock hitFlashProperties;
    private bool isDead;

    private static readonly int IsMovingId = Animator.StringToHash("IsMoving");
    private static readonly int IsDeadId = Animator.StringToHash("IsDead");
    private static readonly int FlashAmountId = Shader.PropertyToID("_FlashAmount");

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (characterRenderer == null) characterRenderer = GetComponent<SpriteRenderer>();
        if (body == null) body = GetComponentInParent<Rigidbody2D>();
        if (health == null) health = GetComponentInParent<CharacterHealth>();
        hitFlashProperties = new MaterialPropertyBlock();
    }

    private void OnEnable()
    {
        if (health == null) return;

        health.OnDamaged += PlayHitFlash;
        health.OnDied += SetDead;
        if (health.IsDead) SetDead();
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnDamaged -= PlayHitFlash;
            health.OnDied -= SetDead;
        }

        StopHitFlash();
    }

    private void Update()
    {
        if (animator != null && body != null && !isDead)
            animator.SetBool(IsMovingId, body.linearVelocity.sqrMagnitude > 0.04f);
    }

    public void PlayHitFlash()
    {
        if (isDead || characterRenderer == null) return;

        StopHitFlash();
        hitFlashRoutine = StartCoroutine(HitFlash());
    }

    public void SetDead()
    {
        if (isDead) return;

        isDead = true;
        StopHitFlash();
        if (animator != null)
        {
            animator.SetBool(IsMovingId, false);
            animator.SetBool(IsDeadId, true);
        }
    }

    private IEnumerator HitFlash()
    {
        characterRenderer.GetPropertyBlock(hitFlashProperties);
        hitFlashProperties.SetFloat(FlashAmountId, 1f);
        characterRenderer.SetPropertyBlock(hitFlashProperties);

        yield return new WaitForSeconds(hitFlashDuration);

        ResetHitFlash();
        hitFlashRoutine = null;
    }

    private void StopHitFlash()
    {
        if (hitFlashRoutine != null)
        {
            StopCoroutine(hitFlashRoutine);
            hitFlashRoutine = null;
        }

        ResetHitFlash();
    }

    private void ResetHitFlash()
    {
        if (characterRenderer == null || hitFlashProperties == null) return;

        characterRenderer.GetPropertyBlock(hitFlashProperties);
        hitFlashProperties.SetFloat(FlashAmountId, 0f);
        characterRenderer.SetPropertyBlock(hitFlashProperties);
    }
}
