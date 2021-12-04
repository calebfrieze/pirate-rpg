using UnityEngine;
using UnityEngine.AI;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;

namespace RPG.Control
{
  public class AIController : MonoBehaviour
  {
    [SerializeField] float chaseDistance = 5f;
    [SerializeField] float suspicionTime = 5f;
    [SerializeField] PatrolPath patrolPath;
    [SerializeField] float waypointTolerance = 1f;
    [SerializeField] float patrolSpeed = 1.8f;
    [SerializeField] float attackSpeed = 2.5f;
    [SerializeField] float waypointDwellTime = 2f;

    Fighter fighter;
    GameObject player;
    Health health;
    Mover mover;
    NavMeshAgent navMeshAgent;

    Vector3 guardPosition;
    float timeSinceLastSawPlayer = Mathf.Infinity;
    float timeSinceArrivedAtWaypoint = Mathf.Infinity;
    int currentWaypointIndex = 0;

    private void Start()
    {
      player = GameObject.FindWithTag("Player");
      fighter = GetComponent<Fighter>();
      health = GetComponent<Health>();
      mover = GetComponent<Mover>();
      navMeshAgent = GetComponent<NavMeshAgent>();

      guardPosition = transform.position;
    }

    private void Update()
    {
      if (health.IsDead()) { return; }
      if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
      {
        AttackBehavior();
      }
      else if (timeSinceLastSawPlayer < suspicionTime)
      {
        SuspicionBehavior();
      }
      else
      {
        PatrolBehavior();
      }
      UpdateTimers();
    }

    private void UpdateTimers()
    {
      timeSinceLastSawPlayer += Time.deltaTime;
      timeSinceArrivedAtWaypoint += Time.deltaTime;
    }

    private void PatrolBehavior()
    {
      Vector3 nextPosition = guardPosition;

      if (patrolPath != null)
      {
        if (AtWayPoint())
        {
          timeSinceArrivedAtWaypoint = 0;
          CycleWaypoint();
        }
        nextPosition = GetCurrentWaypoint();
        if (timeSinceArrivedAtWaypoint > waypointDwellTime)
        {
          mover.StartMoveAction(nextPosition);
        }
      }
    }

    private Vector3 GetCurrentWaypoint()
    {
      return patrolPath.GetWaypoint(currentWaypointIndex);
    }

    private void CycleWaypoint()
    {
      currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
    }

    private bool AtWayPoint()
    {
      float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
      return distanceToWaypoint < waypointTolerance;
    }

    private void SuspicionBehavior()
    {
      navMeshAgent.speed = patrolSpeed;
      GetComponent<ActionScheduler>().CancelCurrentAction();
    }

    private void AttackBehavior()
    {
      timeSinceLastSawPlayer = 0;
      navMeshAgent.speed = attackSpeed;
      fighter.Attack(player);
    }

    private bool InAttackRangeOfPlayer()
    {
      float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
      return distanceToPlayer < chaseDistance;
    }

    // Called by Unity
    private void OnDrawGizmosSelected()
    {
      Gizmos.color = Color.blue;
      DrawChaseDistance();
    }

    private void DrawChaseDistance()
    {
      Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
  }
}
