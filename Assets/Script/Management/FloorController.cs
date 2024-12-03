using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(ScenarioManager.Instance == null){
            // GameObject[] fireObjects1 = GameObject.FindGameObjectsWithTag("Fire1");
            // foreach (GameObject obj in fireObjects1)
            // {
            //     Debug.Log("시나리오"+obj);
            //     obj.SetActive(false);
            // }
            GameObject[] fireObjects2 = GameObject.FindGameObjectsWithTag("Fire2");
            foreach (GameObject obj in fireObjects2)
            {
                Debug.Log("시나리오"+obj);
                obj.SetActive(false);
            }
            GameObject[] fireObjects3 = GameObject.FindGameObjectsWithTag("Fire3");
            foreach (GameObject obj in fireObjects3)
            {
                Debug.Log("시나리오"+obj);
                obj.SetActive(false);
            }
        }
        if(ScenarioManager.Instance.currentScenario == 1)
        {
            Debug.Log("시나리오 1");
             GameObject[] fireObjects1 = GameObject.FindGameObjectsWithTag("Fire2");
             GameObject[] fireObjects2 = GameObject.FindGameObjectsWithTag("Fire3");

            foreach (GameObject obj in fireObjects1)
            {
                Debug.Log("시나리오"+obj);
                obj.SetActive(false);
            }
            foreach (GameObject obj in fireObjects2)
            {
                Debug.Log("시나리오"+obj);
                obj.SetActive(false);
            }

        }
        else if(ScenarioManager.Instance.currentScenario == 2)
        {

             GameObject[] fireObjects1 = GameObject.FindGameObjectsWithTag("Fire1");
             GameObject[] fireObjects2 = GameObject.FindGameObjectsWithTag("Fire3");

            foreach (GameObject obj in fireObjects1)
            {
                Debug.Log("시나리오"+obj);
                obj.SetActive(false);
            }
            foreach (GameObject obj in fireObjects2)
            {
                Debug.Log("시나리오"+obj);
                obj.SetActive(false);
            }
        }
        else if(ScenarioManager.Instance.currentScenario == 3)
        {
    
             GameObject[] fireObjects1 = GameObject.FindGameObjectsWithTag("Fire1");
             GameObject[] fireObjects2 = GameObject.FindGameObjectsWithTag("Fire2");

            foreach (GameObject obj in fireObjects1)
            {
                Debug.Log("시나리오"+obj);
                obj.SetActive(false);
            }
            foreach (GameObject obj in fireObjects2)
            {
                Debug.Log("시나리오"+obj);
                obj.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
