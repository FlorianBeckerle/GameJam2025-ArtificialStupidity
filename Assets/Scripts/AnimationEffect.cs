using UnityEngine;

public class CharacterAudio : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Audio Clips")]
    public AudioClip walkSFX;
    public AudioClip talkSFX;
    public AudioClip pickUpSFX;
    public AudioClip fallSFX;

    [Header("Settings")]
    public float footstepInterval = 0.4f; // Zeit zwischen Footsteps
    private float footstepTimer = 0f;

    void Update()
    {
        if (!animator) return;

        // Timer runterzählen
        footstepTimer -= Time.deltaTime;

        // ===========================
        // Walking
        // ===========================
        bool isWalking = animator.GetBool("isWalking");
        if (isWalking && footstepTimer <= 0f)
        {
            AudioManager.Instance.PlaySFX(walkSFX);
            footstepTimer = footstepInterval;
        }

        // ===========================
        // Talking
        // ===========================
        if (animator.GetBool("isTalking"))
        {
            AudioManager.Instance.PlaySFX(talkSFX);
        }

        // ===========================
        // Falling
        // ===========================
        if (animator.GetBool("fall"))
        {
            AudioManager.Instance.PlaySFX(fallSFX);
        }

        // ===========================
        // PickUp (Trigger)
        // ===========================
        if (animator.GetBool("pickUp"))
        {
            AudioManager.Instance.PlaySFX(pickUpSFX);
            animator.SetBool("pickUp", false); // Trigger zurücksetzen
        }

        // weitere Parameter analog abfragen
    }
}
