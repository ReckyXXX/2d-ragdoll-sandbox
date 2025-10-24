using UnityEngine;

namespace Game.Draggable
{
    public interface IDraggable
    {
        void StartDrag(Vector2 startPosition);
        void Drag(Vector2 dragPosition);
        void FinishDrag(Vector2 finishPosition);
    }
}