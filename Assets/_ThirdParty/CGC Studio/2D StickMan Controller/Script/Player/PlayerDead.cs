using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PlayerDead : MonoBehaviour
{
    
    
    public string DeadTag;
    public bool isDead;
    public UnityEvent OnPlayerDead;
    private void Update() {
    if(isDead)
    {
       OnPlayerDead.Invoke();
    }
    
    }
}
