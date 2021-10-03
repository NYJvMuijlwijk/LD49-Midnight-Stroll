using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private Transform backTransform;

    [Header("Rotation")] [SerializeField] private float rotationSpeed = 25f;
    [SerializeField] private float drunkRotationSpeed = 8f;

    [Header("Random")] [SerializeField] private float randomRotationSpeed = 5f;
    [SerializeField] private Vector2 randomTimerRange = new Vector2(1f, 3f);

    [Header("Movement")] [SerializeField] private float turnSpeed = 1f;
    [SerializeField] private float maxMoveStrength = 500f;
    [SerializeField] private float carImpactStrength = 15f;
    [SerializeField] private float maxWalkingAngle = 30f;
    [SerializeField] private float walkingHeight = 1f;
    [SerializeField] private LayerMask hitMask = 6;

    [HideInInspector] public bool wasted;

    public event Action PlayerDeath;

    private Transform _transform;
    private Rigidbody _rigidbody;

    private Vector2 _randomRotateDirection = Vector2.zero;
    private float _randomTimer;

    #region UnityEvents

    private void Awake()
    {
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        SetRandomMovementVariables();
        StartCoroutine(nameof(RandomRotationCoroutine));
    }

    private void Update()
    {
        if (wasted)
            return;

        RotatePlayer();
        TurnPlayer();
        // AddExtraRotation();
        // AddRandomRotation();

        MaintainStanding();

        CheckForRagdoll();
    }

    private void MaintainStanding()
    {
        Ray groundRay = new Ray(_rigidbody.position, Vector3.down);
        if (Physics.Raycast(groundRay, out RaycastHit hit, walkingHeight, hitMask))
        {
            _rigidbody.position += Vector3.up * (walkingHeight - hit.distance);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Car")) return;

        if (!wasted)
            Die();

        Vector3 direction = _transform.position - other.transform.position;

        _rigidbody.AddForce(direction.normalized * carImpactStrength, ForceMode.Impulse);
    }

    #endregion

    #region Public

    public void ResetPlayer()
    {
        wasted = false;
        _transform.position = spawnPosition != null ? spawnPosition.position : Vector3.zero;
        _transform.rotation = spawnPosition != null ? spawnPosition.rotation : Quaternion.identity;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        StartCoroutine(nameof(RandomRotationCoroutine));
    }

    public void GetWasted()
    {
        Debug.Log("Wasted");
        wasted = true;
        _rigidbody.constraints = RigidbodyConstraints.None;

        StopCoroutine(nameof(RandomRotationCoroutine));
    }

    public void Die()
    {
        Debug.Log("Die");
        GetWasted();

        PlayerDeath?.Invoke();
    }

    #endregion

    #region Private

    private void CheckForRagdoll()
    {
        Vector2 angles = GetNegativeAllowedXZRotation();

        if (!(Mathf.Abs(angles.x) > maxWalkingAngle) && !(Mathf.Abs(angles.y) > maxWalkingAngle)) return;

        Die();
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
        _randomTimer = Random.Range(randomTimerRange.x, randomTimerRange.y);
        _randomRotateDirection = new Vector2(Random.value - .5f, Random.value - .5f).normalized;
    }

    private void AddRandomRotation()
    {
        Vector3 rotationDirection = new Vector3(_randomRotateDirection.x, 0, _randomRotateDirection.y).normalized *
                                    (randomRotationSpeed * Time.deltaTime);

        _transform.rotation *= Quaternion.Euler(rotationDirection);
    }

    private void AddExtraRotation()
    {
        Vector2 angles = GetNegativeAllowedXZRotation();
        Vector3 rotationDirection =
            new Vector3(angles.x, 0, angles.y).normalized * (drunkRotationSpeed * Time.deltaTime);

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
        ) * (maxMoveStrength * Time.deltaTime);

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

    #endregion
}