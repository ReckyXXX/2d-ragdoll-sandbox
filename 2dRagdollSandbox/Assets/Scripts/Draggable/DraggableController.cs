using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Draggable
{
    public class DraggableController : MonoBehaviour
    {
        [SerializeField] private InputActionReference _pointerPressActionReference;
        [SerializeField] private InputActionReference _pointerPositionActionReference;
        [SerializeField] private LayerMask _dragLayers = -1;

        private UnityEngine.Camera _camera;
        private WaitForFixedUpdate _waitForFixedUpdate = new();

        private bool _dragging;

        private Vector2 PointerWorldPosition 
        {
            get 
            {
                Vector2 pointerScreenPosition = _pointerPositionActionReference.action.ReadValue<Vector2>();
                return _camera.ScreenToWorldPoint(new Vector3(pointerScreenPosition.x, pointerScreenPosition.y, -_camera.transform.position.z));
            } 
        }

        void Awake()
        {
            _camera = UnityEngine.Camera.main;
        }

        private void OnEnable()
        {
            _pointerPressActionReference.action.performed += PointerPressPerformed;
            _pointerPressActionReference.action.canceled += PointerPressCanceled;
        }

        private void OnDisable()
        {
            _pointerPressActionReference.action.performed -= PointerPressPerformed;
            _pointerPressActionReference.action.canceled -= PointerPressCanceled;
        }

        private void PointerPressCanceled(InputAction.CallbackContext context)
        {
            _dragging = false;
        }

        private void PointerPressPerformed(InputAction.CallbackContext context)
        {
            RaycastHit2D hit2D = Physics2D.Raycast(PointerWorldPosition, Vector2.zero, Mathf.Infinity, _dragLayers);
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
}
