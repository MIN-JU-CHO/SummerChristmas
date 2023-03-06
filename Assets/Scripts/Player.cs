using System.Collections;
using System.Collections.Generic;
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
    private bool IsGrounded
    {
        get => isGrounded;
        set
        {
            isGrounded = value;
            animator.SetBool(nameof(isGrounded), value);
        }
    }
    bool isSliding = false;
    private bool IsSliding
    {
        get => isSliding;
        set
        {
            isSliding = value;
            animator.SetBool(nameof(isSliding), value);
        }
    }

    // ���� �ʵ�
    [SerializeField] float playerSpeed;
    [SerializeField] float dashSpeed;
    Vector2 moveVector;
    float delayBetweenPress = 0.4f;
    bool isDetectingDash = true;
    bool isDashing = false;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerRigidbody.velocity = Vector2.down * jumpForce;
    }

    void Update()
    {
        if (isDead) return;
        Debug.Log(playerRigidbody.velocity);

        // for test
        // if (Input.GetKeyDown(KeyCode.Q))
        // {
        //     GameManager.instance.IsRunningStage = !GameManager.instance.IsRunningStage;
        //     animator.SetBool("IsRunningStage", GameManager.instance.IsRunningStage);
        //     if (!GameManager.instance.IsRunningStage) playerRigidbody.velocity = Vector3.zero;
        // }

        if (GameManager.CurrentGameState == GameState.Running)
        {
            Jump();
            Sliding();
            spriteRenderer.flipX = false;
            playerRigidbody.gravityScale = 1.0f;
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
            if (Input.GetButtonDown("Horizontal") && isDetectingDash) StartCoroutine(DashDetection(moveVector.x));
            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.gravityScale = 0.0f;
        }
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

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < 2)
        {
            IsGrounded = false;
            jumpCount++;
            StartCoroutine(JumpCoroutine(jumpCount));
        }
    }

    private IEnumerator JumpCoroutine(int currentJump)
    {
        playerRigidbody.velocity = Vector2.up * jumpForce;

        float t = 0;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            if (jumpCount == 2 && currentJump != jumpCount) yield break;
            yield return null;
        }

        playerRigidbody.velocity = Vector2.down * jumpForce * 1.1f;
    }

    private void Sliding()
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

    private IEnumerator DashDetection(float direction)
    {
        isDetectingDash = false;

        float t = 0;
        while (t < delayBetweenPress)
        {
            if (t > 0 && Input.GetButtonDown("Horizontal") && Input.GetAxisRaw("Horizontal") == direction)
            {
                StartCoroutine(Dash(direction));

                isDetectingDash = true;
                yield break;
            }
            t += Time.deltaTime;
            yield return null;
        }

        isDetectingDash = true;
    }

    private IEnumerator Dash(float direction)
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

    public IEnumerator LinearMove(Vector2 startPos, Vector2 endPos, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            transform.position = Vector2.Lerp(startPos, endPos, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
    }
}
