﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
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
            other.GetComponent<Movement>().inWater++;
        }
        if(other.GetComponent<Scent>())
        {
            Debug.Log(other.gameObject.name + " water: " + other.GetComponent<Scent>().scentStrength + " becomes " + other.GetComponent<Scent>().defaultScentStrength);
            other.GetComponent<Scent>().scentStrength = other.GetComponent<Scent>().defaultScentStrength;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Movement>())
        {
            other.GetComponent<Movement>().inWater--;
        }
    }
}
