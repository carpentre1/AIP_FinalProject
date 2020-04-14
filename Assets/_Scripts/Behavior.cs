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
    public enum Objective { Wander = 0, FollowingScent = 1, Stalking = 2, Chasing = 3, Eating = 4, Drinking = 5, Escaping = 6 }
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
    bool alive = true;

    float timeUntilNextThought = 2f;
    bool busyThinking = false;

    List<GameObject> detectedObjects = new List<GameObject>();
    public GameObject objective;



    // Start is called before the first frame update
    void Start()
    {
        movementScript = GetComponent<Movement>();
        smellScript = GetComponent<SenseSmell>();
        sightScript = GetComponent<SenseSight>();
        hearingScript = GetComponent<SenseHearing>();
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
            LookForFood();
        }
        //if nothing productive to do was found, wander
        if(!busyThinking)
        {
            UpdateObjective(Objective.Wander, this.gameObject);
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

    void UseAllSenses()
    {
        List<GameObject> seenObjects = sightScript.SeenObjects();
        detectedObjects = seenObjects;

        //add hearing and smelling to the list of detected objects
    }

    IEnumerator ThoughtCoroutine()
    {
        busyThinking = true;
        yield return new WaitForSeconds(timeUntilNextThought);
        busyThinking = false;
    }

    void Die(bool removeCorpse)
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


        foreach(GameObject g in detectedObjects)
        {
            if(g.tag == "bush" && isPrey)
            {
                //go to bush and eat
                UpdateObjective(Objective.Eating, g);
                objective = g;
                return;
            }
            else if((g.tag == "bunny" || g.tag == "chicken") && isPredator)
            {
                //chase/stalk the prey
                UpdateObjective(Objective.Stalking, g);
                return;
            }
        }
        
    }

    void UpdateObjective(Objective newObj, GameObject obj)
    {
        Debug.Log(gameObject.name + " new obj: " + newObj.ToString());
        curObj = newObj;
        objective = obj;
        StartCoroutine(ThoughtCoroutine());

        behaviorText.text = curObj.ToString();
    }
}
