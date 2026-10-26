using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Collections.Generic;

/** TODO:
 * 3. ADD EVENT FOR STARTING CHASE SEQUENCE
 * 6. ADD RESET METHOD (aggression, startleCounter, freeze flags, light, state, spawn position)
 */
[System.Serializable]
public class StateChangedEvent : UnityEvent<GhostAI.State> { }
public class GhostAI : MonoBehaviour
{
    [SerializeField]
    private float timeBetweenPathFinding = 0.2f;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private float fastStalkSpeed = 7.5f;
    [SerializeField]
    private float slowStalkSpeed = 2.5f;
    [SerializeField]
    private float runningAwaySpeed = 20;
    [SerializeField]
    private float runningAwayDistance = 25;
    [SerializeField]
    private float chaseStartedWarning = 3;
    [SerializeField]
    private float chasingSpeed = 10f; //MAKE SURE THIS IS FASTER THAN THE PLAYER
    [SerializeField]
    private float runAwayDurration = 2f;
    [SerializeField]
    private float NonAggressiveStalkingBaseDuration = 25f;
    [SerializeField]
    private float NonAggressiveStalkDistance = 20f;
    [SerializeField]
    private float playerTrailGap = 10f;

    public UnityEvent onPlayerCaught = new UnityEvent();
    public StateChangedEvent onStateChanged = new StateChangedEvent();

    private byte aggressionLevel = 0;
    private byte startleCounter = 0;
    private NavMeshAgent agent;
    private Light light;
    private State currentState = State.NonAggressiveStalking;

    //helpers
    private int stateLifeTime = -1; //how long the state has lived in frames
    private float lastPathfind = 0;
    private State previousState;
    private float timeWhenEnteredState = 0;
    private bool justStartledInChase = false;// Only modified in chasing
    private bool isFrozen = false;
    private float timeWhenStartled;
    private float durationRandomizer = 0;
    private List<Vector3> playerTrail = new List<Vector3>();

    public enum State
    {
        RunningAway, //Staying as far from the player as possible
        NonAggressiveStalking, //Actively moves towards the player but doenst run towards them
        AggresiveStalking, // Actively moves towards the player and chases them
        ChasingStarted, //Give the player time to gain distance
        Chasing, //When aggressed
    }

