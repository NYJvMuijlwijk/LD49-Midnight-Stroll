using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class CenterOfGravityMovement : MonoBehaviour
{
    [Header("Rotation")] [SerializeField] private float rotationSpeed = 25f;
    [SerializeField] private float drunkRotationSpeed = 7f;

    [Header("Random")] [SerializeField] private float randomRotationSpeed = 7f;
    [SerializeField] private Vector2 randomTimerRange = new Vector2(1f, 3f);

    [Header("Movement")] [SerializeField] private float turnSpeed = 1f;
    [SerializeField] private float maxMoveStrength = 500f;
    [SerializeField] private float maxWalkingAngle = 30f;

    private Transform _transform;
    private Rigidbody _rigidbody;

    private Vector2 _randomRotateDirection = Vector2.zero;
    private float _randomTimer;
    private bool _wasted;

    private void Start()
    {
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody>();
        SetRandomMovementVariables();
        StartCoroutine(nameof(RandomRotationCoroutine));
    }

    void Update()
    {
        if (_wasted)
            if (Input.GetKey(KeyCode.Space))
                ResetPlayer();
            else
                return;

        RotatePlayer();
        TurnPlayer();
        AddExtraRotation();
        AddRandomRotation();

        CheckForRagdoll();
    }

    private void ResetPlayer()
    {
        _wasted = false;
        _transform.position = Vector3.up;
        _transform.rotation = Quaternion.identity;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        StartCoroutine(nameof(RandomRotationCoroutine));
    }

    private void CheckForRagdoll()
    {
        Vector2 angles = GetNegativeAllowedXZRotation();

        if (!(Mathf.Abs(angles.x) > maxWalkingAngle) && !(Mathf.Abs(angles.y) > maxWalkingAngle)) return;

        GetWasted();
    }

    private void GetWasted()
    {
        Debug.Log("Wasted");
        _wasted = true;
        _rigidbody.constraints = RigidbodyConstraints.None;

        StopCoroutine(nameof(RandomRotationCoroutine));
    }

    private IEnumerator RandomRotationCoroutine()
    {
        while (true)
        {
            _randomTimer -= Time.deltaTime;

            if (_randomTimer < 0)
                SetRandomMovementVariables();

            yield return null;
        }
    }

    private void SetRandomMovementVariables()
    {
        Debug.Log("Updating random variables");
        _randomTimer = Random.Range(randomTimerRange.x, randomTimerRange.y);
        _randomRotateDirection = new Vector2(Random.value - .5f, Random.value - .5f).normalized;
    }

    private void AddRandomRotation()
    {
        Vector3 rotationDirection = new Vector3(_randomRotateDirection.x, 0, _randomRotateDirection.y).normalized *
                                    randomRotationSpeed * Time.deltaTime;

        _transform.rotation *= Quaternion.Euler(rotationDirection);
    }

    private void AddExtraRotation()
    {
        Vector2 angles = GetNegativeAllowedXZRotation();
        Vector3 rotationDirection = new Vector3(angles.x, 0, angles.y).normalized * drunkRotationSpeed * Time.deltaTime;

        _transform.rotation *= Quaternion.Euler(rotationDirection);
    }

    private void RotatePlayer()
    {
        _transform.rotation *= Quaternion.Euler(GetPlayerMovement() * Time.deltaTime);

        Vector2 angles = GetNegativeAllowedXZRotation();

        MovePlayer(angles);
    }

    private Vector2 GetNegativeAllowedXZRotation()
    {
        Vector3 rotation = _transform.rotation.eulerAngles;

        float xAngle = rotation.x > 180
            ? rotation.x - 360
            : rotation.x;
        float zAngle = rotation.z > 180
            ? rotation.z - 360
            : rotation.z;

        return new Vector2(xAngle, zAngle);
    }

    private void MovePlayer(Vector2 xzAngles)
    {
        Vector3 direction = new Vector3(
            -Mathf.Clamp(xzAngles.y / maxWalkingAngle, -1, 1),
            0,
            Mathf.Clamp(xzAngles.x / maxWalkingAngle, -1, 1)
        ) * maxMoveStrength * Time.deltaTime;

        _rigidbody.velocity = Quaternion.Euler(0, _transform.rotation.eulerAngles.y, 0) * direction;
    }

    private Vector3 GetPlayerMovement()
    {
        Vector3 rotation = Vector3.zero;

        if (Input.GetKey(KeyCode.A))
            rotation.z += rotationSpeed;
        if (Input.GetKey(KeyCode.D))
            rotation.z -= rotationSpeed;
        if (Input.GetKey(KeyCode.W))
            rotation.x += rotationSpeed;
        if (Input.GetKey(KeyCode.S))
            rotation.x -= rotationSpeed;

        return rotation;
    }

    private void TurnPlayer()
    {
        if (Input.GetKey(KeyCode.Q))
            _transform.RotateAround(transform.position, Vector3.up, -turnSpeed);
        if (Input.GetKey(KeyCode.E))
            _transform.RotateAround(transform.position, Vector3.up, turnSpeed);
    }
}