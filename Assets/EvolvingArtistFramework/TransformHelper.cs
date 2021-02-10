using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[ExecuteAlways]
public class TransformHelper : MonoBehaviour
{
    // Start is called before the first frame update

    Vector3[] points = new Vector3[4];
    public Vector3 A;
    public Vector3 B;
    public Vector3 C;

    Matrix4x4 transformation;

    private void Start()
    {
        points[0] = new Vector3(0, 0,0)*5;
        points[1] = new Vector3(0, 1,0)*5;
        points[2] = new Vector3(1, 1,0)*5;
        points[3] = new Vector3(1, 0,0)*5;

        transformation = Matrix4x4.Scale(Vector3.one);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawLine( points[i], transformation.MultiplyVector(points[(i + 1) % 4]));
        }
    }
}
