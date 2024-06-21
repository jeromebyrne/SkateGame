using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skateboard : MonoBehaviour
{
    public Rigidbody2D deckRB = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            float pushSpeed = 0.02f;
            Vector2 newVel = deckRB.velocity + new Vector2(deckRB.transform.right.x * pushSpeed, 0.0f);

            deckRB.velocity = newVel;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            /*
            float pushSpeed = 0.01f;
            Vector2 newVel = deckRB.velocity + new Vector2(deckRB.transform.right.x * pushSpeed, 0.0f);

            deckRB.velocity = newVel;
            */

            deckRB.AddForce(new Vector2(0.0f, 90.0f), ForceMode2D.Impulse);
        }
    }
}
