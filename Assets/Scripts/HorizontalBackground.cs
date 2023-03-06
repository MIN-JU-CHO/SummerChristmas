using System;
using UnityEngine;

public class HorizontalBackground : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float screenLeft;
    private float screenUp;

    [SerializeField] GameObject runningCloud;
    [SerializeField] GameObject switchingCloud;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        screenLeft = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
        screenUp = Camera.main.ViewportToWorldPoint(Vector3.up).y;
    }

    private void Update()
    {
        ScrollBackground();
        CheckOutOfScreen();
    }

    private void ScrollBackground()
    {
        switch (GameManager.CurrentGameState)
        {
            case GameState.Running:
                transform.Translate(Vector3.left * (GameManager.MoveSpeed * Time.deltaTime));
                break;
            case GameState.Falling:
                transform.Translate(Vector3.up * (GameManager.MoveSpeed * Time.deltaTime));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void CheckOutOfScreen()
    {
        if (spriteRenderer.bounds.max.x < screenLeft)
        {
            transform.Translate(Vector3.right * (spriteRenderer.bounds.size.x * 2));
        }
        if (spriteRenderer.bounds.min.y > screenUp)
        {
            transform.Translate(Vector3.up * (spriteRenderer.bounds.size.y * 2));
        }
    }

    // private float GetScrollSpeed()
    // {
    //     // return spriteRenderer.GameManager.durationOfStage;
    // }
}