using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class Boid : MonoBehaviour
{

    public GeneralSettings settings;

    //current state of boid
    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public Vector3 forward;
    Vector3 velocity;

    //to-update boid values
    Vector3 acceleration;
    [HideInInspector]
    public Vector3 flockOrientation;
    [HideInInspector]
    public Vector3 flockRepulsion;
    [HideInInspector]
    public Vector3 flockCentre;
    [HideInInspector]
    public Vector3 huntingTarget;
    [HideInInspector]
    public Vector3 fleeingTarget;
    [HideInInspector]
    public int flockmateCount;
    [HideInInspector]
    public float trophic;

    //cached data
    Material material;
    Transform cachedTransform;
    Transform target;

    void Awake()
    {
        material = transform.GetComponentInChildren<MeshRenderer>().material;
        cachedTransform = transform;
    }

    public void Initialise(GeneralSettings settings, Transform target)
    {
        this.target = target;
        this.settings = settings;

        position = cachedTransform.position;
        forward = cachedTransform.forward;

        float startVelocity = (settings.minSpeed + settings.maxSpeed) / 2;

        velocity = transform.forward * startVelocity;
    }

    public void SetColour(Color colour)
    {
        if (material != null)
        {
            material.color = colour;
        }
    }

    public void UpdateBoid()
    {
        acceleration = Vector3.zero;

        if (target != null)
        {
            Vector3 offsetFromTarget = target.position - position;
            acceleration = SteerTowards(offsetFromTarget) * settings.targetWeight;
        }
        if (flockmateCount != 0)
        {
            Vector3 flockmateCentreAvg = flockCentre / flockmateCount;
            Vector3 offsetFlockmateCentre = flockmateCentreAvg - position;

            var attractionForce = SteerTowards(offsetFlockmateCentre) * settings.attractionWeight;
            var orientationForce = SteerTowards(flockOrientation) * settings.orientationWeight;
            var repulsionForce = SteerTowards(flockRepulsion) * settings.repulsionWeight;
            var fleeingForce = SteerTowards(fleeingTarget) * settings.fleeingWeight;
            var huntingForce = SteerTowards(huntingTarget) * settings.huntingWeight;

            acceleration += attractionForce;
            acceleration += orientationForce;
            acceleration += repulsionForce;
            acceleration += fleeingForce;
            acceleration += huntingForce;
        }
        if (IsHeadingForCollision())
        {
            Vector3 collisionAvoidDirection = ObstacleRays();
            Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDirection) * settings.collisionAvoidWeight;
            acceleration += collisionAvoidForce;
        }
        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 direction = velocity / speed;
        speed = Mathf.Clamp(speed, settings.minSpeed, settings.maxSpeed);
        velocity = direction * speed;

        cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = direction;
        position = cachedTransform.position;
        forward = direction;
    }

    bool IsHeadingForCollision()
    {
        RaycastHit hit;
        if (Physics.SphereCast(position, settings.boundsRadius, forward, out hit, settings.collisionAvoidRadius, settings.obstacleMask))
        {
            return true;
        }
        else { }
        return false;
    }

    Vector3 ObstacleRays()
    {
        Vector3[] viewDirections = ViewDirections.viewDirections;

        for (int i = 0; i < viewDirections.Length; i++)
        {
            Vector3 direction = cachedTransform.TransformDirection(viewDirections[i]);
            Ray ray = new Ray(position, direction);
            if (!Physics.SphereCast(ray, settings.boundsRadius, settings.collisionAvoidRadius, settings.obstacleMask))
            {
                return direction;
            }
        }
        return forward;
    }

    Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 v = vector.normalized * settings.maxSpeed - velocity;
        return Vector3.ClampMagnitude(v, settings.maxSteerForce);
    }
}
