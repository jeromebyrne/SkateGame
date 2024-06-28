using UnityEngine;

public class RiderMotion : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _riderRigidBody = null;
    [SerializeField] private RiderStatus _riderStatus = null;

    private readonly Vector2 BASE_JUMP_FORCE = new Vector2(0.0f, 150.0f);
    private const float PUSH_FORCE = 650.0f; // Single float constant for push force
    private const float MAX_VELOCITY_X = 500.0f;
    private const float JUMP_FORCE_MULTIPLIER = 0.6f;
    private const float MAX_JUMP_FORCE = 300.0f;

    void FixedUpdate()
    {
        ClampVelocityX();
    }

    public void Push(float percent)
    {
        if (percent > 1.0f) percent = 1.0f;
        else if (percent < 0.0f) percent = 0.0f;

        // Calculate the push direction based on the rider's current forward direction
        Vector2 pushDirection = transform.right * (percent * PUSH_FORCE);

        _riderRigidBody.AddForce(pushDirection, ForceMode2D.Force);
    }

    public void Jump()
    {
        float xVelocity = Mathf.Abs(_riderRigidBody.velocity.x);
        float proportionalJumpForceMagnitude = BASE_JUMP_FORCE.y + (xVelocity * JUMP_FORCE_MULTIPLIER);
        proportionalJumpForceMagnitude = Mathf.Min(proportionalJumpForceMagnitude, MAX_JUMP_FORCE); // Clamp to max jump force

        // Calculate the jump force direction based on the rider's current up direction
        Vector2 jumpDirection = transform.up * proportionalJumpForceMagnitude;

        _riderRigidBody.AddForce(jumpDirection, ForceMode2D.Impulse);
    }

    private void ClampVelocityX()
    {
        if (_riderStatus.AreAnyWheelsTouchingSolid())
        {
            Vector2 currentVelocity = _riderRigidBody.velocity;

            // Calculate forward direction based on the rider's orientation
            Vector2 forwardDirection = transform.right;
            float forwardVelocity = Vector2.Dot(currentVelocity, forwardDirection);

            if (Mathf.Abs(forwardVelocity) > MAX_VELOCITY_X)
            {
                float clampedForwardVelocity = Mathf.Sign(forwardVelocity) * MAX_VELOCITY_X;
                currentVelocity = forwardDirection * clampedForwardVelocity + Vector2.Perpendicular(forwardDirection) * Vector2.Dot(currentVelocity, Vector2.Perpendicular(forwardDirection));
            }

            _riderRigidBody.velocity = currentVelocity;
        }
    }
}