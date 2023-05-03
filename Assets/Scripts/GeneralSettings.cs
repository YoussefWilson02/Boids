using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GeneralSettings : ScriptableObject
{
    [Header("Speed Settings")]
    public float minSpeed = 1;
    public float maxSpeed = 3;
    public float maxSteerForce = 3;

    [Header("Core Behaviour")]
    public float viewRadius = 2.5f;
    public float repulsionRadius = 1;
    public float attractionWeight = 1;
    public float orientationWeight = 1;
    public float repulsionWeight = 1;
    [Header("Predators & Prey")]
    [Header("Hunting")]
    public float huntingRadius = 1;
    public float huntingWeight = 1;
    [Header("Fleeing")]
    public float fleeingRadius = 1;
    public float fleeingWeight = 1;
    public float targetWeight = 1;

    [Header("Collisions")]
    public LayerMask obstacleMask;
    public float boundsRadius = 0.27f;
    public float collisionAvoidRadius = 10;
    public float collisionAvoidWeight = 5;

    [Header("Food Seeking")]
    public LayerMask foodMask;
    public float foodRadius;
    public float foodWeight;
}
