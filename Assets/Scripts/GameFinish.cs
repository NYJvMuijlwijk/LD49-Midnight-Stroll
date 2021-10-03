using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GameFinish : MonoBehaviour
{
    public event Action GameFinished;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        PlayerController player = other.GetComponent<PlayerController>();
            
        if(!player.wasted)
            GameFinished?.Invoke();
    }
}
