using UnityEngine;
using Random = UnityEngine.Random;

public class BallController : MonoBehaviour
{
    private Transform _transform;

    private GameManager _manager;

    [Tooltip("Shoots a ray below to determine the piece it's on, This layermask detects pieces.")]
    [SerializeField] private LayerMask _pieceLayer;

    [Tooltip("Shoots a ray above to check if it can drop down, This layermask detects blocks.")]
    [SerializeField] private LayerMask _blockLayer;

    [SerializeField] private LayerMask _ballLayer;

    private bool _isMoving = false;

    private void Awake()
    {
        _transform = transform;
        _manager = GameManager.Instance;
        _manager.ballCount += _transform.childCount;
    }

    public void ReleaseBalls()
    {
        if (_isMoving)
            return;

        if (!Physics.Raycast(_transform.position, _transform.TransformDirection(Vector3.forward), out _, Mathf.Infinity, _blockLayer))
        {
            _isMoving = true;
            _manager.StartCoroutine(_manager.ExtractBalls(_transform.childCount));

            foreach (Rigidbody _rigidbody in _transform.GetComponentsInChildren<Rigidbody>())
            {
                _rigidbody.isKinematic = false;
                _rigidbody.excludeLayers -= _ballLayer;
                _rigidbody.angularVelocity = new Vector3(Random.Range(-30f, 30f), 0, Random.Range(-30f, 30f));
                _rigidbody.AddForce(100f * Random.Range(1.5f, 3f) * Vector3.down);
            }
        }
    }
}
