using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerHPManager : MonoBehaviourPun
{
    [SerializeField] private GameObject tombstonePrefab;
    [Header("Player Stats")]
    public int maxHealth = 100; // 최대 체력
    public int maxHealHP = 50;
    public int currentHealth;   // 현재 체력

    private void Start()
    {
        // 체력을 최대 체력으로 초기화
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (currentHealth<=0)
            Die();
    }
    // 체력을 감소시키는 함수
    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (photonView.IsMine) // 본인의 객체인지 확인
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    // 사망 처리 함수
    private void Die()
    {

    if (tombstonePrefab != null)
        {
            Quaternion currentRotation = transform.rotation;
            Quaternion tombstoneRotation = Quaternion.Euler(-90, currentRotation.eulerAngles.y, currentRotation.eulerAngles.z);
            // 현재 플레이어의 위치에 비석 생성
            Instantiate(tombstonePrefab, transform.position, tombstoneRotation);
        }

        // 네트워크 연결 상태에 따라 객체 파괴
        if (PhotonNetwork.IsConnected)
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject); // 네트워크 객체 파괴
            }
        }
        else
        {
            Destroy(gameObject); // 로컬 객체 파괴
        }
    }

    // 체력을 회복시키는 함수
    [PunRPC]
    public void Heal(int amount)
    {
        if (photonView.IsMine) // 본인의 객체인지 확인
        {
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealHP); 
        }
    }
}
