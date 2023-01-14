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

    private Vector2 target;
    // Update is called once per frame
    void Update()
    {
        // 배경 위로 움직이기
        target = new Vector2(transform.position.x, transform.position.y + 0.1f);
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 배경 위치 리셋
        if(other.gameObject.tag == "Exit")
        {
            print("Trigger Exit");
            transform.position = new Vector3(0f, -10.4f, 0f);
            ResetObstacles();
        }
    }

    [SerializeField] private Transform [] locations_obs = new Transform [9];
    [SerializeField] private Object [] obstacles = new Object [2];
    [SerializeField] private GameObject group;

    // 장애물 위치도 리셋
    private void ResetObstacles()
    {
        // 기존 장애물 지우기
        Obstacle [] pre_obstacles = group.GetComponentsInChildren<Obstacle>();
        foreach (Obstacle obs in pre_obstacles)
        {
            Destroy(obs.gameObject);
        }

        // 새로 랜덤 장애물 만들기
        int randObs, randColumn, randRow;
        int [] row_shuffle = {0, 3, 6};

        randRow = Random.Range(1, 4);
        ArrayShuffle(row_shuffle);
        for (int i = 0; i < randRow; i++)
        {
            randObs = Random.Range(0, 2);
            randColumn = Random.Range(0, 3);
            Instantiate(obstacles[randObs], locations_obs[row_shuffle[i] + randColumn].position, Quaternion.identity, group.transform);
        }

    }


    // Array Shuffle
    private void ArrayShuffle(int[] array)
    {
        int rand1, rand2, temp;
        for(int i = 0; i < array.Length; i++)
        {
            rand1 = Random.Range(0, array.Length);
            rand2 = Random.Range(0, array.Length);

            temp = array[rand1];
            array[rand1] = array[rand2];
            array[rand2] = temp;
        }
    }
}
