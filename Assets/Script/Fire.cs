using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] private ParticleSystem myparticleSystem; // 파티클 시스템


    void Start()
    {
        if (myparticleSystem == null)
        {
            myparticleSystem = GetComponent<ParticleSystem>();
        }
        
        // 트리거 모듈 활성화

    }

    void OnParticleTrigger()
    {
        // Debug.Log("Trigger Activate");
         ParticleSystem.Particle[] particles = new ParticleSystem.Particle[myparticleSystem.main.maxParticles];
         int numParticles = myparticleSystem.GetParticles(particles);
        //파티클 시스템과 파티클 시스템에서 받아온 정보
         for (int i = 0; i < numParticles; i++)
         {
   
             Collider[] colliders = Physics.OverlapSphere(particles[i].position, 5f); 
             //각각 오브젝트의 위치 파악
            
             foreach (var collider in colliders)
             {

                 FireController flammableObject = collider.GetComponent<FireController>();
                 if (flammableObject != null)
                 {
                    Debug.Log("Trigger Activate");
                     flammableObject.OnHitByExtinguisher();
                 }
             }
         }
     }
}