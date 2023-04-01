using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    // ������Ʈ
    Rigidbody2D playerRigidbody;
    CapsuleCollider2D capsuleCollider;
    Animator animator;
    SpriteRenderer spriteRenderer;

    // ���� ��ü
    [SerializeField] SpriteRenderer dashEffect;
    [SerializeField] GameObject credit;
    bool isDead = false;

    // ���� �ʵ�
    [SerializeField] float jumpForce;
    int jumpCount = 0;
    bool isGrounded = false;

    public bool IsGrounded
    {
        get => isGrounded;
        private set
        {
            isGrounded = value;
            animator.SetBool(nameof(IsGrounded), value);
        }
    }

    bool isSliding = false;

    private bool IsSliding
    {
        get => isSliding;
        set
        {
            isSliding = value;
            animator.SetBool(nameof(IsSliding), value);
        }
    }

    public bool IsReadyToFalling => transform.position.x >= -0.01f;

    // ���� �ʵ�
    [SerializeField] float playerSpeed;
    [SerializeField] float dashSpeed;
    Vector2 moveVector;
    float delayBetweenPress = 0.4f;
    bool isDetectingDash = true;
    bool isDashing = false;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        playerRigidbody.velocity = Vector2.down * jumpForce;
        spriteRenderer.flipX = false;
        playerRigidbody.gravityScale = 1.0f;
    }

    private void Update()
    {
#if UNITY_EDITOR
        UnitTest();
#endif

        if (isDead) return;

        // for test
        // if (Input.GetKeyDown(KeyCode.Q))
        // {
        //     GameManager.instance.IsRunningStage = !GameManager.instance.IsRunningStage;
        //     animator.SetBool("IsRunningStage", GameManager.instance.IsRunningStage);
        //     if (!GameManager.instance.IsRunningStage) playerRigidbody.velocity = Vector3.zero;
        // }

        if (GameManager.CurrentGameState == GameState.Running)
        {
            DetectJump();
            DetectSliding();
        }
        else
        {
            moveVector.x = Input.GetAxisRaw("Horizontal");
            if (!isDashing)
            {
                transform.Translate(playerSpeed * Time.deltaTime * moveVector);
                if (moveVector.x < 0) spriteRenderer.flipX = true;
                else if (moveVector.x > 0) spriteRenderer.flipX = false;
            }

            if (Input.GetButtonDown("Horizontal") && isDetectingDash) StartCoroutine(CoDetectDash(moveVector.x));
        }
    }

    private void UnitTest()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0;
            IsGrounded = true;
        }
    }

    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (collision.gameObject.CompareTag("ChangeStage"))
    //     {
    //         GameManager.instance.IsRunningStage = !GameManager.instance.IsRunningStage;
    //         animator.SetBool("IsRunningStage", GameManager.instance.IsRunningStage);
    //         if (GameManager.instance.IsRunningStage)
    //         {
    //             StartCoroutine(GameManager.instance.FallingToRunning());
    //             StartCoroutine(LinearMove(transform.position, new Vector2(-6, 0), GameManager.instance.stageDelayTime));
    //         }
    //         else
    //         {
    //             StartCoroutine(GameManager.instance.RunningToFalling());
    //             StartCoroutine(LinearMove(transform.position, new Vector2(1, 3.5f), GameManager.instance.stageDelayTime));
    //             playerRigidbody.velocity = Vector3.zero;
    //         }
    //     }
    //     else if (collision.gameObject.CompareTag("Obstacle"))
    //     {
    //         credit.SetActive(true);
    //         isDead = true;
    //     }
    // }

    private void DetectJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (jumpCount >= 2) return;

        IsGrounded = false;
        jumpCount++;
        StartCoroutine(CoJump(jumpCount));
    }

    private IEnumerator CoJump(int currentJump)
    {
        playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);

        float t = 0;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            if (jumpCount == 2 && currentJump != jumpCount) yield break;
            yield return null;
        }

        playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, -(jumpForce * 1.1f));
    }

    private void DetectSliding()
    {
        if (Input.GetButtonDown("Sliding") && jumpCount == 0)
        {
            IsSliding = true;
            capsuleCollider.size = new Vector2(20.48f, 12);
        }
        else if (Input.GetButtonUp("Sliding") && jumpCount == 0)
        {
            IsSliding = false;
            capsuleCollider.size = new Vector2(20.48f, 20.48f);
        }
    }

    private IEnumerator CoDetectDash(float direction)
    {
        isDetectingDash = false;

        float t = 0;
        while (t < delayBetweenPress)
        {
            if (t > 0 && Input.GetButtonDown("Horizontal") && Input.GetAxisRaw("Horizontal") == direction)
            {
                StartCoroutine(CoDash(direction));

                isDetectingDash = true;
                yield break;
            }

            t += Time.deltaTime;
            yield return null;
        }

        isDetectingDash = true;
    }

    private IEnumerator CoDash(float direction)
    {
        isDashing = true;
        animator.SetTrigger("Dash");

        if (direction < 0) dashEffect.GetComponent<SpriteRenderer>().flipX = true;
        else dashEffect.GetComponent<SpriteRenderer>().flipX = false;
        dashEffect.transform.position = transform.position;
        dashEffect.color = new Color(dashEffect.color.r, dashEffect.color.g, dashEffect.color.b, 1.0f);
        Color color = dashEffect.color;

        float t = 0;
        while (t < 0.1f)
        {
            color.a = Mathf.Lerp(1, 0, (t * 2) / 0.2f);
            dashEffect.color = color;
            transform.Translate(dashSpeed * Time.deltaTime * new Vector2(direction, 0));
            t += Time.deltaTime;
            yield return null;
        }

        color.a = 0;
        dashEffect.color = color;
        isDashing = false;
    }

    public void RunningToFalling()
    {
        playerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        playerRigidbody.DOMove(new Vector2(0, 3.5f), 6f / GameManager.MoveSpeed).SetEase(Ease.Linear);
        IsGrounded = false;
    }

    public void FallingToRunning()
    {
        animator.SetBool("Parachute", true);
        playerRigidbody.AddForce(Vector2.down * (GameManager.MoveSpeed * 10));
        StartCoroutine(nameof(CoDetectGround));
    }

    private IEnumerator CoDetectGround()
    {
        while (true)
        {
            var hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity,
                LayerMask.GetMask("Cloud"));
            Debug.Log(hit.distance);
            if (hit.distance < 1.1f)
            {
                IsGrounded = true;
                yield break;
            }
            yield return null;
        }
    }

    public void SetRunningAttribute()
    {
        spriteRenderer.flipX = false;
        playerRigidbody.gravityScale = 1.0f;
        playerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        transform.DOMoveX(-6, 2);
        animator.SetBool("IsRunningStage", true);
    }

    public void SetFallingAttribute()
    {
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.gravityScale = 0;
        animator.SetBool("IsRunningStage", false);
    }
}