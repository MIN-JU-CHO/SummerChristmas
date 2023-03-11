using System;
using UnityEngine;

public class Background : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float screenLeft;
    private float screenUp;

    [SerializeField] int backgroundNumber = 1;
    private bool IsSwitchingBackground => backgroundNumber == GameManager.BackgroundPerStage;
    private bool IsBeforeSwitchingBackground => backgroundNumber + 1 == GameManager.BackgroundPerStage;
    
    [SerializeField] GameObject runningCloud;
    [SerializeField] GameObject runningToFallingCloud;
    [SerializeField] GameObject fallingCloud;
    [SerializeField] GameObject fallingToRunningCloud;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        screenLeft = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
        screenUp = Camera.main.ViewportToWorldPoint(Vector3.up).y;
    }

    private void FixedUpdate()
    {
        ScrollBackground();
        CheckOutOfScreen();
    }

    private void ScrollBackground()
    {
        switch (GameManager.CurrentGameState)
        {
            case GameState.Running:
                transform.Translate(Vector3.left * (GameManager.MoveSpeed * Time.fixedDeltaTime));
                break;
            case GameState.Falling:
                transform.Translate(Vector3.up * (GameManager.MoveSpeed * Time.fixedDeltaTime));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void CheckOutOfScreen()
    {
        if (spriteRenderer.bounds.max.x <= screenLeft)
        {
            if (IsBeforeSwitchingBackground)
            {
                transform.Translate(Vector3.right * (spriteRenderer.bounds.size.x));
                transform.Translate(Vector3.down * (spriteRenderer.bounds.size.y));
                
                backgroundNumber = 1;
                
                runningCloud.SetActive(false);
                fallingCloud.SetActive(true);
                GameManager.RunningToFalling();
            }
            else
            {
                GoRight();
                // spawn obstacle
            }
        }
        if (spriteRenderer.bounds.min.y >= screenUp)
        {
            if (IsBeforeSwitchingBackground)
            {
                transform.Translate(Vector3.right * (spriteRenderer.bounds.size.x));
                transform.Translate(Vector3.down * (spriteRenderer.bounds.size.y));
                
                backgroundNumber = 1;
                
                runningCloud.SetActive(true);
                fallingCloud.SetActive(false);
                GameManager.FallingToRunning();
            }
            else
            {
                GoBelow();
                // spawn obstacle
            }
        }
    }

    private void CheckSwitching()
    {
        if (!IsSwitchingBackground) return;

        if (GameManager.CurrentGameState == GameState.Running && spriteRenderer.bounds.min.x <= screenLeft)
        {
            backgroundNumber = 2;
        }
        else if (GameManager.CurrentGameState == GameState.Falling && spriteRenderer.bounds.max.y >= screenUp)
        {
            backgroundNumber = 2;
        }
    }

    private void GoRight()
    {
        transform.Translate(Vector3.right * (spriteRenderer.bounds.size.x * 2));
        if (IsSwitchingBackground) backgroundNumber = 2;
        else backgroundNumber += 2;
        SetCloud();
    }

    private void GoBelow()
    {
        transform.Translate(Vector3.down * (spriteRenderer.bounds.size.y * 2));
        if (IsSwitchingBackground) backgroundNumber = 2;
        else backgroundNumber += 2;
        SetCloud();
    }

    private void SetCloud()
    {
        runningCloud.SetActive(false);
        runningToFallingCloud.SetActive(false);
        fallingCloud.SetActive(false);
        fallingToRunningCloud.SetActive(false);

        switch (GameManager.CurrentGameState)
        {
            case GameState.Running:
                runningCloud.SetActive(!IsSwitchingBackground);
                runningToFallingCloud.SetActive(IsSwitchingBackground);
                break;
            case GameState.Falling:
                fallingCloud.SetActive(!IsSwitchingBackground);
                fallingToRunningCloud.SetActive(IsSwitchingBackground);
                break;
        }
    }

    // private float GetScrollSpeed()
    // {
    //     // return spriteRenderer.GameManager.durationOfStage;
    // }
}