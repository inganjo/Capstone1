using System.Collections;
using System.Collections.Generic;
using com.zibra.smoke_and_fire.Manipulators;
using UnityEngine;

public class DectectorController : MonoBehaviour
{
    private ZibraSmokeAndFireDetector detector;
    public GameObject ExtendEmitter;
    // Start is called before the first frame update
    void Start()
    {
        detector = gameObject.GetComponent<ZibraSmokeAndFireDetector>();
    }

    // Update is called once per frame
    void Update()
    {
        if(detector != null && ExtendEmitter != null){
            if(ExtendEmitter.activeSelf == false){
                Debug.Log("ExtendEmitter Activate");
            // Debug.Log(gameObject.name+ ":" + detector.CurrentTemparature);
                if(detector.CurrentTemparature > 0.1){
                    ExtendEmitter.SetActive(true);
                }
            }
        }
        
    }
}
