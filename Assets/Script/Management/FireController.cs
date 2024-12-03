using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FireController : MonoBehaviour, IFamable
{
    //최대 확장 크기
    [SerializeField]
    public float maxX;
    [SerializeField]
    public float maxZ;
    //초기 Emiiter 크기
    public float inititalX;
    public float inititalZ;
    [SerializeField]
    public Transform spawnpoint;
    public Vector3 scaleSpeed = new Vector3(0.1f, 0, 0.1f);
    public Vector3 scaleSpeedDown = new Vector3(-0.3f, 0,-0.3f);
    private bool isScaleDown = false;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 currentPosition = spawnpoint.position;
        currentPosition.y = 0f;
        spawnpoint.position = currentPosition;
        

        this.transform.position = spawnpoint.position;
        Debug.Log(transform.position);
        this.transform.localScale += new Vector3(inititalX,0,inititalZ); 
    }

    // Update is called once per frame
    void Update()
    {
        if(!isScaleDown){
            if(this.transform.localScale.z >= maxZ){
                scaleSpeed.z = 0f;

            }
            if(this.transform.localScale.x >= maxX)
            {
                scaleSpeed.x = 0f;
            }
            
            this.transform.localScale += scaleSpeed * Time.deltaTime; 
        }
        else
        {
            Debug.Log("fire small");
            transform.localScale += scaleSpeedDown * Time.deltaTime;

            isScaleDown = false;

        }
        if(transform.localScale.z < 0 || transform.localScale.x < 0)
        {
            Debug.Log("불이 완저히 소화되었다.");
            gameObject.SetActive(false);
        }
    }
    public void OnHitByExtinguisher(){

        isScaleDown = true;
        
    }
}
