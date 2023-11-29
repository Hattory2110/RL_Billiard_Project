using System;
using System.Collections.Generic;
using UnityEngine;



public class BallEventManager : MonoBehaviour
{
    private List<IObserver<BallEvent>> observers = new List<IObserver<BallEvent>>();

    [SerializeField]private  List<Balls> balls = new List<Balls>();
    
    public void Reset()
    {
        foreach (var ball in balls)
        {
            ball.Reset();
        }
    }

    public void Subscribe(IObserver<BallEvent> observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void Unsubscribe(IObserver<BallEvent> observer)
    {
        observers.Remove(observer);
    }

    public void Notify(BallEvent ballEvent)
    {
        foreach (var observer in observers)
        {
            observer.OnNotify(this, ballEvent);
        }
    }

    public void NotifyHit(BallEvent ballEvent)
    {
        foreach (var observer in observers)
        {
            observer.OnNotify(this, ballEvent);
        }
    }

    public void TriggerBallEvent(Team team)
    {
        BallEvent ballEvent = new BallEvent(team);
        Notify(ballEvent);
    }

    public void TriggerBallEventHit(Team team)
    {
        BallEvent ballEvent = new BallEvent(team);
        NotifyHit(ballEvent);
    }
}
