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

    public enum ScentType { Chicken, Bunny, Fox, Foliage}
    public ScentType scent = ScentType.Foliage;

    public float scentStrength = 1f;
    public float scentRadius = 5;

    public ScentType defaultScent = ScentType.Foliage;
    public float defaultScentStrength = 1f;

    float decayWaitTime = 1f;
    bool decayCycleBusy = false;
    float announceWaitTime = 2f;
    bool announceCycleBusy = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DecayScent();
        //AnnounceScent();
    }

    /// <summary>
    /// Scent naturally decays over time
    /// </summary>
    void DecayScent()
    {
        if(decayCycleBusy)
        {
            return;
        }
        StartCoroutine(DecayCycleCoroutine());
        if (scent != defaultScent)
        {
            scentStrength -= .1f;
            if(scentStrength < defaultScentStrength)//if it falls below the minimum value...
            {
                //revert the scent back to its default scent type
                scent = defaultScent;
            }
        }
        else if(scent == defaultScent)
        {
            if(scent == ScentType.Foliage)
            {
                //
            }
            //scent slowly builds up or down to a normalized value
            else if(scentStrength < defaultScentStrength+5)
            {
                scentStrength += .1f;
            }
            else if (scentStrength > defaultScentStrength+5)
            {
                scentStrength -= .1f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Scent>())
        {
            Scent animalScent = other.GetComponent<Scent>();
            if(animalScent.scentStrength > scentStrength)
            {
                AddScent(animalScent);
            }
        }
    }
    /// <summary>
    /// Adds a scent to the object or increases that scent's potency
    /// Occurs each time an animal moves onto or performs an action on that object
    /// </summary>
    public void AddScent(Scent animalScent)
    {
        if(scent != animalScent.scent)
        {
            scent = animalScent.scent;
            scentStrength = animalScent.scentStrength;
            Debug.Log(this.gameObject + " scent changed to " + animalScent.scent);
        }
        else
        {
            if(scentStrength < animalScent.scentStrength)
            {
                scentStrength = animalScent.scentStrength;
            }
        }
    }

    IEnumerator DecayCycleCoroutine()
    {
        decayCycleBusy = true;
        yield return new WaitForSeconds(decayWaitTime);
        decayCycleBusy = false;
    }
    IEnumerator AnnounceCycleCoroutine()
    {
        announceCycleBusy = true;
        yield return new WaitForSeconds(announceWaitTime);
        announceCycleBusy = false;
    }

    /// <summary>
    /// Intermittently announces the scent? Or only when animals move nearby?
    /// POSSIBLE SOLUTION: keep a list of all scents attached to this tile, only "announce" scents in ontriggerenter
    /// </summary>
    void AnnounceScent()//performed how often? via what?
    {
        if(announceCycleBusy)
        {
            return;
        }
        StartCoroutine(AnnounceCycleCoroutine());
        List<GameObject> animalsInRadius = new List<GameObject>();
        Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.transform.position, scentRadius);
        int i = 0;
        while(i<hitColliders.Length)
        {
            if(hitColliders[i].gameObject.GetComponent<SenseSmell>())
            {
                animalsInRadius.Add(hitColliders[i].gameObject);
            }
            i++;
        }
        foreach(GameObject g in animalsInRadius)
        {
            SenseSmell smell = g.GetComponent<SenseSmell>();
            smell.ReceiveScent(this, scentStrength);
        }
    }
}
