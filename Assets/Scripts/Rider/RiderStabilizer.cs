using UnityEngine;

public class RiderStabilizer : MonoBehaviour
{
    [SerializeField] private Transform _riderTransform = null;
    [SerializeField] private Transform _rayCastOriginTransform = null;
    private Quaternion _targetRotation;

    private const float POS_OFFSET_Y = -0.5f;
    private const float RAYCAST_DISTANCE = 20.0f;

    private Vector3 _posOffset;

    void Start()
    {
        _targetRotation = Quaternion.identity;
        _posOffset = new Vector3(0, POS_OFFSET_Y, 0.0f);
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(_rayCastOriginTransform.position + _posOffset, -Vector2.up, RAYCAST_DISTANCE);

        if (hit.collider != null &&
            hit.collider.GetComponent<Rigidbody2D>() == null)
        {
            // Calculate the angle based on the normal of the hit point
            float angle = Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

            // Adjust rotation to match the angle of the collider's normal
            _riderTransform.rotation = Quaternion.Slerp(_riderTransform.rotation, targetRotation, Time.deltaTime * 3.0f);
        }
        else
        {
            // Return to the target rotation
            _riderTransform.rotation = Quaternion.Slerp(_riderTransform.rotation, _targetRotation, Time.deltaTime * 3.0f);
        }
    }

    void OnDrawGizmos()
    {
        if (_rayCastOriginTransform == null)
        {
            return;
        }

        Vector2 origin = _rayCastOriginTransform.position + _posOffset;
        Vector2 dest = origin + (-Vector2.up * RAYCAST_DISTANCE);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, dest);
        
    }
}