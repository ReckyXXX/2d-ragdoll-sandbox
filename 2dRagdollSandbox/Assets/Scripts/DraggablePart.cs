using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class DraggablePart : MonoBehaviour, IDraggable
{
    [SerializeField] float _dragForce = 300f;
    [SerializeField] float _dragDistanceThreshold = 1f;
    [SerializeField] private float _maxVelocity = 10f;

    Rigidbody2D _rigidbody;
    Camera _camera;

    private Vector2 _dragOffset;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _camera = Camera.main;
    }

    void IDraggable.StartDrag(Vector2 startPosition)
    {
        _dragOffset = (Vector2)transform.position - startPosition;
    }

    void IDraggable.Drag(Vector2 dragPosition)
    {
        if (Vector2.Distance(dragPosition, (Vector2)transform.position) < _dragDistanceThreshold)
        {
            return;
        }

        Vector2 targetPosition = dragPosition + _dragOffset;

        Vector2 forceDirection = (targetPosition - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, targetPosition);

        _rigidbody.AddForce(forceDirection * _dragForce * distance, ForceMode2D.Force);
        _rigidbody.linearVelocity = Vector2.ClampMagnitude(_rigidbody.linearVelocity, _maxVelocity);
    }


    void IDraggable.FinishDrag(Vector2 startPosition)
    {
        _rigidbody.linearDamping = 0f;
    }
}
