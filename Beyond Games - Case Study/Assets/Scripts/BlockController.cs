using System.Collections;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    private Transform _transform;

    [SerializeField] private GameObject _ballObject;

    [Tooltip("Shoots a ray below to determine the piece it's on, This layermask detects pieces.")]
    [SerializeField] private LayerMask _pieceLayer;

    [HideInInspector] public Transform[] boundPieces;

    [SerializeField] private Material _material;

    private BallController[] _ballComponents;

    private const float ZOFFSET = 1f, BALLOFFSET = 0.3f;

    private void Awake()
    {
        _transform = transform;
        boundPieces = new Transform[_transform.childCount];
        _ballComponents = new BallController[_transform.childCount];

        SnapPiece();
    }

    private void SnapPiece()
    {
        for (int i = 0; i < _transform.childCount; i++)
        {
            Transform _block = _transform.GetChild(i);

            if (_material != null)
                _block.GetComponent<Renderer>().material = _material;

            if (Physics.Raycast(_block.position, _block.TransformDirection(Vector3.back), out RaycastHit _hitData, Mathf.Infinity, _pieceLayer))
            {
                Vector3 _snapPosition = _hitData.transform.position;

                _snapPosition.z += (_snapPosition.z == -0.2f) ? ZOFFSET + 0.2f : ZOFFSET;
                _block.position = _snapPosition;

                _snapPosition.z -= BALLOFFSET;
                _ballComponents[i] = Instantiate(_ballObject, _snapPosition, Quaternion.identity).GetComponent<BallController>();

                boundPieces[i] = _hitData.transform;
            }
        }
    }

    public IEnumerator UpdateBalls()
    {
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < _ballComponents.Length; i++)
        {
            _ballComponents[i].ReleaseBalls();
        }
    }
}
