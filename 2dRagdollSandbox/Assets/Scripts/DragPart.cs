using UnityEngine;
using UnityEngine.EventSystems;

public class DragPart : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float _dragSpeed = 15f;
    [SerializeField] private float _maxLinearVelocityMagnitude = 20f;

    private Rigidbody2D _rigidbody;
    private Camera _camera;
    
    private bool _isDragging = false;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _camera = Camera.main;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        _isDragging = true;
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        _isDragging = false;
    }

    void FixedUpdate()
    {
        if (_isDragging)
        {
            Vector2 targetPosition = GetMouseWorldPos();
            Vector2 direction = targetPosition - (Vector2)transform.position;

            _rigidbody.AddForce(direction * _dragSpeed, ForceMode2D.Force);
            _rigidbody.linearVelocity = Vector2.ClampMagnitude(_rigidbody.linearVelocity, _maxLinearVelocityMagnitude);
        }
    }

    private Vector2 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -_camera.transform.position.z;
        return _camera.ScreenToWorldPoint(mousePos);
    }
}
