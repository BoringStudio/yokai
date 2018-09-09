using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementTest : MonoBehaviour
{
    public GameObject target;

    NavMeshAgent _agent;
    Animator _animator;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }
    
	void Update ()
    {
        if (target.activeInHierarchy)
        {
            _agent.SetDestination(target.transform.position);
        }
        
        _animator.SetFloat("Speed", _agent.velocity.magnitude);
	}
}
