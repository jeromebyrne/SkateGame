using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField] Camera camera = null;
    [SerializeField] GameObject objectToFollow = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        camera.transform.position = new Vector3(objectToFollow.transform.position.x,
                                                objectToFollow.transform.position.y,
                                                camera.transform.position.z);
    }
}
