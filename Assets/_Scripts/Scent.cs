using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scent : MonoBehaviour
{
    //Any object with this script holds a scent that it rubs off onto terrain
    //Any terrain with a scent will 'announce' that scent to very nearby animals
    //Animal scents grow in strength over time, and are reduced if the animal goes through water
    //Going through water
    //Scents have a strength and animals have a scent detection level
    //If detection level > strength, the animal detects that scent

    enum ScentType { Chicken, Bunny, Fox, Foliage}
    ScentType scent = ScentType.Foliage;

    public float scentStrength = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
