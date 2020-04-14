using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SenseSight : MonoBehaviour
{

    float sightRadius = 2.0f;//how far the animal can see in a circle around it
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

    public List<GameObject> SeenObjects()
    {
        List<GameObject> seenObjects = new List<GameObject>();
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(this.gameObject.transform.position, sightRadius);
        int i = 0;
        while (i < hitColliders.Length)
        {
            //only see objects that are relevant to gameplay
            if (hitColliders[i].gameObject.tag == "bush" || hitColliders[i].gameObject.tag == "predator" || hitColliders[i].gameObject.tag == "water"
                || hitColliders[i].gameObject.tag == "chicken" || hitColliders[i].gameObject.tag == "bunny")
            {
                //if (!GameManager.Instance.occupiedObjects.Contains(hitColliders[i].gameObject))
                //{
                //    seenObjects.Add(hitColliders[i].gameObject);
                //}
                seenObjects.Add(hitColliders[i].gameObject);
            }
            i++;
        }
        string s = "";
        foreach(GameObject g in seenObjects)
        {
            s += g.name + ", ";
        }
        Debug.Log("seen by " + gameObject + ": " + s);
        return seenObjects;
    }
}
