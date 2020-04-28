using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scent : MonoBehaviour
{
    //Any object with this script holds a scent that it rubs off onto terrain
    //Animals will periodically 'smell' their surroundings, picking up the strongest nearby scent
    //Animal scents grow in strength over time, and are reduced if the animal goes through water
    //Going through water
    //Scents have a strength and animals have a scent detection level
    //If detection level > strength, the animal detects that scent

    public enum ScentType { Chicken, Bunny, Fox, Foliage, Bush}
    public ScentType scent = ScentType.Foliage;

    public float scentStrength = 1f;
    public float scentRadius = 5;
    const float maxScentStrength = 4f;
    public GameObject scentProvider;//the thing that put this scent here

    float decayRate = .02f;//default rate is .1

    public ScentType defaultScent = ScentType.Foliage;
    public float defaultScentStrength = 1f;

    float decayWaitTime = .2f;//default wait time is 1
    bool decayCycleBusy = false;
    float announceWaitTime = 2f;
    bool announceCycleBusy = false;

    public bool isTurf = false;
    public GameObject northNeighbor;
    public GameObject southNeighbor;
    public GameObject eastNeighbor;
    public GameObject westNeighbor;
    public List<GameObject> neighbors = new List<GameObject>();

    public GameObject scentTracker;

    // Start is called before the first frame update
    void Start()
    {
        EstablishAllNeighbors();
    }

    // Update is called once per frame
    void Update()
    {
        DecayScent();
    }

    public GameObject StrongestNeighbor()//find a nearby tile with a stronger scent than this one
    {
        float strongestScent = scentStrength;
        GameObject strongestScentObject = null;

        foreach(GameObject g in neighbors)
        {
            if(!g) { continue; }
            if (!g.GetComponent<Scent>()) { continue; }
            Scent neighborScent = g.GetComponent<Scent>();
            if(neighborScent.scent == scent)//we're looking for the same type of scent only
            {
                if(neighborScent.scentStrength > strongestScent)
                {
                    strongestScent = neighborScent.scentStrength;
                    strongestScentObject = g;
                }
            }
        }
        return strongestScentObject;
    }

    void EstablishAllNeighbors()
    {
        northNeighbor = EstablishNeighbor(0, .25f);//.25 is one grid block
        southNeighbor = EstablishNeighbor(0, -.25f);
        eastNeighbor = EstablishNeighbor(.25f, 0);
        westNeighbor = EstablishNeighbor(-.25f, 0);

        neighbors.Add(northNeighbor);
        neighbors.Add(eastNeighbor);
        neighbors.Add(southNeighbor);
        neighbors.Add(westNeighbor);
    }

    GameObject EstablishNeighbor(float offsetX, float offsetY)
    {
        Vector3 neighborPos = new Vector3(gameObject.transform.position.x + offsetX, gameObject.transform.position.y + offsetY);
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(neighborPos, .01f);
        foreach(Collider2D col in hitColliders)
        {
            if (col.gameObject.GetComponent<Scent>())
            {
                if (col.gameObject.GetComponent<Scent>().isTurf)
                {
                    return col.gameObject;
                }
                else continue;
            }
            else continue;
        }
        return null;
    }

    void DebugUpdateScentTracker()
    {
        if(scent == ScentType.Bush || scent == ScentType.Foliage)
        {
            scentTracker.transform.localScale = new Vector3(0, 0, 0);
            return;//don't show scent tracker for default grass scents
        }

        SpriteRenderer sprite = scentTracker.GetComponent<SpriteRenderer>();
        if(scent == ScentType.Bunny || scent == ScentType.Chicken)
        {
            sprite.color = Color.yellow;
        }
        if(scent == ScentType.Fox)
        {
            sprite.color = Color.red;
        }
        if(scent == ScentType.Foliage || scent == ScentType.Bush)
        {
            sprite.color = Color.green;
        }
        scentTracker.transform.localScale = new Vector3(scentStrength / 5f, scentStrength / 5f, scentStrength / 5f);
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
            scentStrength -= decayRate;
            if(scentStrength < defaultScentStrength)//if it falls below the minimum value...
            {
                //revert the scent back to its default scent type
                scent = defaultScent;
            }
        }
        else if(scent == defaultScent)
        {
            if(scent == ScentType.Foliage || scent == ScentType.Bush)
            {
                //
            }
            //scent slowly builds up or down to a normalized value
            else if(scentStrength < maxScentStrength)
            {
                scentStrength += .1f;
            }
            else if (scentStrength > maxScentStrength)
            {
                scentStrength -= .1f;
            }
        }
        if(isTurf) DebugUpdateScentTracker();
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
        if(other.GetComponent<SenseSmell>() && isTurf)
        {
            SenseSmell smell = other.GetComponent<SenseSmell>();
            smell.ReceiveScent(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.GetComponent<SenseSmell>())
        {
            collision.GetComponent<SenseSmell>().ForgetSmell(this);
        }
    }
    /// <summary>
    /// Adds a scent to the object or increases that scent's potency
    /// Occurs each time an animal moves onto or performs an action on that object
    /// </summary>
    public void AddScent(Scent animalScent)
    {
        if(GetComponent<Behavior>())
        {
            return;
        }
        if(scent != animalScent.scent)
        {
            scent = animalScent.scent;
            scentProvider = animalScent.gameObject;
            scentStrength = animalScent.scentStrength;
            //Debug.Log(this.gameObject + " scent changed to " + animalScent.scent);
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
        yield return new WaitForSeconds(decayWaitTime + Random.Range(0.01f, 0.1f));
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
            smell.ReceiveScent(this);
        }
    }
}
