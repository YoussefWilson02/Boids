using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ViewDirections
{
    const int viewDirectionCount = 300;
    public static readonly Vector3[] viewDirections;

    static ViewDirections()
    {
        viewDirections = new Vector3[viewDirectionCount];

        float goldenRation = (1 + Mathf.Sqrt(5)) / 2;
        float angleIncrement = Mathf.PI * 2 * goldenRation;

        for (int n = 0; n < viewDirectionCount; n++)
        {
            float t = (float)n / viewDirectionCount;
            float altitude = Mathf.Acos(1 - 2 * t); //veritcal angle
            float azimuth = angleIncrement * n;     //horizontal angle

            float i = Mathf.Cos(azimuth) * Mathf.Sin(altitude);
            float j = Mathf.Sin(azimuth) * Mathf.Sin(altitude);
            float k = Mathf.Cos(altitude);

            viewDirections[n] = new Vector3(i, j, k);
        }
    }
}
