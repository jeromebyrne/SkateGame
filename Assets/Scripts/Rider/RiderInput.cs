using UnityEngine;

public class RiderInput : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidBody = null;
    [SerializeField] private RiderMotion _riderMotion = null;
    [SerializeField] private RiderStatus _riderStatus = null;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // auto-skate
        if (_riderStatus.AreAnyWheelsTouchingSolid())
        {
            _riderMotion.Push(1.0f);
        }

        if (Input.GetMouseButtonUp(0) &&
            _riderStatus.AreAnyWheelsTouchingSolid())
        {
            _riderMotion.Jump();
        }
    }
}
