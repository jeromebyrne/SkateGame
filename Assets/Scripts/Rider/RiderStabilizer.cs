using UnityEngine;

public class RiderStabilizer : MonoBehaviour
{
    [SerializeField] private Transform _riderTransform = null;
    [SerializeField] private Rigidbody2D _riderRigidbody;
    [SerializeField] private RiderStatus _riderStatus;
    private Quaternion _initialRotation;

    private const float UPRIGHT_TORQUE = 5.0f; // Adjust this value for the torque strength
    private const float MIN_TORQUE_MULTIPLIER = 0.5f; // Minimum torque multiplier
    private const float MAX_TORQUE_MULTIPLIER = 2.5f; // Maximum torque multiplier

    private const float DEFAULT_TORQUE_MULTIPLIER = 0.45f; // Maximum torque multiplier

    void Start()
    {
        _initialRotation = _riderTransform.rotation;
    }

    void FixedUpdate()
    {
        if (!_riderStatus.AreAnyWheelsTouchingSolid()) // no need to orient if touching a surface
        {
            DoAutoOrient();
        }
    }

    private void DoAutoOrient()
    {
        RaycastHit2D hit = _riderStatus.BackWheelRaycastHit.collider != null ? _riderStatus.BackWheelRaycastHit : _riderStatus.FrontWheelRaycastHit;

        if (hit.collider != null)
        {
            // Calculate the angle based on the normal of the hit point
            float angle = Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

            // Calculate the torque multiplier based on the distance (closer = higher torque)
            float torqueMultiplier = Mathf.Lerp(MAX_TORQUE_MULTIPLIER, MIN_TORQUE_MULTIPLIER, hit.distance / RiderStatus.WHEEL_RAYCAST_DISTANCE);

            // Apply torque to match the angle of the collider's normal
            ApplyTorqueToMatchRotation(targetRotation, torqueMultiplier);
        }
        else
        {
            // Apply torque to return to the initial upright rotation
            ApplyTorqueToMatchRotation(_initialRotation, DEFAULT_TORQUE_MULTIPLIER);
        }
    }

    private void ApplyTorqueToMatchRotation(Quaternion targetRotation, float torqueMultiplier)
    {
        float angleDifference = Mathf.DeltaAngle(_riderTransform.eulerAngles.z, targetRotation.eulerAngles.z);
        float torque = angleDifference * UPRIGHT_TORQUE * torqueMultiplier;
        _riderRigidbody.AddTorque(torque);
    }
}