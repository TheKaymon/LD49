using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingDock : MonoBehaviour
{
    public Transform createAtLeft;
    public Transform createAtRight;
    //public PopupWarning leftWarning;
    //public PopupWarning rightWarning;
    public PopupWarning centerWarning;

    public Piece loadedPiece;
    public LoadingUI ui;
    public int PiecesRemaining => ( pieces.Count );
    public bool autoDrop = false;
    public bool alternateDrops = false;
    public float dropInterval = 5f;
    public float dropGravity = .15f;
    public GameObject beamPrefab;
    public GameObject lastBeam;
    private bool active = false;

    [HideInInspector]
    public List<Piece> droppedPieces;
    private List<Piece> activePieces;
    private float dropTimer = 0f;
    private bool lastDropLeft = false;
    private Queue<Piece> pieces;

    // Start is called before the first frame update
    void Start()
    {
        // Convert List to Queue
        pieces = new Queue<Piece>();
        activePieces = new List<Piece>();
        droppedPieces = new List<Piece>();

        ui.SetPieces(pieces.ToArray());

    }

    // Update is called once per frame
    void Update()
    {
        if ( active && autoDrop )
        {
            dropTimer += Time.deltaTime;

            if ( dropTimer > dropInterval )
            {
                GenerateNextPiece();
            }
        }
    }

    public void LoadPieces( List<Piece> p )
    {
        pieces = new Queue<Piece>(p);
        ui.SetPieces(pieces.ToArray());
        //activePieces.Clear();
        //droppedPieces.Clear();
        dropTimer = 0;
        active = true;
        //GenerateNextPiece();
        int direction = lastDropLeft ? 1 : -1;
        centerWarning.Initialize(dropInterval, direction);
    }

    public void PieceGrabbed( Piece p )
    {
        activePieces.Remove(p);
    }

    public void PieceDestroyed( Piece p )
    {
        activePieces.Remove(p);
        droppedPieces.Remove(p);
    }

    public void PieceDropped( Piece p )
    {
        droppedPieces.Add(p);
        if ( !autoDrop )
            GenerateNextPiece();
        //if ( PiecesRemaining <= 0 && activePieces.Count <= 0 )
        //{
            
        //}
    }

    public void GenerateNextPiece()
    {
        if ( pieces.Count > 0 )
        {
            int rotate = Random.Range(0, 4) * 90;
            Vector3 position = ( alternateDrops && lastDropLeft ) ? createAtRight.position : createAtLeft.position;
            if ( alternateDrops )
                lastDropLeft = !lastDropLeft;
            Quaternion rotation = Quaternion.Euler(0, 0, rotate);
            loadedPiece = Instantiate(pieces.Dequeue(), position, rotation);
            loadedPiece.currentAngle = rotate;
            loadedPiece.body2D.gravityScale = dropGravity;
            activePieces.Add(loadedPiece);

            dropTimer = ( pieces.Count > 0 ) ? 0 : - dropInterval;
            // Move Queued Sprites

            ui.SetPieces(pieces.ToArray());
            // Create UI Counter
            if ( pieces.Count > 0 )
            {
                if ( lastDropLeft )
                    centerWarning.Initialize(dropInterval, 1);
                else
                    centerWarning.Initialize(dropInterval, -1);
            }
            else
            {
                centerWarning.Initialize(dropInterval * 2, 0);
            }
        }
        else //if ( pieceCounter >= beamInterval )
        {
            lastBeam = Instantiate(beamPrefab, new Vector2(0, createAtLeft.position.y), Quaternion.identity);

            GameManager.level.player.SetControl(false);
            GameManager.level.BeamDropped();
            active = false;
        }
    }
   
}
