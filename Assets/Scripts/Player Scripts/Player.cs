using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : LivingEntity
{
    public float moveSpeed = 4.5f;

    private PlayerController controller;
    private Camera viewCamera;

    private Vector3 moveInput, moveVelocity;

    private Ray lookRay;

    private GunController gunController;

    private Plane groundPlane;

    public AudioClip playerDeathClip;

    public Crosshair crosshair;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        viewCamera = Camera.main;

        gunController = GetComponent<GunController>();
    }

    protected override void Start()
    {
        base.Start();

        groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight());
    }

    private void Update()
    {
        HandleMovement();

        lookRay = viewCamera.ScreenPointToRay(Input.mousePosition);

        float rayDistance;

        if (groundPlane.Raycast(lookRay, out rayDistance))
        {
            Vector3 point = lookRay.GetPoint(rayDistance);
            controller.LookAt(point);

            crosshair.transform.position = point;
            crosshair.DetectTargets(lookRay);

            gunController.Aim(point);
        }

        if (Input.GetMouseButton(0))
        {
            gunController.OnTriggerHold();
        }

        if (Input.GetMouseButtonUp(0))
        {
            gunController.OnTriggerRelease();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            gunController.Reload();
        }

        if (Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(1))
        {
            gunController.ChangeGun();
        }

        if (transform.position.y < -10)
            TakeDamage(health);

    }

    public override void Die()
    {
        AudioManager.instance.PlaySound(playerDeathClip, transform.position);
        base.Die();
    }

    void HandleMovement()
    {
        moveInput = new Vector3(Input.GetAxisRaw(TagManager.HORIZONTAL_AXIS), 0f,
                    Input.GetAxisRaw(TagManager.VERTICAL_AXIS));

        moveVelocity = moveInput.normalized * moveSpeed;

        controller.Move(moveVelocity);
    }

}