    public void increaseAgression()
    {
        aggressionLevel += 1;
        Debug.Log("Aggression level: " + aggressionLevel);
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        light = GetComponentInChildren<Light>();

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        onPlayerCaught.AddListener(() =>
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        });
        Debug.Log(light);
    }

    void Update()
    {
        stateLifeTime++;
        recordTrail();
        switch (currentState)
        {
            case State.RunningAway:
                RunningAway();
                break;
            case State.NonAggressiveStalking:
                NonAggressiveStalking();
                break;
            case State.AggresiveStalking:
                AggresiveStalking();
                break;
            case State.ChasingStarted:
                ChasingStarted();
                break;
            case State.Chasing:
                Chasing();
                break;
        }

    }

    /// <summary>
    /// Will cause the ghost to run away when stalking the player and freeze when chasing.
    /// </summary>
    public void Startle()
    {
        if (currentState == State.RunningAway) return;


        if (isStalkingState() && startleCounter < 3)
        {
            switchState(State.RunningAway);
            startleCounter++;
        }
        else if (isStalkingState())
        {
            switchState(State.ChasingStarted);
            startleCounter = 0;
        }
        else if (currentState == State.Chasing && !isFrozen)
        {
            justStartledInChase = true;
            startleCounter++;
        }
    }

    public State getCurrentState()
    {
        return currentState;
    }

    public bool isMoving()
    {
        return agent.velocity.sqrMagnitude > 0.01f;
    }

    private void RunningAway()
    {
        if (JustEnteredState())
        {
            changeSpeed(runningAwaySpeed);
        }

        if ((JustEnteredState() || isPathfindTime()) && distanceFromPlayer() <= runningAwayDistance)
        {
            Vector3 lineToPlayer = playerTransform.position - transform.position;
            Vector3 destination = transform.position + (lineToPlayer.normalized * -runningAwayDistance);
            agent.SetDestination(destination);
        }

        if (aggressionLevel < 3)
        {
            return;
        }

        if (timeSinceLastState() >= runAwayDurration)
            switchState(State.NonAggressiveStalking);
    }

    private void NonAggressiveStalking()
    {
        if (JustEnteredState())
        {
            durationRandomizer = Random.Range(0, 1.25f);
            changeSpeed(slowStalkSpeed);
        }


        if (isPathfindTime())
        {
            agent.SetDestination(getTrail(NonAggressiveStalkDistance));
        }

        if (distanceFromPlayer() <= NonAggressiveStalkDistance)
        {
            changeSpeed(0);
        }
        else
        {
            changeSpeed(slowStalkSpeed);
        }

        float duration;
        if (aggressionLevel <= 5)
        {
            duration = NonAggressiveStalkingBaseDuration;
        }
        else if (aggressionLevel <= 10)
        {
            duration = NonAggressiveStalkingBaseDuration * 0.7f;
        }
        else
        {
            duration = NonAggressiveStalkingBaseDuration * 0.5f;
        }
        duration += durationRandomizer;

        bool tooClose = distanceFromPlayer() <= NonAggressiveStalkDistance * 0.5f;
        if (timeSinceLastState() >= duration || tooClose)
            switchState(State.AggresiveStalking);
    }

    private void AggresiveStalking()
    {
        if (JustEnteredState())
        {
            changeSpeed(fastStalkSpeed);
        }
        targetPlayer();

        checkForJumpscare();
    }

    private void ChasingStarted()
    {
        if (JustEnteredState())
        {
            changeSpeed(0);
            light.enabled = true;
        }

        if (timeSinceLastState() <= chaseStartedWarning) return;
        switchState(State.Chasing);
    }

    private void Chasing()
    {
        if (JustEnteredState()) changeSpeed(chasingSpeed);
        targetPlayer();

        if (startleCounter >= 4)
        {
            switchState(State.RunningAway);
            startleCounter = 0;
            light.enabled = false;
            justStartledInChase = false;
            isFrozen = false;
            return;
        }

        if (justStartledInChase)
        {
            justStartledInChase = false;
            isFrozen = true;
            timeWhenStartled = Time.time;
            changeSpeed(0);
        }

        if (isFrozen && Time.time - timeWhenStartled >= 3)
        {
            changeSpeed(chasingSpeed);
            isFrozen = false;
        }

        checkForJumpscare();
    }

    private void recordTrail()
    {
        if (playerTrail.Count == 0 || Vector3.Distance(playerTrail[playerTrail.Count - 1], playerTransform.position) >= playerTrailGap)
        {
            playerTrail.Add(playerTransform.position);
            if (playerTrail.Count >= 30) playerTrail.RemoveAt(0);
        }
    }

    private Vector3 getTrail(float gap)
    {
        if (playerTrail.Count == 0) return playerTransform.position;
        int back = Mathf.RoundToInt(gap / playerTrailGap);
        int trail = Mathf.Max(0, playerTrail.Count - 1 - back);
        return playerTrail[trail];
    }

    //HELPERS
    private bool isPathfindTime()
    {
        if (Time.time - lastPathfind < timeBetweenPathFinding && !JustEnteredState())
            return false;

        lastPathfind = Time.time;
        return true;
    }

    private float distanceFromPlayer()
    {
        return (transform.position - playerTransform.position).magnitude;
    }

    private void switchState(State newState)
    {
        stateLifeTime = -1;
        previousState = currentState;
        currentState = newState;
        timeWhenEnteredState = Time.time;
        Debug.Log(currentState);
        onStateChanged.Invoke(newState);
    }

    private float timeSinceLastState()
    {
        return Time.time - timeWhenEnteredState;
    }


    //wont change speed if it already is set to this
    //because changing speed when you dont need to every frame messes with the acceleration
    private void changeSpeed(float speed)
    {
        if (agent.speed == speed) return;
        agent.speed = speed;
    }

    private bool JustEnteredState()
    {
        if (stateLifeTime <= 0)
            return true;
        else
            return false;
    }

    private void checkForJumpscare()
    {
        if (distanceFromPlayer() > 1) return;

        Debug.Log("PLAYER CAUGHT!");
        onPlayerCaught.Invoke();
    }

    private bool isStalkingState()
    {
        if (currentState == State.AggresiveStalking || currentState == State.NonAggressiveStalking)
            return true;
        else
            return false;
    }

    private void targetPlayer()
    {
        if (isPathfindTime())
        {
            agent.SetDestination(playerTransform.position);
        }
    }
}
