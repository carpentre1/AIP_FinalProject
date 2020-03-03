using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public float movementSpeed = 1.0f;//how fast the animal moves by default
    public float walkMultiplier = 1.0f;
    public float waterMultiplier = .5f;
    public float sprintMultiplier = 2.0f;
    bool isSprinting = false;
    public int inWater = 0;//any value over 0 is in water
    float totalMoveSpeed = 1.0f;

    public float environmentalSpeedReduction = 1.0f;
    public List<float> speedModifiers = new List<float>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CalculateSpeed();
        InputMove();
    }

    #region Manual Movement

    void CalculateSpeed()
    {
        totalMoveSpeed = 1;

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
        if (Input.GetKey(KeyCode.LeftShift)) isSprinting = true;
        else isSprinting = false;
    }

    void InputMove()
    {
        if(Input.GetKey("w"))
        {
            transform.Translate(Vector3.up * Time.deltaTime * movementSpeed * totalMoveSpeed);
        }
        if (Input.GetKey("s"))
        {
            transform.Translate(Vector3.down * Time.deltaTime * movementSpeed * totalMoveSpeed);
        }
        if (Input.GetKey("a"))
        {
            transform.Translate(Vector3.left * Time.deltaTime * movementSpeed * totalMoveSpeed);
        }
        if (Input.GetKey("d"))
        {
            transform.Translate(Vector3.right * Time.deltaTime * movementSpeed * totalMoveSpeed);
        }
    }

    #endregion
}
