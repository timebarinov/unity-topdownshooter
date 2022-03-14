using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public float rotateSpeed = 50f;
    public LayerMask targetMask;

    private Color highlightDotColor = Color.red;
    private Color originalDotColor;

    public SpriteRenderer dot;

    private void Start()
    {
        Cursor.visible = false;
        originalDotColor = dot.color;
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
    }

    public void DetectTargets(Ray ray)
    {
        if (Physics.Raycast(ray, 100, targetMask))
            dot.color = highlightDotColor;
        else
            dot.color = originalDotColor;
    }
}
