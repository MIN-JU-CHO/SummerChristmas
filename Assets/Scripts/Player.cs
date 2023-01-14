using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 컴포넌트
    Rigidbody2D playerRigidbody;
    Animator animator;
    BoxCollider2D boxCollider;

    // 게임 전체
    bool isDead = false;
    /// <summary>
    /// 달리는 스테이지라면 true, 떨어지는 스테이지라면 false
    /// </summary>
    public bool isRunningStage = true;
    IEnumerator startSliding;
    IEnumerator stopSliding;
    Vector3 originScale;

    // 지상 필드
    public float jumpForce;
    int jumpCount = 0;
    bool isGrounded = false;

    // 공중 필드

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        originScale = transform.localScale;
    }

    void Update()
    {
        boxCollider.size = GetComponent<SpriteRenderer>().bounds.size;
        InputDetection();
    }

    private void FixedUpdate()
    {
    }

    private void InputDetection()
    {
        if (isDead) return;

        if (isRunningStage)
        {
            if (Input.GetButtonDown("Jump") && jumpCount < 2)
            {
                jumpCount++;
                playerRigidbody.velocity = Vector2.zero;
                playerRigidbody.AddForce(new Vector2(0, jumpForce));
            }
            else if (Input.GetButtonDown("Sliding") && jumpCount == 0)
            {
                startSliding = ScaleLerp(transform.localScale, new Vector3(transform.localScale.x, transform.localScale.y / 2.0f, 1), 0.5f);
                //StopCoroutine(stopSliding);
                StartCoroutine(startSliding);
            }
            else if (Input.GetButtonUp("Sliding") && jumpCount == 0)
            {
                stopSliding = ScaleLerp(transform.localScale, originScale, 0.5f);
                StopCoroutine(startSliding);
                StartCoroutine(stopSliding);
            }
        }
        else
        {

        }
    }

    private IEnumerator ScaleLerp(Vector3 startScale, Vector3 endScale, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, t / duration);
            yield return null;
        }
    }
}
