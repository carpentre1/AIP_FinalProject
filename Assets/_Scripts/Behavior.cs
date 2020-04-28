using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Behavior : MonoBehaviour
{
    //Fox: wanders randomly until it sees or smells a chicken/bunny
    //Follows scent/sight to chase prey and eats it
    //If it sees water and is thirsty, moves to and drinks it
    //Remembers where it last saw multiple chickens and returns to that area if not following something

    //States: wandering, chasing prey, moving to water, returning to remembered chicken area

    //higher number objectives are more important
    public enum Objective { Wander = 0, FollowingScent = 1, Stalking = 2, Chasing = 3, Eating = 4, Drinking = 5, Escaping = 6, SearchingFar = 7, Tracking=8 }
    public Objective curObj = Objective.Wander;

    public enum Animal { Chicken, Fox, Bunny}
    public Animal animal = Animal.Chicken;

    Movement movementScript;

    public bool isPredator = false;
    public bool isPrey = false;

    SenseSmell smellScript;
    SenseSight sightScript;
    SenseHearing hearingScript;

    public TextMeshProUGUI behaviorText;
    public Slider sliderHunger;
    public Slider sliderThirst;

    //1 is full, 0 causes death
    public float hunger = .7f;
    public float thirst = .7f;
    public bool alive = true;

    //how much food an object has on it
    float foodValue = 1;

    const float DEFAULT_TIME_UNTIL_NEXT_THOUGHT = 1f;
    float timeUntilNextThought = 1f;
    public bool busyThinking = false;

    [HideInInspector]
    public List<GameObject> detectedObjects = new List<GameObject>();
    public GameObject objective;
    public GameObject farBoundary;
    public List<GameObject> smelledObjects = new List<GameObject>();



    // Start is called before the first frame update
    void Start()
    {
        movementScript = GetComponent<Movement>();
        smellScript = GetComponent<SenseSmell>();
        sightScript = GetComponent<SenseSight>();
        hearingScript = GetComponent<SenseHearing>();

        hunger = Random.Range(.7f, 1f);
        thirst = Random.Range(.7f, 1f);
        if(isPredator)
        {
            hunger = .4f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!alive) return;
        if(!busyThinking)
        {
            UseAllSenses();
            //try to do each of these things, stopping at the first one that provided a new objective

            CheckForDanger();
            LookForWater();
            LookForFood();

            SearchFar();
        }
        //if nothing productive to do was found, wander
        if(!busyThinking)
        {
            UpdateObjective(Objective.Wander);
        }

        hunger = Mathf.Max(hunger - .01f * Time.deltaTime, 0);
        sliderHunger.value = hunger;
        thirst = Mathf.Max(thirst - .02f * Time.deltaTime, 0);
        sliderThirst.value = thirst;
        if(hunger <= 0 || thirst <= 0)
        {
            Die(false);
        }
    }

    public void Nibble(GameObject g)
    {
        timeUntilNextThought += Time.deltaTime;
        if (g.GetComponent<Behavior>())
        {
            Behavior behavior = g.GetComponent<Behavior>();
            behavior.foodValue -= .01f;
            hunger = Mathf.Min(hunger + .16f * Time.deltaTime, 1);
            if (behavior.foodValue <= 0)
            {
                behavior.Die(true);
            }
        }
        else
        {
            hunger = Mathf.Min(hunger + .08f * Time.deltaTime, 1);
        }

        if(hunger >= .95f)
        {
            UpdateObjective(Objective.Wander);
        }
    }
    public void Drink()
    {
        timeUntilNextThought += Time.deltaTime;
        thirst = Mathf.Min(thirst + 1f * Time.deltaTime, 1);

        if (thirst >= .95f)
        {
            UpdateObjective(Objective.Wander);
        }
    }

    void SearchFar()
    {
        if(busyThinking)
        {
            return;
        }
        if(hunger > .4f && thirst > .4f)//only search far when desperate
        {
            return;
        }

        if(!farBoundary)
        {
            UpdateObjective(Objective.SearchingFar, FurthestObject());
        }
        else//if we've already established a far boundary, just go to that. reset the far boundary when we reach it in Movement.cs
        {
            UpdateObjective(Objective.SearchingFar, farBoundary);
        }
    }

    GameObject FurthestObject()
    {
        //search in all 4 directions, go in the direction that's furthest away
        List<GameObject> raycastObjects = new List<GameObject>();
        raycastObjects.Add(RaycastObjectHit(Vector2.up));
        raycastObjects.Add(RaycastObjectHit(Vector2.down));
        raycastObjects.Add(RaycastObjectHit(Vector2.left));
        raycastObjects.Add(RaycastObjectHit(Vector2.right));

        float furthestDist = 0;
        GameObject furthestObj = null;

        foreach (GameObject g in raycastObjects)
        {
            float dist = Vector2.Distance(this.gameObject.transform.position, g.transform.position);
            //Debug.Log(g.name + " dist: " + dist);
            if (dist > furthestDist)
            {
                furthestDist = dist;
                furthestObj = g;
            }
        }
        farBoundary = furthestObj;
        return furthestObj;
    }

    GameObject RaycastObjectHit(Vector2 dir)
    {
        int layerMask = LayerMask.GetMask("boundary");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, layerMask);
        Ray dirRay = new Ray(transform.position, dir);
        if(hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    void UseAllSenses()
    {
        List<GameObject> seenObjects = sightScript.SeenObjects();
        detectedObjects = seenObjects;

        //add hearing and smelling to the list of detected objects

    }

    IEnumerator ThoughtCoroutine()
    {
        busyThinking = true;
        timeUntilNextThought = DEFAULT_TIME_UNTIL_NEXT_THOUGHT;
        yield return new WaitForSeconds(timeUntilNextThought);
        busyThinking = false;
        if (GameManager.Instance.occupiedObjects.Contains(objective))
        {
            GameManager.Instance.occupiedObjects.Remove(objective);
        }
    }

    public void Die(bool removeCorpse)
    {
        alive = false;
        StopAllCoroutines();
        behaviorText.text = "RIP";
        if(removeCorpse)
        {
            Destroy(this.gameObject);
        }
    }

    void CheckForDanger()
    {
        if (busyThinking) return;

        foreach (GameObject g in detectedObjects)
        {
            if (g.tag == "predator" && isPrey)
            {
                UpdateObjective(Objective.Escaping, g);
                return;
            }
        }
    }

    void LookForFood()
    {
        if (busyThinking) return;

        if(hunger > .7f)
        {
            return;
        }

        foreach(GameObject g in detectedObjects)
        {
            if(g.tag == "bush" && isPrey && !GameManager.Instance.occupiedObjects.Contains(g))
            {
                //go to bush and eat
                UpdateObjective(Objective.Eating, g);
                objective = g;
                return;
            }
            else if((g.tag == "bunny" || g.tag == "chicken") && isPredator)
            {
                if(g.GetComponent<Behavior>().alive)
                {
                    //chase/stalk the prey
                    UpdateObjective(Objective.Stalking, g);
                }
                else
                {
                    UpdateObjective(Objective.Eating, g);
                }
                return;
            }
        }
        foreach(GameObject g in smelledObjects)
        {
            Scent scent = g.GetComponent<Scent>();
            if(!scent.scentProvider) { continue; }
            if(scent.scentStrength < 1) { continue; }
            if((scent.scentProvider.tag == "chicken" || scent.scentProvider.tag == "bunny") && isPredator)
            {
                Debug.Log("found a scent to track");
                UpdateObjective(Objective.Tracking, g);
            }
        }
    }

    void LookForWater()
    {
        if (busyThinking) return;

        if(thirst > .7f)
        {
            return;
        }

        GameObject target = null;
        float shortestDistance = 10;

        foreach (GameObject g in detectedObjects)
        {
            if (g.tag == "water" && !GameManager.Instance.occupiedObjects.Contains(g))
            {
                float dist = Vector2.Distance(this.gameObject.transform.position, g.transform.position);
                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    target = g;
                }
            }
        }

        if(target)
        {
            UpdateObjective(Objective.Drinking, target);
        }
    }

    public void UpdateObjective(Objective newObj, GameObject obj=null)
    {
        curObj = newObj;
        objective = obj;
        StopAllCoroutines();
        StartCoroutine(ThoughtCoroutine());

        //update the list of occupied objects
        if(GameManager.Instance.occupiedObjects.Contains(objective))
        {
            GameManager.Instance.occupiedObjects.Remove(objective);
        }
        GameManager.Instance.occupiedObjects.Add(obj);

        behaviorText.text = curObj.ToString();
    }
}
