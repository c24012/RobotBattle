using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MacineGunScript : LiveGunOriginScript
{

    void Start()
    {
        //MacineGun用にパラメーターを設定
        bulletAmount = 30;
        bulletSpeed = 15;
        fireRate = 6;
        fireRange = 450;
        reloadTime = 2;
        fireEnergyReq = 2;
        reloadEnergyReq = bulletAmount * fireEnergyReq / 3;

        Preparation();
    }

    void Update()
    {

        



        if (Input.GetButton("Fire1"))
        {
            StartCoroutine(Fire());
        }
    }
}
