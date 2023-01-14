using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMove : MonoBehaviour
{
    public float speed = 0f;


    // 레벨에 따라 배경 스피드 설정
    public void SetSpeed(float setSpeed)
    {
        speed = setSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetRunningStat()
    {
        
    }

    private Vector2 target;
    // Update is called once per frame
    void Update()
    {
        target = new Vector2(transform.position.x, transform.position.y - 0.1f);
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Exit")
        {
            print("Trigger Exit");
            transform.position = new Vector3(0f, 10.4f, 0f);
        }
    }
}
