using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementInhibitor : MonoBehaviour
{
    public float speedModifier = .5f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Movement>())
        {
            other.GetComponent<Movement>().speedModifiers.Add(speedModifier);
        }
    }
}
