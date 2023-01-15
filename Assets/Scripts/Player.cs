using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 컴포넌트
    Rigidbody2D playerRigidbody;
    CapsuleCollider2D capsuleCollider;
    Animator animator;
    SpriteRenderer spriteRenderer;

    // 게임 전체
    [SerializeField] SpriteRenderer dashEffect;
    bool isDead = false;

    // 지상 필드
    [SerializeField] float jumpForce;
    int jumpCount = 0;
    bool isGrounded = false;
    private bool IsGrounded
    {
        get
        {
            return isGrounded;
        }
        set
        {
            isGrounded = value;
            animator.SetBool(nameof(isGrounded), value);
        }
    }
    bool isSliding = false;
    private bool IsSliding
    {
        get
        {
            return isSliding;
        }
        set
        {
            isSliding = value;
            animator.SetBool(nameof(isSliding), value);
        }
    }

    // 공중 필드
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
    }

    void Update()
    {
        if (isDead) return;

        // for test
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameManager.instance.isRunningStage = !GameManager.instance.isRunningStage;
            animator.SetBool("isRunningStage", GameManager.instance.isRunningStage);
            if (!GameManager.instance.isRunningStage) playerRigidbody.velocity = Vector3.zero;
        }

        if (GameManager.instance.isRunningStage)
        {
            Jump();
            Sliding();
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
            playerRigidbody.gravityScale = 0.0f;
        }
    }

    private void LateUpdate()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0;
            IsGrounded = true;
        }
        //else if (collision.gameObject.CompareTag("ChangeStage"))
        //{
        //    GameManager.instance.isRunningStage = !GameManager.instance.isRunningStage;
        //    animator.SetBool("isRunningStage", GameManager.instance.isRunningStage);
        //    if (GameManager.instance.isRunningStage)
        //    {
        //        GameManager.instance.FallingToRunning();
        //    }
        //    else
        //    {
        //        GameManager.instance.RunningToFalling();
        //        playerRigidbody.velocity = Vector3.zero;
        //    }
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ChangeStage"))
        {
            GameManager.instance.isRunningStage = !GameManager.instance.isRunningStage;
            animator.SetBool("isRunningStage", GameManager.instance.isRunningStage);
            if (GameManager.instance.isRunningStage)
            {
                StartCoroutine(GameManager.instance.FallingToRunning());
            }
            else
            {
                StartCoroutine(GameManager.instance.RunningToFalling());
                playerRigidbody.velocity = Vector3.zero;
            }
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < 2)
        {
            IsGrounded = false;
            jumpCount++;
            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.AddForce(new Vector2(0, jumpForce));
        }
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

        dashEffect.transform.position = transform.position;
        dashEffect.color = new Color(dashEffect.color.r, dashEffect.color.g, dashEffect.color.b, 1.0f);
        Color color = dashEffect.color;

        float t = 0;
        while (t < 0.1f)
        {
            color.a = Mathf.Lerp(1, 0, (t * 2) / 0.2f);
            dashEffect.color = color;
            transform.Translate(new Vector2(direction, 0) * dashSpeed * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }
        isDashing = false;
    }
}
