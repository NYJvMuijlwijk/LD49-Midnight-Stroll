using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class ForwardMovement : MonoBehaviour
{
    [SerializeField] private Vector2 moveSpeedRange = new Vector2(9, 14);

    private Transform _transform;
    private Rigidbody _rigidbody;
    private Vector3 _velocity;

    private void Awake()
    {
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _velocity = _transform.TransformDirection(Vector3.forward) * Random.Range(moveSpeedRange.x, moveSpeedRange.y);
    }

    private void Update()
    {
        _rigidbody.MovePosition(_transform.position + _velocity * Time.deltaTime);
    }
}