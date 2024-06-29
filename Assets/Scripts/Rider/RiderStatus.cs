using UnityEngine;

public class RiderStatus : MonoBehaviour
{
    public const float WHEEL_RAYCAST_DISTANCE = 60.0f;
    private const float WHEEL_ORIGIN_OFFSET_Y = -1.5f;
    private const float WHEEL_ON_SURFACE_THRESHOLD = 2.0f;

    [SerializeField] private Rigidbody2D _riderRigidBody = null;
    [SerializeField] private Transform _backWheelTransform = null;
    [SerializeField] private Transform _frontWheelTransform = null;
    private Vector2 _posOffset;

    public RaycastHit2D BackWheelRaycastHit { get; private set; }
    public RaycastHit2D FrontWheelRaycastHit { get; private set; }

    public bool IsDoingManual { get; private set; }

    void Start()
    {
        _posOffset = new Vector2(0, WHEEL_ORIGIN_OFFSET_Y);
    }

    public bool AreAnyWheelsTouchingSolid()
    {
        return IsBackWheelTouchingSolid() || IsFrontWheelTouchingSolid();
    }

    public bool AreAllWheelsTouchingSolid()
    {
        return IsBackWheelTouchingSolid() && IsFrontWheelTouchingSolid();
    }

    public bool IsBackWheelTouchingSolid()
    {
        return BackWheelRaycastHit.collider != null && BackWheelRaycastHit.distance < WHEEL_ON_SURFACE_THRESHOLD;
    }

    public bool IsFrontWheelTouchingSolid()
    {
        return FrontWheelRaycastHit.collider != null && FrontWheelRaycastHit.distance < WHEEL_ON_SURFACE_THRESHOLD;
    }

    void FixedUpdate()
    {
        UpdateRaycasts();
    }

    public void TryToManual()
    {
        if (!IsBackWheelTouchingSolid())
        {
            return;
        }

        // RiderStabilizer component will stabilize the manual
        IsDoingManual = true;
    }

    public void StopManual()
    {
        IsDoingManual = false;
    }

    private void UpdateRaycasts()
    {
        Vector2 downwardDirection = -_riderRigidBody.transform.up; // Downward direction based on rider's orientation

        // Calculate the world position of the raycast origins
        Vector2 backWheelOrigin = (Vector2)_backWheelTransform.position + _posOffset;
        Vector2 frontWheelOrigin = (Vector2)_frontWheelTransform.position + _posOffset;

        BackWheelRaycastHit = Physics2D.Raycast(backWheelOrigin, downwardDirection, WHEEL_RAYCAST_DISTANCE);
        FrontWheelRaycastHit = Physics2D.Raycast(frontWheelOrigin, downwardDirection, WHEEL_RAYCAST_DISTANCE);
    }

    void OnDrawGizmos()
    {
        if (_frontWheelTransform == null || _backWheelTransform == null)
        {
            return;
        }

        Vector2 downwardDirection = -_riderRigidBody.transform.up; // Downward direction based on rider's orientation

        // Calculate the world position of the raycast origins
        Vector2 backWheelOrigin = (Vector2)_backWheelTransform.position + _posOffset;
        Vector2 frontWheelOrigin = (Vector2)_frontWheelTransform.position + _posOffset;

        { // front wheel
            Vector2 dest = frontWheelOrigin + (downwardDirection * WHEEL_RAYCAST_DISTANCE);
            Gizmos.color = FrontWheelRaycastHit.collider == null ? Color.white : Color.yellow;
            Gizmos.DrawLine(frontWheelOrigin, dest);
        }

        { // front wheel (is on solid check)
            Vector2 dest = frontWheelOrigin + (downwardDirection * WHEEL_ON_SURFACE_THRESHOLD);
            Gizmos.color = IsFrontWheelTouchingSolid() ? Color.green : Color.red;
            Gizmos.DrawLine(frontWheelOrigin, dest);
        }

        { // back wheel
            Vector2 dest = backWheelOrigin + (downwardDirection * WHEEL_RAYCAST_DISTANCE);
            Gizmos.color = BackWheelRaycastHit.collider == null ? Color.white : Color.yellow;
            Gizmos.DrawLine(backWheelOrigin, dest);
        }

        { // back wheel (is on solid check)
            Vector2 dest = backWheelOrigin + (downwardDirection * WHEEL_ON_SURFACE_THRESHOLD);
            Gizmos.color = IsBackWheelTouchingSolid() ? Color.green : Color.red;
            Gizmos.DrawLine(backWheelOrigin, dest);
        }
    }
}