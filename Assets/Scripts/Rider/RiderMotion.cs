using UnityEngine;

public class RiderMotion : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _riderRigidBody = null;
    [SerializeField] private RiderStatus _riderStatus = null;

    private readonly Vector2 BASE_JUMP_FORCE = new Vector2(0.0f, 250.0f);
    private const float PUSH_FORCE = 350.0f;
    private const float MAX_VELOCITY_X = 200.0f;
    private const float JUMP_FORCE_MULTIPLIER = 3.0f;
    private const float MAX_JUMP_FORCE = 375.0f;
    private const float MIN_FORWARD_VELOCITY = 75.0f; // Minimum forward velocity to maintain
    private const float FORWARD_FORCE_MULTIPLIER = 5.0f; // Multiplier for forward force when velocity drops

    void FixedUpdate()
    {
        ClampVelocityX();
        EnsureForwardMovement();
    }

    public void Push(float percent)
    {
        if (percent > 1.0f) percent = 1.0f;
        else if (percent < 0.0f) percent = 0.0f;

        // Calculate the push direction based on the ground's orientation
        Vector2 pushDirection = Vector2.right * (percent * PUSH_FORCE); // Assuming right is the ground-relative forward direction
        _riderRigidBody.AddForce(pushDirection, ForceMode2D.Force);
    }

    public void Jump()
    {
        float xVelocity = Mathf.Abs(_riderRigidBody.velocity.x);
        float proportionalJumpForceMagnitude = BASE_JUMP_FORCE.y + (xVelocity * JUMP_FORCE_MULTIPLIER);
        proportionalJumpForceMagnitude = Mathf.Min(proportionalJumpForceMagnitude, MAX_JUMP_FORCE);

        // set the y velocity to 0 before we jump
        Vector2 currentVelocity = _riderRigidBody.velocity;
        currentVelocity.y = 0; // Set Y velocity to 0
        _riderRigidBody.velocity = currentVelocity;

        // Calculate the jump force direction based on the ground's orientation
        Vector2 jumpDirection = Vector2.up * proportionalJumpForceMagnitude; // Assuming up is the ground-relative up direction
        _riderRigidBody.AddForce(jumpDirection, ForceMode2D.Impulse);
    }

    private void ClampVelocityX()
    {
        if (_riderStatus.AreAnyWheelsTouchingSolid() == false)
        {
            return;
        }

        Vector2 currentVelocity = _riderRigidBody.velocity;
        Vector2 forwardDirection = Vector2.right; // Assuming right is the ground-relative forward direction
        float forwardVelocity = Vector2.Dot(currentVelocity, forwardDirection);

        if (forwardVelocity > MAX_VELOCITY_X)
        {
            forwardVelocity = MAX_VELOCITY_X;
        }

        currentVelocity = forwardDirection * forwardVelocity + Vector2.Perpendicular(forwardDirection) * Vector2.Dot(currentVelocity, Vector2.Perpendicular(forwardDirection));
        _riderRigidBody.velocity = currentVelocity;
    }

    private void EnsureForwardMovement()
    {
        if (_riderStatus.AreAnyWheelsTouchingSolid() == false)
        {
            return;
        }

        Vector2 forwardDirection = Vector2.right; // Assuming right is the ground-relative forward direction
        float forwardVelocity = Vector2.Dot(_riderRigidBody.velocity, forwardDirection);

        if (forwardVelocity < 0)
        {
            // Directly set the velocity to ensure forward movement
            Vector2 correctedVelocity = forwardDirection * MIN_FORWARD_VELOCITY + Vector2.Perpendicular(forwardDirection) * Vector2.Dot(_riderRigidBody.velocity, Vector2.Perpendicular(forwardDirection));
            _riderRigidBody.velocity = correctedVelocity;
        }
        else if (forwardVelocity < MIN_FORWARD_VELOCITY)
        {
            // Apply a strong forward force to maintain minimum forward velocity
            Vector2 forwardForce = forwardDirection * (PUSH_FORCE * FORWARD_FORCE_MULTIPLIER);
            _riderRigidBody.AddForce(forwardForce, ForceMode2D.Force);
        }
    }
}