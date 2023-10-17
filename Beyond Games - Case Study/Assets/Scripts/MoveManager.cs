using UnityEngine;
using DG.Tweening;
using System;

public class MoveManager: MonoBehaviour
{
    private BoardManager _boardManager;

    [Tooltip("Shoots a ray to check if a position has a block.")]
    [SerializeField] private LayerMask _blockLayer;

    [SerializeField, Range(0f, 1f)] private float _moveDuration = 0.3f;

    private void Awake()
    {
        _boardManager = GetComponent<BoardManager>();
    }

    private void OnEnable()
    {
        InputManager.OnValidSwipe += BlockSwipe;
    }

    private void OnDisable()
    {
        InputManager.OnValidSwipe -= BlockSwipe;
    }

    /// <summary> First loop checks for invalid movement, Second loop moves the pieces. </summary>
    private void BlockSwipe(Transform _target, Vector2 _swipeDirection)
    {
        BlockController _targetComponent = _target.GetComponent<BlockController>();

        int _direction = (int)((_swipeDirection.x == 0) ? _swipeDirection.y * -_boardManager.boardWidth : _swipeDirection.x);
        int[] _indexes = new int[_targetComponent.boundPieces.Length];

        for (int i = 0; i < _indexes.Length; i++)
        {
            _indexes[i] = _boardManager.boardPieces.IndexOf(_targetComponent.boundPieces[i]);

            if (IsMoveFlawed(_indexes[i], _direction, _swipeDirection) || IsPositionOccupied(_target, _boardManager.boardPieces[_indexes[i] + _direction]))
            {
                InvalidMove(_target.GetChild(i), _swipeDirection);
                return;
            }
        }

        for (int i = 0; i < _indexes.Length; i++)
        {
            Transform _block = _target.GetChild(i);

            _indexes[i] += _direction;
            _ = _block.DOPunchScale(Vector3.one * 0.1f, 0.2f).SetEase(Ease.OutSine);

            Vector3 _movePosition = _boardManager.boardPieces[_indexes[i]].position;
            _movePosition.z = _block.position.z;
            _ = _block.DOMove(_movePosition, _moveDuration).SetEase(Ease.OutCubic);

            _targetComponent.boundPieces[i] = _boardManager.boardPieces[_indexes[i]];
        }

        StartCoroutine(_targetComponent.UpdateBalls());
    }

    /// <summary> Makes sure the horizontal movement & index increment does not break the rules. </summary>
    /// <returns> Returns TRUE if the move is invalid. </returns>
    private bool IsMoveFlawed(int _index, int _direction, Vector2 _swipeDirection)
    {
        bool CanMoveLeft(int index)
        {
            int col = index % _boardManager.boardWidth;
            return col > 0 && IsValidIndex(index - 1);
        }

        bool CanMoveRight(int index)
        {
            int col = index % _boardManager.boardWidth;
            return col < _boardManager.boardWidth - 1 && IsValidIndex(index + 1);
        }

        if ((_swipeDirection.x > 0 && !CanMoveRight(_index)) || (_swipeDirection.x < 0 && !CanMoveLeft(_index)))
            return true;
        
        if (_swipeDirection.y != 0 && !IsValidIndex(_index + _direction))
            return true;

        return false;
    }

    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < _boardManager.boardWidth * _boardManager.boardHeight;
    }

    private bool IsPositionOccupied(Transform _original, Transform _target)
    {
        return Physics.Raycast(_target.position, _target.TransformDirection(Vector3.forward), out RaycastHit _hitData, Mathf.Infinity, _blockLayer)
            && _hitData.transform.parent != _original;
    }

    private void InvalidMove(Transform _target, Vector2 _direction)
    {
        _ = _target.DOPunchScale(Vector3.one * 0.1f, 0.2f).SetEase(Ease.OutSine);
        Vector3 _tiltRotation = new(_direction.y * 10, _direction.x * 10, 0);
        _ = transform.DOPunchRotation(-_tiltRotation, 0.5f, 0, 0).SetEase(Ease.OutSine);
    }
}
