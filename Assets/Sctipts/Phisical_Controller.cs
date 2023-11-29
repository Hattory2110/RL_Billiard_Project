using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;
using System.Collections.Generic;

public class Phisical_Controller : Agent, IObserver<BallEvent>
{
    [SerializeField] Rigidbody rb;

    [SerializeField] private BallEventManager ballEventManager;
    [SerializeField] private BallManager ballManager;
    [SerializeField] float innerVelocityThreshold = 0.001f;
    [SerializeField] int baseReward = 50;
    [SerializeField] int basePenalty = 50;

    private Boolean canShoot;

    public Team team = Team.none;

    private void Start()
    {
        ballEventManager.Subscribe(this);
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
        ballEventManager.Unsubscribe(this);
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

        sensor.AddObservation(this.canShoot);

        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);

        List<Balls> allBalls = ballManager.GetAllBalls();
        List<Hole> allHoles = ballManager.GetAllHoles();
        
        foreach (Balls ball in allBalls)
        {
            Transform Otransform =  ball.GetTransform();

            sensor.AddObservation(Otransform.localPosition.x);
            sensor.AddObservation(Otransform.localPosition.z);
        }

        foreach (Hole hole in allHoles)
        {
            Transform Otransform = hole.GetPosition();
            
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
        ballEventManager.Reset();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Extract horizontal and vertical directions, and force from actions
        float horizontalDirection = actions.ContinuousActions[0];
        float verticalDirection = actions.ContinuousActions[1];
        float force = actions.ContinuousActions[2];

        if (!canShoot)
        {
            AddReward(-0.2f);
        }

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

    private float GetRewardNumber()
    {
        if (team == Team.Full)
        {
            int count = 7 - ballManager.GetCounterFull_Scored();
            float reward = baseReward / count + 1;
            return reward;
        }
        else if (team == Team.Striped)
        {
            int count = 7 - ballManager.GetCounterStripped_Scored();
            float reward = baseReward / count + 1;
            return reward;
        }

        return 0;
    }

    private float GetPenaltyNumber(Team oppositeTeam)
    {
        if (oppositeTeam == Team.Full)
        {   
            int count = 7 - ballManager.GetCounterFull_Scored();
            float reward = basePenalty / count + 1;
            return -reward;
        }
        else if (oppositeTeam == Team.Striped)
        {
            int count = 7 - ballManager.GetCounterStripped_Scored();
            float reward = basePenalty / count + 1;
            return -reward;
        }

        return 0;
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
            float reward = GetRewardNumber();
            AddReward(reward);
            if (ballManager.GetTeamCounter(e.team) == 7)
            {
                Debug.Log("Game Won.");
                EndEpisode();
            }
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
            float penalty = GetPenaltyNumber(e.team);
            AddReward(penalty);
            if (ballManager.GetTeamCounter(e.team) == 7)
            {
                Debug.Log("Game Lost.");
                EndEpisode();
            }
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
