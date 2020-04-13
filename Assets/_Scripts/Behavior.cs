using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    //1 is full, 0 causes death
    public float hunger = .7f;
    public float thirst = .7f;

    float timeUntilNextThought = 2f;
    bool busyThinking = false;



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
        if(!busyThinking)
        {
            //try to do each of these things, stopping at the first one that provided a new objective
            CheckForDanger();
            LookForFood();
        }
        //if nothing productive to do was found, wander
        if(!busyThinking)
        {
            UpdateObjective(Objective.Wander);
        }
    }

    IEnumerator ThoughtCoroutine()
    {
        busyThinking = true;
        yield return new WaitForSeconds(timeUntilNextThought);
        busyThinking = false;
    }

    void CheckForDanger()
    {
        if (busyThinking) return;

        List<GameObject> seenObjects = sightScript.SeenObjects();
        if (seenObjects.Count == 0) { return; }

        foreach (GameObject g in seenObjects)
        {
            if (g.tag == "predator" && isPrey)
            {
                UpdateObjective(Objective.Escaping);
                return;
            }
        }
    }

    void LookForFood()
    {
        if (busyThinking) return;

        List<GameObject> seenObjects = sightScript.SeenObjects();
        if(seenObjects.Count == 0) { return; }

        foreach(GameObject g in seenObjects)
        {
            if(g.tag == "bush" && isPrey)
            {
                //go to bush and eat
                UpdateObjective(Objective.Eating);
                return;
            }
            else if((g.tag == "bunny" || g.tag == "chicken") && isPredator)
            {
                //chase/stalk the prey
                UpdateObjective(Objective.Stalking);
                return;
            }
        }
        
    }

    void UpdateObjective(Objective newObj)
    {
        Debug.Log(gameObject.name + " new obj: " + newObj.ToString());
        curObj = newObj;
        StartCoroutine(ThoughtCoroutine());

        behaviorText.text = curObj.ToString();
    }
}
