using System;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Rendering.FilterWindow;

public class InputManager : MonoBehaviour
{
    [Header("싱글톤")]
    static InputManager _instance;
    public static InputManager Instance => _instance;

    [Header("Input System")]
    InputSystemActions _inputSystemActions;

    [Header("액션")]
    public Action fireAction;
    public Action interactionAction;
    public Action holdInteractionAction;
    public Action runAction;
    public Action stopRunAction;
    public Action<Vector2> changeWeaponAction;

    Vector2 _moveInput;
    Vector2 _changeWeaponInput;
    Vector2 _pointerMoveInput;

    public Vector2 MoveInput => _moveInput;
    public Vector2 ChangeWeaponInput => _changeWeaponInput;
    public Vector2 PointerMoveInput => _pointerMoveInput;


    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
        Init();
    }

    public void Init()
    {
        _inputSystemActions = new InputSystemActions();
        _inputSystemActions.Enable();                        // InputSystemActions 활성화
        _inputSystemActions.Player.Enable();                 // Player Action Map 활성화

        _inputSystemActions.Player.Move.performed += OnMove;
        _inputSystemActions.Player.Move.canceled += OnMove;
        _inputSystemActions.Player.Fire.performed += OnFire;
        _inputSystemActions.Player.Fire.canceled += OnFire;
        _inputSystemActions.Player.Interaction.performed += OnInteraction;
        _inputSystemActions.Player.Interaction.canceled += OnInteraction;
        _inputSystemActions.Player.ChangeWeapon.performed += OnChangeWeapon;
        _inputSystemActions.Player.ChangeWeapon.canceled += OnChangeWeapon;
        _inputSystemActions.Player.PointerMove.performed += OnPointerMove;
        _inputSystemActions.Player.PointerMove.canceled += OnPointerMove;
        _inputSystemActions.Player.Run.performed += OnRun;
        _inputSystemActions.Player.Run.canceled += OnRun;
    }

    void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _moveInput = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            _moveInput = Vector2.zero;
        }
    }
    void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            runAction?.Invoke();
        }
        else if (context.canceled)
        {
            stopRunAction?.Invoke();
        }
    }
    void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            fireAction?.Invoke();
        }
    }

    void OnInteraction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interactionAction?.Invoke();
        }
        else if (context.canceled)
        {
            holdInteractionAction?.Invoke();
        }
    }

    void OnChangeWeapon(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            changeWeaponAction?.Invoke(context.ReadValue<Vector2>());
        }
        
    }

    void OnPointerMove(InputAction.CallbackContext context)
    {
        Vector3 mousePos = context.ReadValue<Vector2>();
        mousePos.z = Camera.main.nearClipPlane;         // 스크린 좌표에서 카메라까지의 거리인 nearClipPlane을 z축으로 설정
        _pointerMoveInput = Camera.main.ScreenToWorldPoint(mousePos);
    }


    public void Clear()
    {

        _inputSystemActions.Player.Disable();

        _inputSystemActions.Player.Move.performed -= OnMove;
        _inputSystemActions.Player.Move.canceled -= OnMove;
        _inputSystemActions.Player.Fire.performed -= OnFire;
        _inputSystemActions.Player.Fire.canceled -= OnFire;
        _inputSystemActions.Player.Interaction.performed -= OnInteraction;
        _inputSystemActions.Player.Interaction.canceled -= OnInteraction;
        _inputSystemActions.Player.ChangeWeapon.performed -= OnChangeWeapon;
        _inputSystemActions.Player.ChangeWeapon.canceled -= OnChangeWeapon; 
        _inputSystemActions.Player.PointerMove.performed -= OnPointerMove;
        _inputSystemActions.Player.PointerMove.canceled -= OnPointerMove;
        _inputSystemActions.Player.Run.performed -= OnRun;
        _inputSystemActions.Player.Run.canceled -= OnRun;

        _inputSystemActions.Disable();
        _inputSystemActions = null;
    }

    void OnDestroy()
    {
        Clear();
    }
}