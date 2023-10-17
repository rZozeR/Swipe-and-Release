using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject _boardPiece;

    [SerializeField] private Material _materialVariant1, _materialVariant2;


    [Range(1, 10)] public int boardWidth = 0, boardHeight = 0;

    [HideInInspector] public List<Transform> boardPieces = new();


    private Transform _boardBox, _boardPiecesParent;


    private const float SCALE_MAGNIFY = 1;

    private float _cubeSize;


    private void Awake()
    {
        InitiliazeBoard();
    }

    [ContextMenu("Create Board")]
    public void InitiliazeBoard()
    {
        _boardBox = transform.GetChild(0);
        _boardPiecesParent = transform.GetChild(1);

        ClearBoard();
        CreateBoard();
    }

    private void ClearBoard()
    {
        List<Transform> _tempList = _boardPiecesParent.Cast<Transform>().ToList();
        foreach (Transform _child in _tempList)
        {
            DestroyImmediate(_child.gameObject);
        }
        boardPieces.Clear();
    }

    private void CreateBoard()
    {
        _cubeSize = _boardPiece.GetComponent<Renderer>().bounds.size.x;
        _boardBox.localScale = new Vector3((_cubeSize * boardWidth) + SCALE_MAGNIFY, (_cubeSize * boardHeight) + SCALE_MAGNIFY, 1);

        CreateBoardPieces();
    }

    private void CreateBoardPieces()
    {
        Vector3 _spawnPosition;
        GameObject _spawnedPiece;

        Vector2 _spawnOffset = new Vector2((boardWidth - 1) * _cubeSize, (boardHeight - 1) * _cubeSize) * 0.5f;

        for (int i = 0; i < boardHeight; i++)
        {
            for (int j = 0; j < boardWidth; j++)
            {
                _spawnPosition = transform.position + new Vector3(_spawnOffset.x - j, _spawnOffset.y - i, 0);
                _spawnedPiece = Instantiate(_boardPiece, _spawnPosition, Quaternion.identity, _boardPiecesParent);

                if ((i + j) % 2 == 0)
                {
                    _spawnedPiece.GetComponent<Renderer>().material = _materialVariant1;
                }
                else
                {
                    _spawnedPiece.GetComponent<Renderer>().material = _materialVariant2;
                    _spawnedPiece.transform.position += Vector3.forward * -0.2f;
                }

                boardPieces.Add(_spawnedPiece.transform);
            }
        }
    }
}
