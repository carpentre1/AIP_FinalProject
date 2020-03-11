using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SenseSmell : MonoBehaviour
{
    public float smellPower = 1f;//how effective the creature is at picking up subtle smells

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReceiveScent(Scent scent, float intensity)
    {
        if(intensity < smellPower)
        {
            return;//couldn't smell it!
        }
        else
        {
            Debug.Log(this.gameObject + " smelled " + scent.gameObject);
        }
    }

    void DetermineScentResponse(Scent scent, float intensity)
    {
        //list of smells?
        //if following a good scent, go towards strongest smell or start with weakest?
    }
}
