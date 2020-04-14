using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Rigidbody2D rb;

    public float movementSpeed = 1.0f;//how fast the animal moves by default
    public float walkMultiplier = 1.0f;
    public float waterMultiplier = .5f;
    public float sprintMultiplier = 2.0f;
    public float wanderMultiplier = .5f;
    bool isSprinting = false;
    public int inWater = 0;//any value over 0 is in water
    float totalMoveSpeed = 1.0f;

    const float MOVEMENT_NORMALIZATION = 1f;//adjusts the overall speed of movement to accomodate the size of objects relative to the world

    bool isPlayerControlled = false;

    public float environmentalSpeedReduction = 1.0f;
    public List<float> speedModifiers = new List<float>();

    float wanderTime = 0f;
    Vector2 wanderDirection;
    bool isPerformingAction = false;
    bool isWandering = false;

    GameObject target;
    Vector3 directionGoal;
    Behavior behaviorScript;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        behaviorScript = GetComponent<Behavior>();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateSpeed();
        WanderRandomly();
        InputMove();
    }

    private void FixedUpdate()
    {
        if(behaviorScript.curObj == Behavior.Objective.Wander)
        {
            //rb.AddForce(wanderDirection * Time.deltaTime * movementSpeed * totalMoveSpeed * wanderMultiplier);
            transform.Translate((wanderDirection * Time.deltaTime * movementSpeed * totalMoveSpeed * wanderMultiplier));
            wanderTime -= Time.deltaTime;
            if(wanderTime <= 0) { isWandering = false; }
        }

        if(behaviorScript.curObj == Behavior.Objective.Eating)
        {
            if (Vector2.Distance(this.gameObject.transform.position, behaviorScript.objective.transform.position) > .03f)
            {
                transform.position = Vector2.MoveTowards(transform.position, behaviorScript.objective.transform.position, totalMoveSpeed * Time.deltaTime);
            }
            else
            {
                behaviorScript.Nibble();
            }
        }

        if (behaviorScript.curObj == Behavior.Objective.Drinking)
        {
            if (Vector2.Distance(this.gameObject.transform.position, behaviorScript.objective.transform.position) > .03f)
            {
                transform.position = Vector2.MoveTowards(transform.position, behaviorScript.objective.transform.position, totalMoveSpeed * Time.deltaTime);
            }
            else
            {
                behaviorScript.Drink();
            }
        }

        if(behaviorScript.curObj == Behavior.Objective.Escaping)
        {
            if (Vector2.Distance(this.gameObject.transform.position, behaviorScript.objective.transform.position) < 3f)
            {
                Vector2 fleeDirection = transform.position - behaviorScript.objective.transform.position;
                transform.Translate(fleeDirection * Time.deltaTime * totalMoveSpeed);
            }
            else
            {
                //
            }
        }

        if (rb.velocity.magnitude > totalMoveSpeed)
        {
            rb.velocity = rb.velocity.normalized * totalMoveSpeed;
        }
    }

    void WanderRandomly()
    {
        if(isPlayerControlled || isWandering || isPerformingAction)
        {
            return;
        }
        wanderDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        wanderTime = Random.Range(1f, 5f);
        isWandering = true;
    }

    #region Manual Movement

    void CalculateSpeed()
    {
        totalMoveSpeed = 1 * MOVEMENT_NORMALIZATION;

        if (isSprinting) totalMoveSpeed *= sprintMultiplier;
        else totalMoveSpeed *= walkMultiplier;

        if (inWater > 0) totalMoveSpeed *= waterMultiplier;

        if (speedModifiers.Count > 0)
        {
            foreach(float f in speedModifiers)
            {
                totalMoveSpeed *= f;
            }
        }
    }

    void InputSprint()
    {
        if(!isPlayerControlled) { return; }
        if (Input.GetKey(KeyCode.LeftShift)) isSprinting = true;
        else isSprinting = false;
    }

    void InputMove()
    {
        if (!isPlayerControlled) { return; }
        if (Input.GetKey("w"))
        {
            rb.AddForce(Vector3.up * Time.deltaTime * movementSpeed * totalMoveSpeed);
        }
        if (Input.GetKey("s"))
        {
            rb.AddForce(Vector3.down * Time.deltaTime * movementSpeed * totalMoveSpeed);
        }
        if (Input.GetKey("a"))
        {
            rb.AddForce(Vector3.left * Time.deltaTime * movementSpeed * totalMoveSpeed);
        }
        if (Input.GetKey("d"))
        {
            rb.AddForce(Vector3.right * Time.deltaTime * movementSpeed * totalMoveSpeed);
        }
    }

    #endregion
}
