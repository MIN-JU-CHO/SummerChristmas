using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 컴포넌트
    Rigidbody2D playerRigidbody;
    Animator animator;

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
    [SerializeField] float jumpForce;
    int jumpCount = 0;
    bool isGrounded = false;

    // 공중 필드
    [SerializeField] float playerSpeed;
    [SerializeField] float dashForce;
    Vector2 moveVector;
    float delayBetweenPress = 0.4f;
    bool isDetectingDash = true;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originScale = transform.localScale;
    }

    void Update()
    {
        if (isDead) return;

        if (isRunningStage)
        {
            Jump();
            Sliding();
        }
        Debug.Log(playerRigidbody.velocity);
    }

    private void FixedUpdate()
    {
        if (isRunningStage)
        {

        }
        else
        {
            moveVector.x = Input.GetAxisRaw("Horizontal");
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x * moveVector.x * playerSpeed,
                playerRigidbody.velocity.y);
            transform.Translate(playerSpeed * Time.deltaTime * moveVector);
            if (Input.GetButtonDown("Horizontal") && isDetectingDash) StartCoroutine(DashDetection(moveVector.x));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0;
            isGrounded = true;
        }
    }
    
    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < 2)
        {
            isGrounded = false;
            jumpCount++;
            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.AddForce(new Vector2(0, jumpForce));
        }
    }

    private void Sliding()
    {
        Debug.Log("Sliding");
        if (Input.GetButtonDown("Sliding") && jumpCount == 0)
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

    private void InputDetection()
    {
        if (isDead) return;

        if (isRunningStage)
        {
            if (Input.GetButtonDown("Jump") && jumpCount < 2)
            {
                isGrounded = false;
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
            moveVector.x = Input.GetAxisRaw("Horizontal");
            playerRigidbody.velocity = new Vector2(moveVector.x * playerSpeed, playerRigidbody.velocity.y);
            transform.Translate(playerSpeed * Time.deltaTime * moveVector);
            if (Input.GetButtonDown("Horizontal") && isDetectingDash) StartCoroutine(DashDetection(moveVector.x));
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

    private IEnumerator DashDetection(float direction)
    {
        isDetectingDash = false;

        float t = 0;
        while (t < delayBetweenPress)
        {
            if (t > 0 && Input.GetButtonDown("Horizontal") && Input.GetAxisRaw("Horizontal") == direction)
            {
                Debug.Log("Dash");
                playerRigidbody.AddForce(new Vector2(direction, 0) * dashForce);

                isDetectingDash = true;
                yield break;
            }
            t += Time.deltaTime;
            yield return null;
        }

        isDetectingDash = true;
    }
}
