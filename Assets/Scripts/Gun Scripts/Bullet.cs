using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public LayerMask collisionMask;

    public float speed = 10f;
    public float destroyAfter = 2f;
    public float damage = 1f;
    public float skinWidth = 0.1f;

    public Color trailColor;

    public TrailRenderer trailRenderer;

    private void Start()
    {
        Destroy(gameObject, destroyAfter);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
        if (initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0], transform.position);
        }

        trailRenderer.material.SetColor("_TintColor", trailColor);
    }

    private void Update()
    {
        CheckCollisions(speed * Time.deltaTime);
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void CheckCollisions(float moveDistance)
    {
        Ray _ray = new Ray(transform.position, transform.forward);
        RaycastHit _hit;

        if (Physics.Raycast(_ray, out _hit, moveDistance + skinWidth,
            collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(_hit.collider, _hit.point);
        }
    }

    void OnHitObject(Collider c, Vector3 hitPoint)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();

        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage, hitPoint, transform.forward);
        }

        Destroy(gameObject);
    }

} // class
