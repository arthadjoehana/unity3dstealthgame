using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    NavMeshAgent agent;

    public Transform player;
    public GameObject alert;
    public Animator _animator;
    public enum STATE
    {
        IDLE, PATROL, CHASE
    }
    public STATE currState = STATE.IDLE;

    public List<GameObject> patrolPoints = new List<GameObject>();
    int curPatrolIndex = -1;

    float visionDistance = 10.0f;
    float visionAngle = 90.0f;

    public bool canSeePlayer;

    private void Start()
    {
        canSeePlayer = false;
    }


    void Awake()
    {
        agent = this.GetComponent<NavMeshAgent>();

        if (patrolPoints.Count != 0)
            ChangeState(STATE.PATROL);
            alert.SetActive(false);
    }

    void Update()
    {
        switch (currState)
        {
            case STATE.IDLE:
                _animator.SetFloat("Speed", 0);
                if (canSeePlayer)
                {
                    ChangeState(STATE.CHASE);
                    alert.SetActive(true);
                }
                else 
                {
                    ChangeState(STATE.PATROL);
                    alert.SetActive(false);
                }
                break;
            case STATE.PATROL:
                _animator.SetFloat("Speed", 1);
                if (agent.remainingDistance < 1)
                {
                    if (curPatrolIndex >= patrolPoints.Count - 1)
                        curPatrolIndex = 0;
                    else
                        curPatrolIndex++;
                    agent.SetDestination(patrolPoints[curPatrolIndex].transform.position);
                }

                if (canSeePlayer)
                {
                    ChangeState(STATE.CHASE);
                    alert.SetActive(true);
                }
                break;
            case STATE.CHASE:
                if (canSeePlayer)
                {
                    _animator.SetFloat("Speed", 2);
                    agent.SetDestination(player.position);
                }
                else
                {
                    ChangeState(STATE.PATROL);
                    alert.SetActive(false);
                }
                break;
        }


        DetectPlayer();
    }

    public void DetectPlayer()
    {
        Vector3 direction = player.position - transform.position;
        if (Physics.Raycast(transform.position, direction, out var hit, direction.magnitude, LayerMask.GetMask("obstacle")))
        {
            Debug.Log(hit.collider.name);
            Debug.DrawRay(transform.position, direction, Color.red, 0.5f);
            canSeePlayer = false;
            return;
        }
        Debug.DrawRay(transform.position, direction, Color.green, 0.5f);

        float angle = Vector3.Angle(direction, transform.forward);

        if (direction.magnitude < visionDistance && angle < visionAngle)
        {
            canSeePlayer = true;
            
        }
        else
        {
            canSeePlayer = false;
        }
        return;
    }


    

    public void ChangeState(STATE newState)
    {
        switch (currState)
        {
            case STATE.IDLE:

                break;
            case STATE.PATROL:

                break;
            case STATE.CHASE:

                break;
        }
        switch (newState)
        {
            case STATE.IDLE:

                break;
            case STATE.PATROL:
                agent.speed = 2;
                agent.isStopped = false;

                float lastDist = Mathf.Infinity;
                for (int i = 0; i < patrolPoints.Count; i++)
                {
                    GameObject thisWP = patrolPoints[i];
                    float distance = Vector3.Distance(transform.position, thisWP.transform.position);
                    if (distance < lastDist)
                    {
                        curPatrolIndex = i - 1;
                        lastDist = distance;
                    }
                }

                break;
            case STATE.CHASE:
                agent.speed = 3;
                agent.isStopped = false;

                break;
        }

        currState = newState;
    }

   
}
