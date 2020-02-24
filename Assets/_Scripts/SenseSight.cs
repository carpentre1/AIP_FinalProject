using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SenseSight : MonoBehaviour
{

    public float sightRadius = 5.0f;//how far the animal can see in a circle around it
    public float distractedModifier = .7f;//how much sight is reduced when distracted (eating, drinking)
    float currentDistractionModifier = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
