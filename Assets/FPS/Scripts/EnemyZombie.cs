﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class EnemyZombie : MonoBehaviour
{
    // Public setting
    [Header("Parameters")] public Transform target;
    [Header("Damage")] public float damagePerHit = 1;
    public int damageArea = 2;
    [Header("State")] public AIState aiState;

    public enum AIState
    {
        Chasing,
        Attack
    }

    // Unity event
    public UnityAction onDamaged;

    // Private
    private GameObject _player;
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private Health _health;
    private bool _inAttackAnimation;


    // Start is called before the first frame update
    void Start()
    {
        // Set GameObject/Component
        _player = GameObject.Find("Player");
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();

        // Set size box collider from setting
        GetComponent<BoxCollider>().size = new Vector3(damageArea, damageArea, damageArea);

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

    IEnumerator InflictDamageToPlayer()
    {
        yield return new WaitForSeconds(1f);

        // Check if enemy is range to touch player
        if (gameObject.GetComponent<BoxCollider>().bounds.Contains(_player.transform.position))
        {
            // Inflict damage
            Damageable damageable = _player.GetComponent<Damageable>();
            damageable.InflictDamage(damagePerHit, false, null);
        }
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
            // Inflict player Damage
            StartCoroutine(InflictDamageToPlayer());
            StartCoroutine(OnCompleteAttackAnimation());
        }
    }
}