using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fire : MonoBehaviour
{
    [SerializeField] private ParticleSystem myparticleSystem; // 파티클 시스템
    [SerializeField] private string targetTag = "fire1";

    

    void Start()
    {
        if (myparticleSystem == null)
        {
            myparticleSystem = GetComponent<ParticleSystem>();
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        // 트리거 모듈 활성화
        FireDetect("Fire1");
        
    }

    void OnParticleTrigger()
    {
        // Debug.Log("Trigger Activate");
         ParticleSystem.Particle[] particles = new ParticleSystem.Particle[myparticleSystem.main.maxParticles];
         int numParticles = myparticleSystem.GetParticles(particles);
        //  Debug.Log("파티클 개수: "+ numParticles);
        //파티클 시스템과 파티클 시스템에서 받아온 정보
         for (int i = 0; i < numParticles; i++)
         {
            Vector3 particlePosition = myparticleSystem.transform.TransformPoint(particles[i].position); 
            //player는 스폰되기 때문에 좌표가 player를 기준으로 서술된다.
            //그러니 월드 기준으로 좌표를 바꾸어 주어야 제대로 감지가 가능하다.
                Debug.Log($"파티클 {i} 위치: {particlePosition}");
            

             Collider[] colliders = Physics.OverlapSphere(particlePosition, 1f); 

            Debug.Log($"파티클 {i} 주변 감지된 콜라이더 수: {colliders.Length}");
             foreach (var collider in colliders)
             {
                Debug.Log($"감지된 콜라이더: {collider.name}");
                 FireController flammableObject = collider.GetComponent<FireController>();
                 if (flammableObject != null)
                 {
                    Debug.Log("Trigger Activate3");
                     flammableObject.OnHitByExtinguisher();
                 }
             }
         }
     }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded 작동");
        FireDetect("Fire1");
    }
     void FireDetect(string tag)
     {
        var trigger = myparticleSystem.trigger;
        trigger.enabled = true;
        Debug.Log("FireDetect 작동");
        trigger.inside = ParticleSystemOverlapAction.Callback;      //내부
        trigger.outside = ParticleSystemOverlapAction.Ignore;   //외부
        trigger.enter = ParticleSystemOverlapAction.Callback;   //진입
        trigger.exit = ParticleSystemOverlapAction.Ignore;
        //정확히는 모르겠지만 위의 내용이 없을 경우, 사전에 설정이 되어 있더라도 오류가 걸린다.
        
        GameObject[] fireobjects = GameObject.FindGameObjectsWithTag(tag);
        Debug.Log("불 오브젝트:" + fireobjects);
            
            
        foreach(GameObject fireobject in fireobjects)
        {

            var collider = fireobject.GetComponent<BoxCollider>();
            if (collider == null)
            {
                // 콜라이더가 없으면 새로 추가
                collider = fireobject.AddComponent<BoxCollider>();
            }
            collider.isTrigger = true;
            // AddCollider에 콜라이더 추가
            Debug.Log("들어간 콜라이더: "+ fireobject);
            trigger.AddCollider(collider);
        }

     }
         void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void Update()
    {
        // FireDetect("fire1");
    }
}