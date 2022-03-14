using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode { Auto, Burst, Single };
    public FireMode fireMode;

    public Transform[] bulletSpawn;
    public Bullet bullet;

    public float msBetweenShots = 100f;
    public float bulletVelocity = 35f;

    public int burstCount = 3;
    public int bulletPerMag = 10;

    public float reloadTime = 0.3f;

    [Header("Effects")]
    public Transform shell;
    public Transform shellEjector;
    private MuzzleFlash muzzleFlash;
    private float _nextShotTime;
    public AudioClip shootAudio;
    public AudioClip reloadAudio;

    [Header("Recoil")]
    public Vector2 kickMinMax = new Vector2(0.05f, 0.2f);
    public Vector2 recoilAngleMinMax = new Vector2(3f, 5f);
    public float recoilMoveSettleTime = 0.1f;
    public float recoilRotationSettleTime = 0.1f;

    private bool triggerReleasedSinceLastShot;
    private int shotsRemainingBurst;
    private int bulletsRemainingInMag;
    private bool isReloading;

    private Vector3 recoilSmoothDampVelocity;

    private float recoilAngle;
    private float recoilRotSmoothDampVel;

    public void Awake()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shotsRemainingBurst = burstCount;
        bulletsRemainingInMag = bulletPerMag;
    }

    private void LateUpdate()
    {
        transform.localPosition =
            Vector3.SmoothDamp(transform.localPosition,
            Vector3.zero, ref recoilSmoothDampVelocity, recoilRotationSettleTime);

        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0,
            ref recoilRotSmoothDampVel, recoilRotationSettleTime);

        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

        if (!isReloading && bulletsRemainingInMag == 0)
        {
            Reload();
        }

    }

    void Shoot()
    {
        if (!isReloading && Time.time > _nextShotTime && bulletsRemainingInMag > 0)
        {
            if (fireMode == FireMode.Burst)
            {
                if (shotsRemainingBurst == 0)
                    return;
                else
                    shotsRemainingBurst--;
            }
            else if (fireMode == FireMode.Single)
            {
                if (!triggerReleasedSinceLastShot)
                    return;
            }

            for (int i = 0; i < bulletSpawn.Length; i++)
            {
                if (bulletsRemainingInMag == 0)
                    break;

                bulletsRemainingInMag--;

                _nextShotTime = Time.time + msBetweenShots / 1000f;

                Bullet newBullet =
                    Instantiate(bullet, bulletSpawn[i].position,
                    bulletSpawn[i].rotation) as Bullet;

                newBullet.SetSpeed(bulletVelocity);
            }

            Instantiate(shell, shellEjector.position, shellEjector.rotation);
            muzzleFlash.Activate();

            transform.localPosition = Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);

            AudioManager.instance.PlaySound(shootAudio, transform.position);
        }
    }

    public void Reload()
    {
        if (bulletsRemainingInMag != bulletPerMag && !isReloading)
        {
            StartCoroutine(AnimateReload());
            AudioManager.instance.PlaySound(reloadAudio, transform.position);
        }
    }

    IEnumerator AnimateReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(0.2f);

        float reloadSpeed = 1 / reloadTime;
        float percent = 0;

        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 30f;

        while (percent <= 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0f, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;

            yield return null;
        }

        isReloading = false;
        bulletsRemainingInMag = bulletPerMag;
    }

    public void Aim(Vector3 aimPoint)
    {
        if (!isReloading)
            transform.LookAt(aimPoint);
    }

    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainingBurst = burstCount;
    }

} // class
