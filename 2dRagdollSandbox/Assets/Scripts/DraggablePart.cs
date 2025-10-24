using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class DraggablePart : MonoBehaviour, IDraggable
{
    [SerializeField] DraggableConfig _config;

    Rigidbody2D _rigidbody;

    private Vector2 _dragOffset;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void IDraggable.StartDrag(Vector2 startPosition)
    {
        _dragOffset = (Vector2)transform.position - startPosition;
    }

    void IDraggable.Drag(Vector2 dragPosition)
    {
        if (Vector2.Distance(dragPosition, (Vector2)transform.position) < _config.dragDistanceThreshold)
        {
            return;
        }

        Vector2 targetPosition = dragPosition + _dragOffset;
        Vector2 forceDirection = (targetPosition - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, targetPosition);

        _rigidbody.AddForce(forceDirection * _config.dragForce * distance, ForceMode2D.Force);
        _rigidbody.linearVelocity = Vector2.ClampMagnitude(_rigidbody.linearVelocity, _config.maxVelocity);
    }


    void IDraggable.FinishDrag(Vector2 finishPosition)
    {
        _rigidbody.linearDamping = 0f;
    }
}
