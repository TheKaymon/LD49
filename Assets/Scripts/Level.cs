using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Level : MonoBehaviour
{
    public Camera mainCam;
    public Player player;
    public LoadingDock loadZone;
    public Transform scoringZoneParent;
    public LayerMask pieceMask;
    public GameObject levelEndUI;
    public TextMeshProUGUI scoreText;
    public List<Piece> pieceTypes;
    public LevelSetup currentLevel;
    public PopupText popupTextPrefab;
    private List<BoxCollider2D> scoringZones;
    private IEnumerator checkCoroutine;
    private int stage = 0;
    private List<Piece> countedPieces;

    private void Awake()
    {
        GameManager.level = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        checkCoroutine = CheckForMotion();
        scoringZones = new List<BoxCollider2D>();
        countedPieces = new List<Piece>();

        for ( int i = 0; i < scoringZoneParent.childCount; i++ )
        {
            scoringZones.Add(scoringZoneParent.GetChild(i).GetComponent<BoxCollider2D>());
        }

        StartStage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadLevel( LevelSetup level )
    {
        stage = 0;
        currentLevel = level;
        countedPieces.Clear();
        StartStage();
    }

    public void BeamDropped()
    {
        Debug.Log("Beam Dropped");
        //for ( int i = 0; i < loadZone.droppedPieces.Count; i++ )
        //{
        //    loadZone.droppedPieces[i].body2D.constraints = RigidbodyConstraints2D.None;
        //}
        StartCoroutine(checkCoroutine);
    }

    private void StartStage()
    {
        // Generate Pieces
        List<Piece> pieces = new List<Piece>();
        int type;
        for ( int i = 0; i < currentLevel.levelStages[stage]; i++ )
        {
            type = Random.Range(0, pieceTypes.Count);
            pieces.Add(pieceTypes[type]);
            //Debug.Log($"Adding piece {pieceTypes[type]}");
        }
        // Set up Loading Zone
        loadZone.LoadPieces(pieces);
        player.paused = false;
    }

    private void EndStage()
    {
        StopAllCoroutines();
        stage++;

        //CalculateScore();
        // Move Camera

        if ( stage < currentLevel.levelStages.Count )
        {
            StartStage();
        }
        else
        {
            CalculateScore();
        }
    }

    private void CalculateScore()
    {
        Piece p;
        List<Piece> countedPieces = new List<Piece>();
        int hits = 0;
        // TODO: Set Cap to Match Level Pieces
        Collider2D[] colliders = new Collider2D[32];
        // Setup Filter
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(pieceMask);
        //filter.NoFilter();
        int totalScore = 0;
        int score = 1;

        for ( int i = 0; i < scoringZones.Count; i++ )
        {
            hits = scoringZones[i].OverlapCollider(filter, colliders);
            //Debug.Log($"{hits} Hits!");
            for ( int h = 0; h < hits; h++ )
            {
                p = colliders[h].GetComponent<Piece>();
                if( !countedPieces.Contains(p) )
                {
                    totalScore += score;
                    countedPieces.Add(p);
                    Debug.Log($"{score} scored from {p}");
                }
            }

            score *= 2;
        }

        // Calculate Beam Score?

        scoreText.SetText($"Score: {totalScore}");
        levelEndUI.SetActive(true);
        Debug.Log($"Level Score is: {totalScore}");
    }

    private IEnumerator CheckForMotion()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("Checking for Motion");
        while ( true )
        {
            bool movement = false;
            for ( int i = 0; i < loadZone.droppedPieces.Count; i++ )
            {
                if ( loadZone.droppedPieces[i].body2D.velocity.sqrMagnitude > 0.01f
                    || loadZone.droppedPieces[i].body2D.angularVelocity > 0.1f )
                {
                    movement = true;
                }
            }
            if ( !movement )
                yield return StartCoroutine(CalculateRoundScore());
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator CalculateRoundScore()
    {
        Debug.Log("Calculating Round Score");
        Piece p;
        int hits = 0;
        // TODO: Set Cap to Match Level Pieces
        Collider2D[] colliders = new Collider2D[32];
        // Setup Filter
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(pieceMask);
        //filter.NoFilter();
        int totalScore = 0;
        int score = 1;
        Vector3 pos;
        PopupText text;

        for ( int i = 0; i < scoringZones.Count; i++ )
        {
            hits = scoringZones[i].OverlapCollider(filter, colliders);
            //Debug.Log($"{hits} Hits!");
            for ( int h = 0; h < hits; h++ )
            {
                if ( colliders[h] != null )
                {
                    p = colliders[h].GetComponent<Piece>();
                    if ( !countedPieces.Contains(p) )
                    {
                        totalScore += score;
                        countedPieces.Add(p);

                        pos = p.transform.position;
                        pos.z = -1;
                        text = Instantiate(popupTextPrefab, pos, Quaternion.identity);
                        text.Initialize(score);

                        //Debug.Log($"{score} scored from {p}");
                    }
                }

                yield return new WaitForSeconds(0.2f);
            }

            score *= 2;

            yield return new WaitForSeconds(0.3f);
        }

        yield return StartCoroutine(MoveCamera());
    }

    private IEnumerator MoveCamera()
    {
        Debug.Log("Moving Camera");
        float yTarg = loadZone.lastBeam.transform.position.y + 4.5f;
        float yStart = mainCam.transform.position.y;
        float timer = 0f;
        while ( true )
        {
            timer += Time.deltaTime;

            if( timer > 1f)
            {
                mainCam.transform.position = new Vector3(0, yTarg, -10);
                EndStage();
            }
            else
            {
                float y = Mathf.Lerp(yStart, yTarg, timer);
                mainCam.transform.position = new Vector3(0, y, -10);
            }

                
            yield return null;
        }
    }

    //private void OnTriggerEnter2D( Collider2D collider )
    //{
    //    //Debug.Log("Triggered Level End Zone!");
    //    //if ( collider.CompareTag("Piece") )
    //    //{
    //    //    collider.GetComponent<Piece>().Destruction(.5f);
    //    //}
    //    if ( collider.CompareTag("Player") )
    //    {
    //        if( player.currentPiece == null && loadZone.PiecesRemaining == 0 )
    //            EndLevel();
    //    }
    //}
}
