using System.Linq;
using UnityEngine;

public class RiderStatus : MonoBehaviour
{
    public const float WHEEL_RAYCAST_DISTANCE = 60.0f;
    private const float WHEEL_ORIGIN_OFFSET_Y = -0.5f;
    private const float WHEEL_ON_SURFACE_THRESHOLD = 3.0f;

    [SerializeField] private Rigidbody2D _riderRigidBody = null;
    [SerializeField] private Transform _backWheelTransform = null;
    [SerializeField] private Transform _frontWheelTransform = null;
    private Vector2 _posOffset;

    public RaycastHit2D BackWheelRaycastHit { get; private set; }
    public RaycastHit2D FrontWheelRaycastHit { get; private set; }

    public bool IsDoingManual { get; private set; }
    public bool IsDoingNoseManual { get; private set; }

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

        UpdateManual();
    }

    public void TryToManual()
    {
        if (IsDoingNoseManual)
        {
            return;
        }

        if (!IsBackWheelTouchingSolid())
        {
            return;
        }

        // RiderStabilizer component will stabilize the manual
        IsDoingManual = true;
    }

    public void TryToNoseManual()
    {
        if (IsDoingNoseManual)
        {
            return;
        }

        if (!IsFrontWheelTouchingSolid())
        {
            return;
        }

        // RiderStabilizer component will stabilize the manual
        IsDoingNoseManual = true;
    }

    public void StopManual()
    {
        IsDoingManual = false;
    }

    public void StopNoseManual()
    {
        IsDoingNoseManual = false;
    }

    private void UpdateRaycasts()
    {
        // reset 
        BackWheelRaycastHit = default(RaycastHit2D);
        FrontWheelRaycastHit = default(RaycastHit2D);

        Vector2 downwardDirection = -_riderRigidBody.transform.up; // Downward direction based on rider's orientation

        RaycastHit2D[] backWheelHits = Physics2D.RaycastAll((Vector2)_backWheelTransform.position + _posOffset,
                                                                downwardDirection,
                                                                WHEEL_RAYCAST_DISTANCE /* TODO: add layer */);
        RaycastHit2D[] frontWheelHits = Physics2D.RaycastAll((Vector2)_frontWheelTransform.position + _posOffset,
                                                                downwardDirection,
                                                                WHEEL_RAYCAST_DISTANCE /* TODO: add layer */);

        //TODO: optimize
        backWheelHits = backWheelHits.OrderBy(hit => hit.distance).ToArray();
        frontWheelHits = frontWheelHits.OrderBy(hit => hit.distance).ToArray();

        // back wheel
        foreach (RaycastHit2D hit in backWheelHits)
        {
            // Check if the hit collider should be ignored
            if (ShouldIgnoreColliderinRaycast(hit.collider))
            {
                continue;
            }

            BackWheelRaycastHit = hit;
            break;
        }

        // front wheel
        foreach (RaycastHit2D hit in frontWheelHits)
        {
            // Check if the hit collider should be ignored
            if (ShouldIgnoreColliderinRaycast(hit.collider))
            {
                continue;
            }

            FrontWheelRaycastHit = hit;
            break;
        }
    }

    bool ShouldIgnoreColliderinRaycast(Collider2D collider)
    {
        if (collider.tag == "Rider") // TODO: this is not efficient
        {
            return true;
        }

        return false;
    }

    void UpdateManual()
    {
        if (!AreAnyWheelsTouchingSolid())
        {
            StopManual();
            StopNoseManual();
            return;
        }

        float frontWheelDistance = 99999.0f;
        float backWheelDistance = 99999.0f;
        bool isBackWheelsOnManualPad = false;
        bool isFrontWheelsOnManualpad = false;

        if (IsBackWheelTouchingSolid())
        {
            if (BackWheelRaycastHit.collider.tag == "ManualPad")
            {
                backWheelDistance = BackWheelRaycastHit.distance;
                isBackWheelsOnManualPad = true;
            }
        }

        if (IsFrontWheelTouchingSolid())
        {
            if (FrontWheelRaycastHit.collider.tag == "ManualPad")
            {
                frontWheelDistance = FrontWheelRaycastHit.distance;
                isFrontWheelsOnManualpad = true;
            }
        }

        if (isBackWheelsOnManualPad && isFrontWheelsOnManualpad)
        {
            if (backWheelDistance < frontWheelDistance)
                TryToManual();
            else
                TryToNoseManual();
        }
        else if (isBackWheelsOnManualPad)
        {
            TryToManual();
        }
        else if (isFrontWheelsOnManualpad)
        {
            TryToNoseManual();
        }
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