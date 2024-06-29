using UnityEngine;

public class RiderStabilizer : MonoBehaviour
{
    [SerializeField] private Transform _riderTransform = null;
    [SerializeField] private Rigidbody2D _riderRigidbody;
    [SerializeField] private RiderStatus _riderStatus;

    private const float MIN_TORQUE_MULTIPLIER = 0.25f; // Minimum torque multiplier
    private const float MAX_TORQUE_MULTIPLIER = 1.0f; // Maximum torque multiplier
    private const float DEFAULT_TORQUE_MULTIPLIER = 0.3f; // Default torque multiplier
    private const float BACK_WHEEL_BALANCE_ANGLE = 10.0f; // Angle to balance on the back wheel
    private const float ADDITIONAL_UPWARD_FORCE = 675.0f; // Reduced force to lift the front wheel
    private const float BALANCE_TORQUE = 40.0f; // Torque for balancing
    private const float MAX_BACK_ANGLE = 15.0f; // Maximum allowed tilt angle backwards
    private const float MAX_FORWARD_ANGLE = 10.0f; // Maximum allowed tilt angle forwards
    private const float DAMPING_FACTOR = 0.95f; // Damping factor to smooth forces

    void Start()
    {
        // Optionally, adjust the center of mass to help with balancing
        _riderRigidbody.centerOfMass = new Vector2(0, -0.65f);
    }

    void FixedUpdate()
    {
        if (!_riderStatus.AreAnyWheelsTouchingSolid()) // No need to orient if not touching a surface
        {
            DoAutoOrient();
        }
        else
        {
            if (_riderStatus.IsDoingManual)
            {
                DoManual();
            }
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
            ApplyTorqueToMatchRotation(Quaternion.identity, DEFAULT_TORQUE_MULTIPLIER);
        }
    }

    private void DoManual()
    {
        if (_riderStatus.IsBackWheelTouchingSolid())
        {
            // Apply an upward force at a fixed relative point to the rider to lift the front wheel
            _riderRigidbody.AddForceAtPosition(Vector2.up * ADDITIONAL_UPWARD_FORCE, _riderTransform.TransformPoint(new Vector2(0, -0.5f)));

            // Get the angle of the back wheel's contact point
            float angle = Mathf.Atan2(_riderStatus.BackWheelRaycastHit.normal.y, _riderStatus.BackWheelRaycastHit.normal.x) * Mathf.Rad2Deg - 90f;
            // Add offset to angle to balance on the back wheel
            angle += BACK_WHEEL_BALANCE_ANGLE;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

            // Apply increased torque to maintain balance on the back wheel
            ApplyTorqueToMatchRotation(targetRotation, 1.0f);

            // Apply corrective torque if the rider tilts too far back or forward
            float currentAngle = _riderTransform.eulerAngles.z;
            if (currentAngle > MAX_BACK_ANGLE && currentAngle < 360 - MAX_FORWARD_ANGLE)
            {
                ApplyTorqueToMatchRotation(Quaternion.identity, 1.0f);
            }
        }
    }

    private void ApplyTorqueToMatchRotation(Quaternion targetRotation, float torqueMultiplier)
    {
        float angleDifference = Mathf.DeltaAngle(_riderTransform.eulerAngles.z, targetRotation.eulerAngles.z);
        float torque = angleDifference * BALANCE_TORQUE * torqueMultiplier;

        // Apply damping to the torque
        _riderRigidbody.angularVelocity *= DAMPING_FACTOR;
        _riderRigidbody.AddTorque(torque);
    }
}