using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // Add this at the top

/// <summary>
/// Camera controller with smooth following and shake effects for BCI feedback
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -8);
    public float followSpeed = 20f; // Increased for closer/snappier follow
    public float rotationSpeed = 1f;
    
    [Header("Look Settings")]
    public bool lookAtTarget = true;
    public Vector3 lookOffset = Vector3.zero;
    
    [Header("Shake Settings")]
    public float shakeDecay = 1f;
    public float shakeIntensity = 1f;
    
    [Header("Mouse Look Settings")]
    public bool enableMouseLook = true;
    public float mouseSensitivity = 2f;
    public float minYAngle = -60f;
    public float maxYAngle = 80f;
    
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private float currentShakeIntensity = 0f;
    private float yaw = 0f;
    private float pitch = 0f;
    private InputAction mouseDeltaAction; // Add this field
    
    void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        
        // Auto-find target if not set
        // (Removed RPGMonsterBCIPlayer reference)
        // You may set the target manually or via inspector.
        
        // Initialize yaw/pitch from current rotation
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
        
        // Setup mouse delta action for Input System
        mouseDeltaAction = new InputAction(type: InputActionType.Value, binding: "<Mouse>/delta");
        mouseDeltaAction.Enable();
    }
    
    void LateUpdate()
    {
        if (target == null) return;

        // Mouse look using Input System
        if (enableMouseLook)
        {
            Vector2 mouseDelta = mouseDeltaAction.ReadValue<Vector2>() * mouseSensitivity;

            yaw += mouseDelta.x;
            pitch -= mouseDelta.y;
            pitch = Mathf.Clamp(pitch, minYAngle, maxYAngle);

            // Camera position: snap directly to offset from player
            Vector3 cameraOffset = Quaternion.Euler(pitch, yaw, 0f) * offset;
            transform.position = target.position + cameraOffset;

            // Camera looks at player
            transform.LookAt(target.position);
        }
        else if (lookAtTarget)
        {
            // Snap directly to follow position
            Vector3 desiredPosition = target.position + offset;
            transform.position = desiredPosition;

            Vector3 lookPosition = target.position + lookOffset;
            Vector3 direction = lookPosition - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        // Apply shake effect
        if (currentShakeIntensity > 0)
        {
            Vector3 shakeOffset = Random.insideUnitSphere * currentShakeIntensity * shakeIntensity;
            transform.position += shakeOffset;
            
            // Decay shake
            currentShakeIntensity = Mathf.Lerp(currentShakeIntensity, 0f, shakeDecay * Time.deltaTime);
        }
    }
    
    /// <summary>
    /// Trigger camera shake effect
    /// </summary>
    /// <param name="duration">Duration of shake</param>
    /// <param name="intensity">Intensity multiplier</param>
    public void Shake(float duration, float intensity = 1f)
    {
        StartCoroutine(ShakeCoroutine(duration, intensity));
    }
    
    IEnumerator ShakeCoroutine(float duration, float intensity)
    {
        currentShakeIntensity = intensity;
        yield return new WaitForSeconds(duration);
        currentShakeIntensity = 0f;
    }
    
    /// <summary>
    /// Set new follow target
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    /// <summary>
    /// Reset camera to original position and rotation
    /// </summary>
    public void ResetCamera()
    {
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
        currentShakeIntensity = 0f;
    }

    void OnDestroy()
    {
        // Clean up input action
        if (mouseDeltaAction != null)
            mouseDeltaAction.Disable();
    }
}
