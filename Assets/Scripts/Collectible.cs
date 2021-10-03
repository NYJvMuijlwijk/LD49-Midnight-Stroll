using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectible : MonoBehaviour
{
    [SerializeField] private GameObject collectible;
    [SerializeField] private float spawnHeight = 1f;
    [SerializeField] private float verticalMovement = 0.25f;
    [SerializeField] private float verticalSpeed = 1.5f;
    [SerializeField] private float rotationSpeed = 25f;

    private GameObject _collectible;
    private Transform _transform;

    public static event Action Collected;

    private void Start()
    {
        _transform = transform;

        Init();
    }

    public void Init()
    {
        if (_collectible != null) Destroy(_collectible);

        _collectible = Instantiate(
            collectible,
            _transform
        );

        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (_collectible == null) return;

        _collectible.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        float height = spawnHeight + Mathf.Sin(Time.time * verticalSpeed) * verticalMovement;
        _collectible.transform.position = height * Vector3.up + _transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_collectible == null) return;

        Debug.Log("Collect");
        Collected?.Invoke();
        Destroy(_collectible);

        gameObject.SetActive(false);
    }
}