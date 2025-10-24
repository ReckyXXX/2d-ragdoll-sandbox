using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private InputActionReference _scrollActionReference;
        [SerializeField] private InputActionReference _pointerPressActionReference;
        [SerializeField] private InputActionReference _pointerPositionActionReference;

        [Header("Zoom Settings")]
        [SerializeField] private float _zoomSpeed;
        [SerializeField] private float _minZoom;
        [SerializeField] private float _maxZoom;
        [SerializeField] private float _smoothZoomSpeed;

        [Header("Drag Settings")]
        [SerializeField] private float _dragSpeed;
        [SerializeField] private float _smoothDragSpeed;

        private UnityEngine.Camera _camera;

        private float _targetZoom;

        private bool _dragging;

        private Vector3 _cameraStartPosition;
        private Vector3 _targetPosition;
        private Vector3 _dragStartPosition;

        private Vector3 PointerWorldPosition
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

        private void Start()
        {
            _targetZoom = _camera.orthographicSize;
            _targetPosition = _camera.transform.position;
        }

        private void OnEnable()
        {
            _scrollActionReference.action.performed += ScrollPerformed;
            _pointerPressActionReference.action.performed += PointerPressPerformed;
            _pointerPressActionReference.action.canceled += PointerPressCanceled;
            _pointerPositionActionReference.action.performed += PointerPositionChanged;
        }

        private void OnDisable()
        {
            _scrollActionReference.action.performed -= ScrollPerformed;
            _pointerPressActionReference.action.performed -= PointerPressPerformed;
            _pointerPressActionReference.action.canceled -= PointerPressCanceled;
            _pointerPositionActionReference.action.performed -= PointerPositionChanged;
        }

        private void Update()
        {
            MoveCamera();
        }

        private void ScrollPerformed(InputAction.CallbackContext context)
        {
            float scrollValue = context.ReadValue<Vector2>().y;
            _targetZoom -= scrollValue * _zoomSpeed;
            _targetZoom = Mathf.Clamp(_targetZoom, _minZoom, _maxZoom);
        }

        private void PointerPressPerformed(InputAction.CallbackContext context)
        {
            _dragging = true;
            _dragStartPosition = PointerWorldPosition;
            _cameraStartPosition = _camera.transform.position;
        }

        private void PointerPositionChanged(InputAction.CallbackContext context)
        {
            if (!_dragging)
            {
                return;
            }

            Vector3 dragDelta = _dragStartPosition - PointerWorldPosition;
            _targetPosition = _cameraStartPosition + dragDelta * _dragSpeed;
        }

        private void PointerPressCanceled(InputAction.CallbackContext context)
        {
            _dragging = false;
        }

        private void MoveCamera()
        {
            if (!Mathf.Approximately(_camera.orthographicSize, _targetZoom))
            {
                _camera.orthographicSize = Mathf.Lerp(
                    _camera.orthographicSize,
                    _targetZoom,
                    _smoothZoomSpeed * Time.deltaTime
                );
            }

            if (Vector3.Distance(transform.position, _targetPosition) > 0.01f)
            {
                _camera.transform.position = Vector3.Lerp(
                    _camera.transform.position,
                    _targetPosition,
                    _smoothDragSpeed * Time.deltaTime
                );
            }
        }
    }
}