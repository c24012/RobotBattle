﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

public class LiveGunOriginScript : MonoBehaviour
{
    public int bulletAmount;//装弾数
    public float fireRate;//発射速度
    public float bulletSpeed;//弾速
    public float fireRange;//射程
    public float reloadTime;//リロード時間
    public int fireEnergyReq;//必要エネルギー(1発あたり)
    public int reloadEnergyReq;//必要エネルギー(リロード時)

    public bool isShotGun = false;//ショットガンかどうか

    bool isReload = false;//リロード中かどうか

    bool isRunningFire = false;//発射処理のコルーチンが動いているか

    public GameObject gunObj;//銃本体のオブジェクト(親オブジェクト)

    [SerializeField] GameObject bulletObj;//弾のプレハブオブジェクト
    public List<GameObject> unUsedBulletList = new List<GameObject>();//残弾用リスト
    public List<GameObject> usedBulletList = new List<GameObject>();//使用済みの弾用リスト

    public List<BulletScript> unUsedBulletSCList = new List<BulletScript>();//残弾のスクリプト用リスト
    public List<BulletScript> usedBulletSCList = new List<BulletScript>();//使用済み弾のスクリプト用リスト

    public EnergyScript energySC;//エネルギースクリプト
    public CoreScript coreSC;//コアスクリプト

    public void Preparation()
    {
        gunObj = transform.parent.gameObject;

        //弾プレハブを装弾数×2個分用意
        unUsedBulletList = BulletInst(bulletAmount);
        usedBulletList = BulletInst(bulletAmount * 2);


        //弾のスクリプト取得
        foreach (var list in unUsedBulletList)
        {
            unUsedBulletSCList.Add(list.GetComponent<BulletScript>());
        }
        foreach (var list in usedBulletList)
        {
            usedBulletSCList.Add(list.GetComponent<BulletScript>());
        }
    }



    List<GameObject> BulletInst(int amount)
    {
        var bulletList = new List<GameObject>();
        //弾プレハブを装弾数分用意
        for (int i = 0; i < amount; i++)
        {
            bulletList.Add(
            Instantiate(
                bulletObj,
                transform.position,
                Quaternion.identity
            ));
        }

        return bulletList;
    }


    //球発射関数
    public IEnumerator Fire(GameObject targetObj = null)
    {

        //コルーチン重複防止
        if (isRunningFire || isReload)
        {
            yield break;
        }

        isRunningFire = true;

        //TargetLook(targetObj);

        //残弾があれば撃つ
        if (unUsedBulletList.Count > 0 && energySC.UseEnergy(fireEnergyReq))
        {
            unUsedBulletList[0].transform.position = transform.position;
            unUsedBulletList[0].transform.rotation = transform.rotation;

            //残弾のスクリプトのShot関数呼び出し
            StartCoroutine(unUsedBulletSCList[0].Shot(bulletSpeed, fireRange));

            //撃ち出された弾とそのスクリプトを使用済みリストに追加
            usedBulletList.Add(unUsedBulletList[0]);
            usedBulletSCList.Add(unUsedBulletSCList[0]);


            //残弾リストから削除
            unUsedBulletList.RemoveAt(0);
            unUsedBulletSCList.RemoveAt(0);

            //ショットガンのみ
            if (isShotGun)
            {
                var currentAngle = transform.localEulerAngles;//角度保存

                //9発追加で発射
                for (int i = 0; i < 9; i++)
                {
                    //アングルをランダムに変えてばらけさせる
                    transform.localEulerAngles = new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(-7.5f, 7.5f));
                    unUsedBulletList[0].transform.position = transform.position;
                    unUsedBulletList[0].transform.rotation = transform.rotation;

                    //残弾のスクリプトのShot関数呼び出し
                    StartCoroutine(unUsedBulletSCList[0].Shot(bulletSpeed, fireRange));

                    //撃ち出された弾とそのスクリプトを使用済みリストに追加
                    usedBulletList.Add(unUsedBulletList[0]);
                    usedBulletSCList.Add(unUsedBulletSCList[0]);


                    //残弾リストから削除
                    unUsedBulletList.RemoveAt(0);
                    unUsedBulletSCList.RemoveAt(0);

                    transform.localEulerAngles = currentAngle;//角度を戻す
                }
            }

        }
        else
        {
            //リロード中でなければリロード
            if (!isReload && energySC.UseEnergy(reloadEnergyReq))
            {
                StartCoroutine(Reload());
                isReload = true;
            }
        }
        yield return new WaitForSeconds(1f / fireRate);//発射間隔分待つ

        isRunningFire = false;

    }

    //リロード関数
    IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime);//リロード時間

        //装弾数がマックスになるまでリストに追加
        while (unUsedBulletList.Count < bulletAmount && usedBulletList.Count > 0)
        {
            unUsedBulletList.Add(usedBulletList[0]);
            unUsedBulletSCList.Add(usedBulletSCList[0]);

            usedBulletList.RemoveAt(0);
            usedBulletSCList.RemoveAt(0);
        }

        isReload = false;
    }

    //bool TargetLook(GameObject targetObj)
    //{
    //    if (targetObj != null)
    //    {
            

    //        //現在の銃本体の回転の値を取得
    //        Vector3 movedAngle = new Vector3(gunObj.transform.localEulerAngles.x, gunObj.transform.localEulerAngles.y);
    //        movedAngle.x = movedAngle.x <= 180f ? Mathf.Abs(movedAngle.x) : Mathf.Abs(movedAngle.x - 360f);//X軸
    //        movedAngle.y = movedAngle.y <= 180f ? Mathf.Abs(movedAngle.y) : Mathf.Abs(movedAngle.y - 360f);//Y軸
            

    //        Vector3 targetDir = targetObj.transform.position - gunObj.transform.position;//ターゲットの方向
    //        float angle = Vector3.Angle(targetDir, gunObj.transform.forward);//銃本体とターゲットの方向の差分

    //        Debug.Log($"{angle - movedAngle.x} + {angle - movedAngle.y}");

    //        if (angle - movedAngle.x <= 22.5f && angle - movedAngle.y <= 22.5f)
    //        {
    //            gunObj.transform.LookAt(targetObj.transform, Vector3.forward);
    //            return true;
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}
}
