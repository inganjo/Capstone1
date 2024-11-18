using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
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
    // Start is called before the first frame update
    void Start()
    {
        Vector3 currentPosition = transform.position;
        currentPosition.y = 0f;
        transform.position = currentPosition;

        this.transform.position = spawnpoint.position;
        this.transform.localScale += new Vector3(inititalX,0,inititalZ); 
    }

    // Update is called once per frame
    void Update()
    {
        if(this.transform.localScale.z >= maxZ){
            scaleSpeed.z = 0f;

        }
        if(this.transform.localScale.x >= maxX)
        {
            scaleSpeed.x = 0f;
        }
        this.transform.localScale += scaleSpeed * Time.deltaTime; 
    }
}
