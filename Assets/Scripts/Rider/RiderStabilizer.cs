using UnityEngine;

public class RiderStabilizer : MonoBehaviour
{
    [SerializeField] private Transform _riderTransform = null;
    [SerializeField] private Transform _backWheelTransform = null;
    [SerializeField] private Transform _frontWheelTransform = null;
    [SerializeField] private Rigidbody2D _riderRigidbody;
    [SerializeField] private RiderStatus _riderStatus;
    private Quaternion _initialRotation;

    private const float POS_OFFSET_Y = -2.5f;
    private const float RAYCAST_DISTANCE = 25.0f;
    private const float UPRIGHT_TORQUE = 5.0f; // Adjust this value for the torque strength
    private const float MIN_TORQUE_MULTIPLIER = 0.5f; // Minimum torque multiplier
    private const float MAX_TORQUE_MULTIPLIER = 2.5f; // Maximum torque multiplier

    private Vector3 _posOffset;

    void Start()
    {
        _initialRotation = _riderTransform.rotation;
        _posOffset = new Vector3(0, POS_OFFSET_Y, 0.0f);
    }

    void FixedUpdate()
    {
        if (_riderStatus.AreAnyWheelsTouchingSolid() == false) // no need to orient if touching a surface
        {
            DoAutoOrient();
        }
    }

    private void DoAutoOrient()
    {
        RaycastHit2D hit = Physics2D.Raycast(_frontWheelTransform.position + _posOffset, Vector2.down, RAYCAST_DISTANCE);

        if (hit.collider != null)
        {
            // advanced
            {
                // Calculate the angle based on the normal of the hit point
                float angle = Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg - 90f;
                Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

                // Calculate the distance from the raycast origin to the hit point
                float distance = Vector2.Distance(_frontWheelTransform.position + _posOffset, hit.point);

                // Calculate the torque multiplier based on the distance (closer = higher torque)
                float torqueMultiplier = Mathf.Lerp(MAX_TORQUE_MULTIPLIER, MIN_TORQUE_MULTIPLIER, distance / RAYCAST_DISTANCE);

                // Apply torque to match the angle of the collider's normal
                ApplyTorqueToMatchRotation(targetRotation, torqueMultiplier);
            }

            // simple
            {
                /*
                // Calculate the angle based on the normal of the hit point
                float angle = Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg - 90f;
                Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

                // Apply torque to match the angle of the collider's normal
                ApplyTorqueToMatchRotation(targetRotation, 1.0f);
                */
            }
        }
        else
        {
            // Apply torque to return to the initial upright rotation
            ApplyTorqueToMatchRotation(_initialRotation, 0.5f);
        }
    }

    private void ApplyTorqueToMatchRotation(Quaternion targetRotation, float torqueMultiplier)
    {
        float angleDifference = Mathf.DeltaAngle(_riderTransform.eulerAngles.z, targetRotation.eulerAngles.z);
        float torque = angleDifference * UPRIGHT_TORQUE * torqueMultiplier;
        _riderRigidbody.AddTorque(torque);
    }

    void OnDrawGizmos()
    {
        if (_frontWheelTransform == null)
        {
            return;
        }

        Vector2 origin = _frontWheelTransform.position + _posOffset;
        Vector2 dest = origin + (Vector2.down * RAYCAST_DISTANCE);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, dest);
    }
}