using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    const int threadGroupSize = 1024;
    public ComputeShader compute;

    Boid[] boids;

    public struct BoidData
    {
        public Vector3 position;
        public Vector3 direction;

        public Vector3 flockCentre;
        public Vector3 flockOrientation;
        public Vector3 flockRepulsion;
        public Vector3 huntingTarget;
        public Vector3 fleeingTarget;

        public float viewRadius;
        public float repulsionRadius;
        public float huntingRadius;
        public float fleeingRadius;

        public float trophic;

        public int flockMateCount;

        public static int Size
        {
            get
            {
                return (sizeof(float) * 3 * 7) + (sizeof(float) * 5) + sizeof(int);
            }
        }
    }

    void Start()
    {
        boids = FindObjectsOfType<Boid>();
    }

    private void Update()
    {
        if (boids != null) 
        {
            int boidCount = boids.Length;
            var boidData = new BoidData[boidCount];

            for (int i = 0; i < boidCount; i++)
            {
                boidData[i].position = boids[i].position;
                boidData[i].direction = boids[i].forward;

                boidData[i].viewRadius = boids[i].settings.viewRadius;
                boidData[i].repulsionRadius = boids[i].settings.repulsionRadius;
                boidData[i].huntingRadius = boids[i].settings.huntingRadius;
                boidData[i].fleeingRadius = boids[i].settings.fleeingRadius;

                boidData[i].trophic = boids[i].trophic;
            }

            ComputeBuffer boidBuffer = new ComputeBuffer(boidCount, BoidData.Size);
            boidBuffer.SetData(boidData);

            compute.SetBuffer(0, "boids", boidBuffer);
            compute.SetInt("boidsCount", boids.Length);

            int threadGroups = Mathf.CeilToInt(boidCount / (float)threadGroupSize);
            compute.Dispatch(0, threadGroups, 1, 1);

            boidBuffer.GetData(boidData);

            for (int i = 0; i < boids.Length; i++)
            {   
                boids[i].flockCentre = boidData[i].flockCentre;
                boids[i].flockOrientation = boidData[i].flockOrientation;
                boids[i].flockRepulsion = boidData[i].flockRepulsion;
                boids[i].huntingTarget = boidData[i].huntingTarget;
                boids[i].fleeingTarget = boidData[i].fleeingTarget;

                boids[i].flockmateCount = boidData[i].flockMateCount;

                boids[i].UpdateBoid();
            }
            boidBuffer.Release();
        }
    }
}
