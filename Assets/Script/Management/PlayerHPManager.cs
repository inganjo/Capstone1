using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerHPManager : MonoBehaviourPun
{
    [SerializeField] private GameObject tombstonePrefab;
    [SerializeField] private GameObject geometry;
    [SerializeField] private GameObject main;
    private bool isDead=false;

    [Header("Player Stats")]
    public float maxHealth = 100f; // 최대 체력
    public float maxHealHP = 50f;  // 자동 회복 상한
    public float currentHealth;   // 현재 체력
    private float timer;          // 데미지 받은 후 경과 시간
    public float healDelay = 5f;  // 회복 시작까지 대기 시간
    public float healRate = 2f;   // 초당 회복량

    private void Start()
    {
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
        timer += Time.deltaTime;
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        if (photonView.IsMine) 
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
        if (isDead) return;

        isDead=true;

        if (main != null)
        {
            main.SetActive(false); 
        }
        if (geometry != null)
        {
            geometry.SetActive(false);
        }

        if (tombstonePrefab != null)
        {
            Quaternion currentRotation = transform.rotation;
            Quaternion tombstoneRotation = Quaternion.Euler(-90, currentRotation.eulerAngles.y, currentRotation.eulerAngles.z);
            Collider capsuleCollider = GetComponent<CapsuleCollider>();
            CharacterController cc= GetComponent<CharacterController>();
            if (capsuleCollider != null)
            {
                cc.enabled=false;
                capsuleCollider.enabled = false;
            }
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
                    Vector3 forceDirection = Vector3.up * 5f;
                    rb.AddForce(forceDirection, ForceMode.Impulse);
                }
            }
            StartCoroutine(DestroyAfterDelay(3f));
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (PhotonNetwork.IsConnected)
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject); // 네트워크 객체 제거
            }
        }
        else
        {
            Destroy(gameObject); // 로컬 객체 제거
        }
    }

    [PunRPC]
    public void HealOverTime()
    {
        if (photonView.IsMine)
        {
            currentHealth += healRate * Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealHP); 
        }
    }
}
