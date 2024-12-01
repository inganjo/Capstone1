using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerHPManager : MonoBehaviourPun
{
    [SerializeField] private GameObject tombstonePrefab;
    [Header("Player Stats")]
    public float maxHealth = 100f; // 최대 체력
    public float maxHealHP = 50f;
    public float currentHealth;   // 현재 체력
    private float timer;          // 데미지 받은 후 경과 시간
    public float healDelay = 5f;  // 회복 시작까지 대기 시간
    public float healRate = 2f;   // 초당 회복량

    private void Start()
    {
        // 체력을 최대 체력으로 초기화
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (currentHealth<=0) {
            Die();
        }

        if (currentHealth < maxHealHP && timer >= healDelay)
        {
            HealOverTime();
        }

        // 타이머 증가
        timer += Time.deltaTime;
    }
    // 체력을 감소시키는 함수
    [PunRPC]
    public void TakeDamage(float damage)
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
    public void Die()
    {
        if (tombstonePrefab != null)
        {
            Quaternion currentRotation = transform.rotation;
            Quaternion tombstoneRotation = Quaternion.Euler(-90, currentRotation.eulerAngles.y, currentRotation.eulerAngles.z);

            // 비석 생성
            GameObject tombstone = null;
            if (PhotonNetwork.IsConnected)
            {
                tombstone = PhotonNetwork.Instantiate(tombstonePrefab.name, transform.position, tombstoneRotation);
            }
            else
            {
                tombstone = Instantiate(tombstonePrefab, transform.position, tombstoneRotation);
            }

            // 비석의 Rigidbody에 힘 가하기
            if (tombstone != null)
            {
                Rigidbody rb = tombstone.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 forceDirection = Vector3.up * 5f; // Z축 방향으로 힘 (10f는 힘의 크기)
                    rb.AddForce(forceDirection, ForceMode.Impulse);
                }
            }
        }
        else
        {
            Debug.LogWarning("[HPManager] 비석 프리팹이 설정되지 않았습니다.");
        }

        // 네트워크 객체 파괴
        if (PhotonNetwork.IsConnected)
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 체력을 회복시키는 함수
    [PunRPC]
    public void HealOverTime()
    {
        if (photonView.IsMine) // 본인의 객체인지 확인
        {
            currentHealth += healRate * Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealHP); // 체력 범위 제한
        }
    }
}
