using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    public static BallManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    [SerializeField]private Balls black;
    [SerializeField] private List<Balls> strapped;
    [SerializeField] private List<Balls> full;

    [SerializeField] private List<Balls> allBalls;

    private void Start()
    {
        allBalls = new List<Balls>();

        allBalls.Add(black);
        allBalls.AddRange(strapped);
        allBalls.AddRange(full);
    }

    public List<Balls> GetAllBalls() { return allBalls; }
}
