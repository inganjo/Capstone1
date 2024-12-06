using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;
public class PlayerHPManager : MonoBehaviourPun
{
    [SerializeField] private GameObject tombstonePrefab;
    [SerializeField] private GameObject geometry;
    [SerializeField] private GameObject main;
    [SerializeField] private Image damageOverlay;
    private bool isDead=false;

    [Header("Player Stats")]
    public float maxHealth = 100f; // 최대 체력
    public float maxHealHP = 50f;  // 자동 회복 상한
    public float currentHealth;   // 현재 체력
    private float timer;          // 데미지 받은 후 경과 시간
    public float healDelay = 5f;  // 회복 시작까지 대기 시간
    public float healRate = 2f;   // 초당 회복량

    public float Firedamage = 20.0f;

    public float SmokeDamage = 5.0f;

    [Header("Overlay Settings")]
    public float overlayFadeSpeed = 2f; // 알파값 감소 속도
    private Color overlayColor;

    PhotonHashTable playerSetting = PhotonNetwork.LocalPlayer.CustomProperties;

    

    

    private void Start()
    {
        if(PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("HP"))
        {
            currentHealth = (float)playerSetting["HP"];
        }
        else
        {
            currentHealth = maxHealth;
        }
        
        if (damageOverlay != null)
        {
            overlayColor = damageOverlay.color;
            overlayColor.a = 0;
            damageOverlay.color = overlayColor;
        }

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
        UpdateDamageOverlay();
    }

    public void OnTriggerStay(Collider other){
        if(other.gameObject.tag == "Fire1" || other.gameObject.tag == "Fire2" || other.gameObject.tag == "Fire3")
        {
            TakeDamage(Firedamage);
        }
        else if(other.gameObject.tag =="smoke")
        {
            TakeDamage(SmokeDamage);
        }
        
    }
    // 체력을 감소시키는 함수
    [PunRPC]
    public void TakeDamage(float damage)
    {
        if (photonView.IsMine) 
        {
            currentHealth -= damage * Time.deltaTime;
            playerSetting["HP"] = currentHealth;
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerSetting);
            Debug.Log(currentHealth);
            if (damageOverlay != null)
            {
                overlayColor.a = Mathf.Clamp01(overlayColor.a + 0.5f);
                damageOverlay.color = overlayColor;
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 변경 후 체력 유지 확인
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("HP"))
        {
            
            currentHealth = (int)PhotonNetwork.LocalPlayer.CustomProperties["HP"];
            Debug.Log("OnSceneLoaded: "+ currentHealth);
        }
    }

    private void UpdateDamageOverlay()
    {
        if (damageOverlay != null)
        {
            if (currentHealth <= 30)
            {
                overlayColor.a = 0.5f;
            }
            else
            {
                overlayColor.a = Mathf.Clamp01(overlayColor.a - Time.deltaTime * overlayFadeSpeed); 
            }
            damageOverlay.color = overlayColor;
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
                Debug.Log("비석 생성");
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
                PhotonNetwork.Destroy(gameObject);
                TimerManager.instance.TriggerGameOver();
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
