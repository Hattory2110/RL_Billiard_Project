using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Hole : MonoBehaviour, ISink
{

    //counter for how many balls were hit down the hole for tensorboard
    [SerializeField] private BallEventManager ballEventManager;
    int count = 0;
    
    public void OnSink(Balls ball)
    {
        ballEventManager.TriggerBallEvent(ball.team);

        if(ball.TryGetComponent(out Phisical_Controller controller))
        {
            return;
        }
        if(ball.team == Team.Both)
        {
            return;
        }
        ball.gameObject.SetActive(false);
        count++;
    }

    public Transform GetPosition()
    {
        return this.transform;
    }

}
