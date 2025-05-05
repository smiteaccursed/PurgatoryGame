using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    private Vector2 moveDirection = Vector2.zero;
    private bool interactPressed = false;
    private bool submitPressed = false;
    private bool lightAttack = false;
    private bool heavyAttack = false;
    private bool placed = false;
    private bool dodge = false;
    private static InputManager instance;

    private bool UIDown = false;
    private bool UIUp = false;

    private bool esc = false;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Input Manager in the scene.");
        }
        instance = this;
    }

    public static InputManager GetInstance()
    {
        return instance;
    }

    public void MovePressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveDirection = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            moveDirection = context.ReadValue<Vector2>();
        }
    }

    public void ESCPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            esc = true;
        }
        else if(context.canceled)
        {
            esc = false;
        }
    }

    public void InteractButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interactPressed = true;
        }
        else if (context.canceled)
        {
            interactPressed = false;
        }
    }

    public void SubmitPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            submitPressed = true;
        }
        else if (context.canceled)
        {
            submitPressed = false;
        }
    }

    public void LightAttackPressed(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            lightAttack = true;
        }    
        else if(context.canceled)
        {
            lightAttack = false;
        }
    }

    public void HeavyAttackPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            heavyAttack = true;
        }
        else if (context.canceled)
        {
            heavyAttack = false;
        }
    }

    public void DodgePressed(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            dodge = true;
        }
        else if(context.canceled)
        {
            dodge = false;
        }
    }

    public void PlacePressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            placed = true;
        }
        else if (context.canceled)
        {
            placed = false;
        }
    }

    public void UIUpPressed(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            UIUp = true;
        }
        else
        {
            UIUp = false;
        }
    }
    public void UIDownPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UIDown = true;
        }
        else
        {
            UIDown = false;
        }
    }

    public Vector2 GetMoveDirection()
    {
        return moveDirection;
    }

    public bool GetESCPressed()
    {
        bool result = esc;
        esc = false;
        return result;
    }
    public bool GetInteractPressed()
    {
        bool result = interactPressed;
        interactPressed = false;
        return result;
    }

    public bool GetSubmitPressed()
    {
        bool result = submitPressed;
        submitPressed = false;
        return result;
    }

    public void RegisterSubmitPressed()
    {
        submitPressed = false;
    }

    public bool GetUIUpPressed()
    {
        bool result = UIUp;
        UIUp = false;
        return result;
    }
    public void RegisterUIUpPressed()
    {
        UIUp = false;
    }
    public bool GetUIDownPressed()
    {
        bool result = UIDown;
        UIDown = false;
        return result;
    }
    public void RegisterUIDownPressed()
    {
        UIDown = false;
    }

    public void RegisterESCPressed()
    {
        esc = false;
    }

    public bool GetLightAttackPressed()
    {
        bool result = lightAttack;
        lightAttack = false;
        return result;
    }

    public bool GetHeavyAttackPressed()
    {
        bool result = heavyAttack;
        heavyAttack = false;
        return result;
    }
    public bool GetDodegPressed()
    {
        bool result = dodge;
        dodge = false;
        return result;
    }

    public bool GetPlacePressed()
    {
        bool result = placed;
        placed = false;
        return result;
    }
}