using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class EnemyZombie : MonoBehaviour
{
    public enum AIState
    {
        Chasing,
        Attack
    }

    public UnityAction onDamaged;

    public Transform target;
    public AIState aiState;

    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private Health _health;
    private bool _inAttackAnimation;


    // Start is called before the first frame update
    void Start()
    {
        // Get Component
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();

        // Subscript event
        _health.onDamaged += OnDamaged;

        // Creep Up zombie
        StartCoroutine(CreepUp());
    }

    IEnumerator CreepUp()
    {
        yield return new WaitForSeconds(2f);
        // Init walk & think
        Walk();
        StartCoroutine(Think());
    }

    IEnumerator Think()
    {
        while (true)
        {
            if (aiState == AIState.Chasing)
            {
                _navMeshAgent.SetDestination(target.position);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator OnCompleteAttackAnimation()
    {
        yield return new WaitForSeconds(2f);
        _inAttackAnimation = false;
    }

    void OnDamaged(float damage, GameObject damageSource)
    {
        _animator.Play("damege");
    }

    void OnTriggerEnter(Collider other)
    {
        Attack();
    }

    void OnTriggerStay(Collider other)
    {
        Attack();
    }

    void OnTriggerExit(Collider other)
    {
        Walk();
    }

    void Walk()
    {
        // Set chasing state
        aiState = AIState.Chasing;
        // Set walk id random
        _animator.SetInteger("walkId", Random.Range(0, 5));
        // Enable walk layer animation
        _animator.SetLayerWeight(1, 1);
    }

    void Attack()
    {
        // Set attack state
        aiState = AIState.Attack;

        if (_inAttackAnimation == false)
        {
            // Disable walk layer animation
            _animator.SetLayerWeight(1, 0);
            // Play animation
            _inAttackAnimation = true;
            _animator.Play("atack0" + Random.Range(1, 3));
            StartCoroutine(OnCompleteAttackAnimation());
        }
    }
}