using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameFinish gameFinish;
    [SerializeField] private PlayerController player;
    [SerializeField] private Collectible collectible;

    void Start()
    {
        Init();
        Collectible.Collected += OnCollectibleCollected;

        if (gameFinish != null) gameFinish.GameFinished += OnGameFinish;
        if (player != null) player.PlayerDeath += OnPlayerDeath;
    }

    private void Init()
    {
        Debug.Log("Game Init");
        player.ResetPlayer();
        
        if (gameFinish != null)
            gameFinish.gameObject.SetActive(false);
        
        collectible.Init();
    }

    private void OnCollectibleCollected()
    {
        gameFinish.gameObject.SetActive(true);
    }
    
    private void OnPlayerDeath()
    {
        Debug.Log("Player Death");
        Invoke(nameof(Init), 3);

        //TODO: game over screen
    }

    private void OnGameFinish()
    {
        Debug.Log("Game Finished");
        player.GetWasted();

        //TODO: victory screen

        Invoke(nameof(Init), 4);
    }

    private void OnDestroy()
    {
        Collectible.Collected -= OnCollectibleCollected;
        if (gameFinish != null) gameFinish.GameFinished -= OnGameFinish;
        if (player != null) player.PlayerDeath -= OnPlayerDeath;
    }
}