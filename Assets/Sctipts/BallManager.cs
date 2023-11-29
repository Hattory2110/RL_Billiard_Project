using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class BallManager : MonoBehaviour, IObserver<BallEvent>
{
    [SerializeField]private Balls black;
    [SerializeField] private List<Balls> strapped;
    [SerializeField] private List<Balls> full;

    [SerializeField] private List<Balls> allBalls;

    [SerializeField] private List<Hole> allHoles;
    [SerializeField] private BallEventManager eventManager;

    [SerializeField] private int counter_Full_Scored = 0;
    [SerializeField] private int counter_Stripped_Scored = 0;

    private void Start()
    {
        
        allBalls = new List<Balls>();

        allBalls.Add(black);
        allBalls.AddRange(strapped);
        allBalls.AddRange(full);

        eventManager.Subscribe(this);


    }

    private void OnDestroy()
    {
        eventManager.Unsubscribe(this);
    }
    public List<Balls> GetAllBalls() { return allBalls; }

    public List<Hole> GetAllHoles() { return allHoles; }

    public int GetCounterFull_Scored() {  return counter_Full_Scored; }

    public int GetCounterStripped_Scored() { return counter_Stripped_Scored;}

    public int GetTeamCounter(Team team)
    {
        if (team == Team.Full)
        {
            return GetCounterFull_Scored();
        }
        else if (team == Team.Striped)
        {
            return GetCounterStripped_Scored();
        }
        return 0;
    }

    private void Reset()
    {
        counter_Full_Scored = 0;
        counter_Stripped_Scored = 0;
    }

    public void OnNotify(object sender, BallEvent eventArgs)
    {
        if (eventArgs.team==Team.Full)
        {
            counter_Full_Scored += 1;
        }
        else if (eventArgs.team == Team.Striped)
        {
            counter_Stripped_Scored += 1;
        }
        else
        {
            Reset();
        }
    }

    public void OnNotifyHit(object sender, BallEvent eventArgs)
    {
        return;
    }
}
