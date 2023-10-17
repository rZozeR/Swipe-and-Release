using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Camera _camera;
    private PlayerInput _playerInput;
    private InputAction _actionClick, _actionPosition;


    public delegate void SwipeEventHandler(Transform transform, Vector2 position);
    public static event SwipeEventHandler OnValidSwipe;


    [Tooltip("Interactable object's Layer")]
    [SerializeField] private LayerMask _interactableLayer;

    [Tooltip("Cooldown between each swipe in seconds")]
    [SerializeField, Range(0f, 2f)] private float _swipeCooldownTimer = 0.5f;

    [Tooltip("Threshold between input start and end positions")]
    [SerializeField, Range(0f, 10f)] private float swipeThreshold = 5f;


    private Transform _selection;
    private Vector2 _inputStartPos;
    private bool _inputCooldown;


    private void Awake()
    {
        _camera = Camera.main;
        _playerInput = GetComponent<PlayerInput>();

        _actionClick = _playerInput.actions["Click"];
        _actionPosition = _playerInput.actions["Click Position"];
    }

    private void OnEnable()
    {
        _actionClick.started += InputStart;
        _actionClick.canceled += InputEnd;
    }

    private void OnDisable()
    {
        _actionClick.started -= InputStart;
        _actionClick.canceled -= InputEnd;
    }

    private void InputStart(InputAction.CallbackContext _context)
    {
        if (_inputCooldown) return;
        StartCoroutine(ResetCooldown());

        _inputStartPos = _actionPosition.ReadValue<Vector2>();
        _selection = (Physics.Raycast(_camera.ScreenPointToRay(_inputStartPos), out RaycastHit _rayHit, float.PositiveInfinity, _interactableLayer)) ?
            _rayHit.transform.parent : null;
    }

    private void InputEnd(InputAction.CallbackContext _context)
    {
        if (_selection == null) return;

        Vector2 _swipeDelta = _actionPosition.ReadValue<Vector2>() - _inputStartPos;

        if (_swipeDelta.magnitude < swipeThreshold) return;

        Vector2 _swipeDirection = Mathf.Abs(_swipeDelta.x) > Mathf.Abs(_swipeDelta.y) ?
            (int)Mathf.Sign(_swipeDelta.x) * Vector2.right :
            (int)Mathf.Sign(_swipeDelta.y) * Vector2.up;

        OnValidSwipe?.Invoke(_selection, _swipeDirection);
    }

    IEnumerator ResetCooldown()
    {
        _inputCooldown = true;
        yield return new WaitForSeconds(_swipeCooldownTimer);
        _inputCooldown = false;
    }
}
