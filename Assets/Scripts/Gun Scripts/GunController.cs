using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;

    public Gun[] allGuns;
    public int gunIndex;

    private Gun _equippedGun;

    private void Start()
    {
        EquipGun(allGuns[gunIndex]);
    }

    public void EquipGun(Gun gunToEquip)
    {
        if (_equippedGun != null)
            Destroy(_equippedGun.gameObject);

        _equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        _equippedGun.transform.parent = weaponHold;
    }

    public void OnTriggerHold()
    {
        if (_equippedGun != null)
            _equippedGun.OnTriggerHold();
    }

    public void OnTriggerRelease()
    {
        if (_equippedGun != null)
            _equippedGun.OnTriggerRelease();
    }

    public float GunHeight()
    {
        return weaponHold.position.y;
    }

    public void Aim(Vector3 aimPoint)
    {
        if (_equippedGun)
            _equippedGun.Aim(aimPoint);
    }

    public void Reload()
    {
        if (_equippedGun)
            _equippedGun.Reload();
    }

    public void ChangeGun()
    {
        gunIndex++;

        if (gunIndex == allGuns.Length)
            gunIndex = 0;

        EquipGun(allGuns[gunIndex]);
    }

} // class
