using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyScript : MonoBehaviour
{
    //�G�l���M�[��
    int energyAmount = 0;

    void Start()
    {
        IncEnergy();
    }

    void Update()
    {
        
    }

    //��莞�Ԃ��ƂɃG�l���M�[�ʂ𑝂₷
    void IncEnergy()
    {
        energyAmount++;
        //Debug.Log(energyAmount);

        Invoke("IncEnergy", 1);
    }

    //�G�l���M�[����֐�
    public void UseEnergy(int useAmount)//�֐����Ăԑ��ŏ���ʂ��w��
    {
        energyAmount -= useAmount;
    }
}
