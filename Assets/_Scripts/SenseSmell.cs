using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SenseSmell : MonoBehaviour
{
    public float smellPower = 1f;//how effective the creature is at picking up subtle smells
    Behavior myBehavior;

    // Start is called before the first frame update
    void Start()
    {
        myBehavior = GetComponent<Behavior>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReceiveScent(Scent scent)
    {
        if(scent.scentStrength < smellPower)
        {
            return;//couldn't smell it!
        }
        else
        {
            DetermineScentResponse(scent);
        }
    }

    void DetermineScentResponse(Scent scent)
    {
        if(scent.gameObject.GetComponent<Behavior>())
        {
            Behavior behavior = scent.gameObject.GetComponent<Behavior>();
        }
        if(scent.scent == Scent.ScentType.Fox && myBehavior.isPrey)
        {
            myBehavior.UpdateObjective(Behavior.Objective.Escaping, scent.gameObject);
            Debug.Log("fleeing due to smell");
        }
        GetComponent<Behavior>().smelledObjects.Add(scent.gameObject);
    }

    public void ForgetSmell(Scent scent)
    {
        GetComponent<Behavior>().smelledObjects.Remove(scent.gameObject);
    }
}
