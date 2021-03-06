﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    public enum AnimalType {None, Bunny, Chicken, Fox};

    public List<GameObject> waypoints = new List<GameObject>();

    //prevent animals from picking/crowding around the same bush or water
    public List<GameObject> occupiedObjects = new List<GameObject>();
}
