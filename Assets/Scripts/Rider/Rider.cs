using UnityEngine;

public class Rider : MonoBehaviour
{
    public Rigidbody2D _rigidBody = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            float pushSpeed = 1.0f;
            Vector2 newVel = _rigidBody.velocity + new Vector2(_rigidBody.transform.right.x * pushSpeed, 0.0f);

            _rigidBody.velocity = newVel;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            /*
            float pushSpeed = 0.01f;
            Vector2 newVel = deckRB.velocity + new Vector2(deckRB.transform.right.x * pushSpeed, 0.0f);

            deckRB.velocity = newVel;
            */

            _rigidBody.AddForce(new Vector2(0.0f, 90.0f), ForceMode2D.Impulse);
        }
    }
}
