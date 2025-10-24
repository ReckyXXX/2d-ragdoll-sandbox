using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : MonoBehaviour
{
    [SerializeField] private InputActionReference _pointerPressActionReference;
    [SerializeField] private InputActionReference _pointerPositionActionReference;
    [SerializeField] private LayerMask dragLayers = -1;

    private Camera _camera;
    private Vector2 _pointerScreenPosition;
    private WaitForFixedUpdate _waitForFixedUpdate = new ();

    private bool _dragging;

    private Vector2 PointerWorldPosition => _camera.ScreenToWorldPoint(new Vector3(_pointerScreenPosition.x, _pointerScreenPosition.y, -_camera.transform.position.z));

    void Awake()
    {
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        _pointerPressActionReference.action.performed += PointerPressPerformed;
        _pointerPressActionReference.action.canceled += PointerPressCanceled;
        _pointerPositionActionReference.action.performed += PointerPositionChanged;
    }

    private void OnDisable()
    {
        _pointerPressActionReference.action.performed -= PointerPressPerformed;
        _pointerPressActionReference.action.canceled -= PointerPressCanceled;
        _pointerPositionActionReference.action.performed -= PointerPositionChanged;
    }

    private void PointerPositionChanged(InputAction.CallbackContext context)
    {
        _pointerScreenPosition = context.ReadValue<Vector2>();
    }

    private void PointerPressCanceled(InputAction.CallbackContext context)
    {
        _dragging = false;
    }

    private void PointerPressPerformed(InputAction.CallbackContext context)
    {
        RaycastHit2D hit2D = Physics2D.Raycast(PointerWorldPosition, Vector2.zero, Mathf.Infinity, dragLayers);
        if (hit2D.collider != null && hit2D.collider.TryGetComponent(out IDraggable draggable))
        {
            StartCoroutine(DragProcess(draggable));
        }
    }

    IEnumerator DragProcess(IDraggable draggable)
    {
        _dragging = true;
        draggable.StartDrag(PointerWorldPosition);
        while (_dragging)
        {
            draggable.Drag(PointerWorldPosition);
            yield return _waitForFixedUpdate;
        }
        draggable.FinishDrag(PointerWorldPosition);
    }
}