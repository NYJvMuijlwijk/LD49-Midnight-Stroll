using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private GameObject car;
    [SerializeField] private Vector2 spawnTimingRange = new Vector2(2, 5);
    [SerializeField] private float spawnLifetime = 5f;
    [Space] [SerializeField] public bool active = true;


    private float _timer;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    private void Start()
    {
        SetSpawnTimer();
    }

    private void Update()
    {
        if (!active) return;
        
        _timer -= Time.deltaTime;

        if (!(_timer <= 0)) return;

        Destroy(Instantiate(car, _transform.position, _transform.rotation), spawnLifetime);
        SetSpawnTimer();
    }

    private void SetSpawnTimer()
    {
        _timer = Random.Range(spawnTimingRange.x, spawnTimingRange.y);
    }
}