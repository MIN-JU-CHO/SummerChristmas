using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBlink : MonoBehaviour
{
    Text _start;
    public float BlinkFadeInTime = 0.5f;
    public float BlinkStayTime = 0.8f;
    public float BlinkFadeOutTime = 0.7f;
    private float _timeChecker = 0;
    private Color _color;

    // Start is called before the first frame update
    void Start()
    {
        _start = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        _timeChecker += Time.deltaTime;
        if (_timeChecker < BlinkFadeInTime)
        {
            _start.color = new Color(_color.r, _color.g, _color.b, _timeChecker / BlinkFadeInTime);
        }
        else if (_timeChecker < BlinkFadeInTime + BlinkStayTime)
        {
            _start.color = new Color(_color.r, _color.g, _color.b, 1);
        }
        else if (_timeChecker < BlinkFadeInTime + BlinkStayTime + BlinkFadeOutTime)
        {
            _start.color = new Color(_color.r, _color.g, _color.b, 1 - (_timeChecker - (BlinkFadeInTime + BlinkStayTime)) / BlinkFadeOutTime );
        }
        else if (_timeChecker < BlinkFadeInTime + BlinkStayTime)
        {
            _start.color = new Color(_color.r, _color.g, _color.b, 1);
        }
        else
        {
            _timeChecker = 0;
        }
    }
}
