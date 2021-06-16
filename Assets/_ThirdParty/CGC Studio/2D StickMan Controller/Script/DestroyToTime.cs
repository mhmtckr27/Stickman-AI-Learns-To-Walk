using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyToTime : MonoBehaviour
{    public float DestroyTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       Destroy(gameObject,DestroyTime); 
    }
}
