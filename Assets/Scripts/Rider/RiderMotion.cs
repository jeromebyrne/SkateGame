using UnityEngine;

public class RiderMotion : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _riderRigidBody = null;

    private readonly Vector2 JUMP_FORCE = new Vector2(0.0f, 275.0f);
    private readonly Vector2 PUSH_FORCE = new Vector2(1000.0f, 0.0f);
    private const float MAX_VELOCITY_X = 120.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
        _riderRigidBody.AddForce(JUMP_FORCE, ForceMode2D.Impulse);
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
