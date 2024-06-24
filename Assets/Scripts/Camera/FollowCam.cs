using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField] private Camera _camera = null;
    [SerializeField] private GameObject _objectToFollow = null;
    [SerializeField] private float _xOffset = 0f;
    [SerializeField] private float _yOffset = 0f;

    private Vector3 _newPosition;

    private void Start()
    {
        _newPosition = _camera.transform.position;
    }

    // LateUpdate is called once per frame, after all Update methods have been called
    private void LateUpdate()
    {
        // Update _newPosition with offsets
        _newPosition.x = _objectToFollow.transform.position.x + _xOffset;
        _newPosition.y = _objectToFollow.transform.position.y + _yOffset;

        // Apply the new position to the camera
        _camera.transform.position = _newPosition;
    }
}