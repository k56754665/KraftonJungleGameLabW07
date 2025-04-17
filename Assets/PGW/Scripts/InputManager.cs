using System;
using UnityEngine;
using UnityEngine.InputSystem;

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
    bool _isFocusing; 

    public Vector2 MoveInput => _moveInput;
    public Vector2 ChangeWeaponInput => _changeWeaponInput;
    public Vector2 PointerMoveInput => _pointerMoveInput;
    public bool IsFocusing => _isFocusing;


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
        _inputSystemActions.Player.Interaction.performed += OnInteraction;
        _inputSystemActions.Player.Interaction.canceled += OnInteraction;
        _inputSystemActions.Player.ChangeWeapon.performed += OnChangeWeapon;
        _inputSystemActions.Player.ChangeWeapon.canceled += OnChangeWeapon;
        _inputSystemActions.Player.Run.performed += OnRun;
        _inputSystemActions.Player.Run.canceled += OnRun;
        _inputSystemActions.Player.Focus.performed += OnFocus;
        _inputSystemActions.Player.Focus.canceled += OnFocus;
    }

    private void Update()
    {
        Vector2 pointerPosition = _inputSystemActions.Player.PointerMove.ReadValue<Vector2>();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(pointerPosition.x, pointerPosition.y, Camera.main.nearClipPlane));

        _pointerMoveInput = worldPosition;
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

    void OnFocus(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isFocusing = true;
        }
        else if (context.canceled)
        {
            _isFocusing = false;
        }
    }


    public void Clear()
    {

        _inputSystemActions.Player.Disable();

        _inputSystemActions.Player.Move.performed -= OnMove;
        _inputSystemActions.Player.Move.canceled -= OnMove;
        _inputSystemActions.Player.Fire.performed -= OnFire;
        _inputSystemActions.Player.Interaction.performed -= OnInteraction;
        _inputSystemActions.Player.Interaction.canceled -= OnInteraction;
        _inputSystemActions.Player.ChangeWeapon.performed -= OnChangeWeapon;
        _inputSystemActions.Player.ChangeWeapon.canceled -= OnChangeWeapon; 
        _inputSystemActions.Player.Run.performed -= OnRun;
        _inputSystemActions.Player.Run.canceled -= OnRun;
        _inputSystemActions.Player.Focus.performed += OnFocus;
        _inputSystemActions.Player.Focus.canceled -= OnFocus;

        _inputSystemActions.Disable();
        _inputSystemActions = null;
    }

    void OnDestroy()
    {
        Clear();
    }
}