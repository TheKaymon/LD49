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
    private List<ScoringZone> scoringZones;
    private IEnumerator checkCoroutine;
    private int stage = 0;
    private List<Piece> countedPieces;
    private int levelScore;
    //private int totalScore;

    private void Awake()
    {
        GameManager.level = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        checkCoroutine = CheckForMotion();
        scoringZones = new List<ScoringZone>();
        countedPieces = new List<Piece>();
        int points = 1;
        for ( int i = 0; i < scoringZoneParent.childCount; i++ )
        {
            scoringZones.Add(scoringZoneParent.GetChild(i).GetComponent<ScoringZone>());
            scoringZones[i].Score = points;
            points *= 2;
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

    public void PieceDestroyed( Piece p )
    {
        loadZone.PieceDestroyed(p);
        levelScore--;
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
        int lastType = -1;
        int type;
        int lastIndex = currentLevel.levelStages[stage].lastBlockAvailable;
        int typeCount = lastIndex + 1;
        for ( int i = 0; i < currentLevel.levelStages[stage].numBlocks; i++ )
        {
            type = Random.Range(0, typeCount);
            if( type == lastType )
            {
                type = (type + Random.Range(0, lastIndex )) % typeCount;
            }
            lastType = type;
            pieces.Add(pieceTypes[type]);
            //Debug.Log($"Adding piece {pieceTypes[type]}");
        }
        // Set up Loading Zone
        loadZone.dropInterval = currentLevel.levelStages[stage].dropInterval;
        loadZone.dropGravity = currentLevel.levelStages[stage].dropGravity;
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
        scoreText.SetText($"Score: {levelScore}");
        levelEndUI.SetActive(true);
        Debug.Log($"Level Score is: {levelScore}");
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
            movement |= ( loadZone.lastBeam.GetComponent<Rigidbody2D>().velocity.sqrMagnitude > 0.01f
                            || loadZone.lastBeam.GetComponent<Rigidbody2D>().angularVelocity > 0.1f );
            if ( !movement )
                yield return StartCoroutine(CalculateRoundScore());
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator CalculateRoundScore()
    {
        Debug.Log("Calculating Round Score");
        Piece p;
        Piece last = null;
        int hits = 0;
        // TODO: Set Cap to Match Level Pieces
        Collider2D[] colliders = new Collider2D[32];
        // Setup Filter
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(pieceMask);
        //filter.NoFilter();
        Vector3 pos;
        PopupText text;

        for ( int i = 0; i < scoringZones.Count; i++ )
        {
            // Get Hits
            hits = scoringZones[i].box.OverlapCollider(filter, colliders);

            // Turn Off Last Zone Highlight
            if ( i > 0 )
                scoringZones[i - 1].Deactivate();
            // Turn On Current Zone Highlight
            if ( hits > 0 )
            {
                scoringZones[i].Activate();

                //Debug.Log($"{hits} Hits!");
                for ( int h = 0; h < hits; h++ )
                {
                    if ( colliders[h] != null )
                    {
                        p = colliders[h].GetComponent<Piece>();
                        if ( !countedPieces.Contains(p) )
                        {
                            if ( last != null )
                                last.ToggleOutline(false);

                            levelScore += scoringZones[i].Score;
                            countedPieces.Add(p);
                            p.gameObject.layer = 7;
                            p.ToggleOutline(true);
                            last = p;

                            pos = p.transform.position;
                            pos.z = -1;
                            text = Instantiate(popupTextPrefab, pos, Quaternion.identity);
                            text.Initialize(scoringZones[i].Score);

                            //Debug.Log($"{score} scored from {p}");
                        }
                    }

                    yield return new WaitForSeconds(0.25f);
                }

                if ( last != null )
                    last.ToggleOutline(false);

                yield return new WaitForSeconds(1f);
            }


        }
        // Turn off Scoring Zone
        scoringZones[scoringZones.Count - 1].Deactivate();

        yield return StartCoroutine(MoveCamera());
    }

    private IEnumerator MoveCamera()
    {
        Debug.Log("Moving Camera");
        float yTarg = loadZone.lastBeam.GetComponent<BoxCollider2D>().bounds.min.y + 5.5f;
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
