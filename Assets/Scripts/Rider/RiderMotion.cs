using UnityEngine;

public class RiderMotion : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _riderRigidBody = null;

    private readonly Vector2 BASE_JUMP_FORCE = new Vector2(0.0f, 150.0f);
    private readonly Vector2 PUSH_FORCE = new Vector2(1000.0f, 0.0f);
    private const float MAX_VELOCITY_X = 150.0f;
    private const float JUMP_FORCE_MULTIPLIER = 0.6f;
    private const float MAX_JUMP_FORCE = 300.0f; 

    void FixedUpdate()
    {
        ClampVelocityX();
    }

    public void Push()
    {
        _riderRigidBody.AddForce(PUSH_FORCE, ForceMode2D.Force);
    }

    public void Jump()
    {
        float xVelocity = Mathf.Abs(_riderRigidBody.velocity.x);
        float proportionalJumpForceY = BASE_JUMP_FORCE.y + (xVelocity * JUMP_FORCE_MULTIPLIER);
        proportionalJumpForceY = Mathf.Min(proportionalJumpForceY, MAX_JUMP_FORCE); // Clamp to max jump force
        Vector2 proportionalJumpForce = new Vector2(0.0f, proportionalJumpForceY);

        _riderRigidBody.AddForce(proportionalJumpForce, ForceMode2D.Impulse);
    }

    private void ClampVelocityX()
    {
        Vector2 currentVelocity = _riderRigidBody.velocity;

        if (Mathf.Abs(currentVelocity.x) > MAX_VELOCITY_X)
        {
            currentVelocity.x = Mathf.Sign(currentVelocity.x) * MAX_VELOCITY_X;
        }

        _riderRigidBody.velocity = currentVelocity;
    }
}