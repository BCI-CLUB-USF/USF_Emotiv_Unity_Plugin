using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    private ImprovedBCIPlayer bciPlayer;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        bciPlayer = GetComponent<ImprovedBCIPlayer>();
    }

    void Update()
    {
        // Movement check
        Vector3 horizontalVelocity = rb.linearVelocity;
        horizontalVelocity.y = 0;
        bool isMoving = horizontalVelocity.magnitude > 0.1f;

        // Jump check
        bool isJumping = bciPlayer != null ? !bciPlayer.GetIsGrounded() : Mathf.Abs(rb.linearVelocity.y) > 0.1f;

        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isJumping", isJumping);
    }
}
