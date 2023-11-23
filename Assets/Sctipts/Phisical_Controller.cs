using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;
using System.Collections.Generic;

public class Phisical_Controller : Agent, IObserver<BallEvent>
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float innerVelocityThreshold = 0.001f;

    private Boolean canShoot;

    public Team team = Team.none;

    private void Start()
    {
        BallEventManager.Instance.Subscribe(this);
    }

    

    private void Update()
    {
        if (rb.velocity.magnitude < innerVelocityThreshold)
        {
            canShoot= true;
        }

      
    }

    private void OnDestroy()
    {
        BallEventManager.Instance.Unsubscribe(this);
    }

    private void SetTeam(object sender, BallEvent e)
    {
        if ((e.team == Team.none) || (e.team == Team.Both))
        {
            return;
        }
        else if (team == Team.Striped || team == Team.Full){
            return;
        }
        this.team = e.team;
    }

    public override void CollectObservations(VectorSensor sensor)
    {



        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);

        List<Balls> allBalls = BallManager.instance.GetAllBalls();
        
        foreach (Balls ball in allBalls)
        {
            Transform Otransform =  ball.GetTransform();

            sensor.AddObservation(Otransform.localPosition.x);
            sensor.AddObservation(Otransform.localPosition.z);
        }

    }

    // Implement the Heuristic method for manual control during training
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxisRaw("Horizontal");
        continuousActionsOut[1] = Input.GetAxisRaw("Vertical");

    }

    public override void OnEpisodeBegin()
    {
        // Reset relevant variables at the beginning of each episode
        team = Team.none;
        BallEventManager.Instance.Reset();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Extract horizontal and vertical directions, and force from actions
        float horizontalDirection = actions.ContinuousActions[0];
        float verticalDirection = actions.ContinuousActions[1];
        float force = actions.ContinuousActions[2];

        Vector2 shootingDirection = new Vector2(horizontalDirection, verticalDirection);
        ShootBall(shootingDirection, force);
    }


    void ShootBall(Vector2 direction, float force)
    {
        if (!canShoot)
        {
            return;
        }
       
       
            // Apply the forces to shoot the ball
            rb.AddForce(new Vector3(direction.x * force * 1000, 0, direction.y * force * 1000));
            canShoot = false;
        

    }
    

    private void OnCollisionEnter(Collision collision)
    {
        IHitable hitable = collision.gameObject.GetComponent<IHitable>();
        if (hitable != null)
        {
            //a little reward for hitting any balls
            AddReward(+0.01f);
        }
    }

    public void OnNotify(object sender, BallEvent e)
    {
        if (team == Team.none)
        {
            // Set the team of the Ai for the first ball that was hit down first
            SetTeam(sender, e);
        }

        if (e.team == this.team)
        {
            // Add reward for hitting the right balls
            AddReward(10f);
        }
        else if (e.team == Team.Both)
        {
            // Add penalty for hitting down the Black or White balls
            AddReward(-10f);
            Debug.Log("Episode end");
            EndEpisode();
        }
        else
        {
            // Add penalty for hitting the wrong team's balls
            AddReward(-1f);
        }
    }


    public void OnNotifyHit(object sender, BallEvent e)
    {
        
        if (e.team == this.team)
        {
            // Add reward for hitting the right balls
            AddReward(1f);
        }
        else if (e.team == Team.Both)
        {
            // Add penalty for hitting down the Black or White balls
            AddReward(-5f);
        }
        else
        {
            // Add penalty for hitting the wrong team's balls
            AddReward(-1f);
        }
    }
}
