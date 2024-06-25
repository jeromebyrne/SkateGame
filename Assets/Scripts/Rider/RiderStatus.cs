using UnityEngine;

public class RiderStatus : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _riderRigidBody = null;
    [SerializeField] private Collider2D _backWheelCollider = null;
    [SerializeField] private Collider2D _frontWheelCollider = null;

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
        return _backWheelCollider.IsTouchingLayers();
    }

    public bool IsFrontWheelTouchingSolid()
    {
        return _frontWheelCollider.IsTouchingLayers();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }
}
