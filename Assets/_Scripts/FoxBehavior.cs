using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxBehavior : MonoBehaviour
{
    //Fox: wanders randomly until it sees or smells a chicken/bunny
    //Follows scent/sight to chase prey and eats it
    //If it sees water and is thirsty, moves to and drinks it
    //Remembers where it last saw multiple chickens and returns to that area if not following something

    //States: wandering, chasing prey, moving to water, returning to remembered chicken area

    //higher number objectives are more important
    public enum Objective { Wander=0, FollowScent=1, StalkPrey=2, ChasePrey=3 }
    public Objective curObj = Objective.Wander;

    Movement movementScript;

    bool isPredator = false;
    bool isPrey = false;



    // Start is called before the first frame update
    void Start()
    {
        movementScript = GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ChooseObj()
    {

    }
}
