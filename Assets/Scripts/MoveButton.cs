using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MoveButton : MonoBehaviour
{
    public delegate void MoveButtonDelegate();
    public event MoveButtonDelegate OnMoveButtonDown;
    
    bool _isDown = false;

    private void Update()
    {
        if (_isDown)
        {
            OnMoveButtonDown?.Invoke();
        }
    }
    
    public void ButtonDown()
    {
        _isDown = true;
    }

    public void ButtonUp()
    {
        _isDown = false;
    }
}
