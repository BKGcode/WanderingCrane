// Assets/Scripts/Input/InputManager.cs
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    [Header("Input Events")]
    public UnityEvent OnJumpPressed;
    public UnityEvent OnRunPressed;
    public UnityEvent OnRunReleased;
    public UnityEvent OnAttackPressed;

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            OnJumpPressed?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            OnRunPressed?.Invoke();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            OnRunReleased?.Invoke();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            OnAttackPressed?.Invoke();
        }
    }
}
