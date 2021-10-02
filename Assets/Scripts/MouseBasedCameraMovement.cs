using System;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class MouseBasedCameraMovement : MonoBehaviour
{
    [SerializeField] private float rotationStrength = 100f;

    private CinemachineVirtualCamera _virtualCamera;
    private Transform _followTransform;

    private void Start()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        if (_virtualCamera != null) _followTransform = _virtualCamera.Follow;
    }

    void Update()
    {
        if (_followTransform == null || !Input.GetKey(KeyCode.Mouse0)) return;

        Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));

        if (mouseInput == Vector2.zero) return;

        // Rotating
        Quaternion rotation = _followTransform.rotation;
        rotation *= Quaternion.AngleAxis(mouseInput.x * rotationStrength * Time.deltaTime, Vector3.up);
        rotation *= Quaternion.AngleAxis(mouseInput.y * rotationStrength * Time.deltaTime, Vector3.right);

        _followTransform.rotation = rotation;

        // Clamping
        Vector3 angles = _followTransform.localEulerAngles;
        angles.z = 0;

        if (angles.x > 180 && angles.x < 340)
            angles.x = 340;
        if (angles.x < 180 && angles.x > 65)
            angles.x = 65;
        
        _followTransform.localEulerAngles = angles;
    }
}