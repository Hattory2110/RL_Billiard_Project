using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balls : MonoBehaviour, IHitable
{
    [SerializeField] private BallEventManager ballEventManager;
    public Team team;

    //counter counts how many times the ball has been hit for tensor board
    int counter = 0;

    public void OnHit(Team team)
    {
        ballEventManager.TriggerBallEventHit(team);
        counter++;
    }

    public Transform GetTransform() { return this.transform; }

    public Vector3 originalPosition;

    private void Start() {
        originalPosition = new Vector3(transform.position.x,transform.position.y,transform.position.z);
    }
    public void Reset()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.position = originalPosition;
        gameObject.SetActive(true);
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.TryGetComponent(out ISink sink)){
            
            sink.OnSink(this);
        }

        if(other.gameObject.TryGetComponent(out IHitable hitable))
        {
            if(team == Team.none)
            {
                    hitable.OnHit(other.gameObject.GetComponent<Balls>().team);
            }
        }
    }
}

public class BallEvent : EventArgs
{
    public Team team;

    public BallEvent(Team team)
    {
        this.team = team;
    }
}

public enum Team{
    Full,
    Striped,
    none,
    Both    
} 
