using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    public static Level instance;

    public Camera mainCam;
    public Player player;
    public AudioClip pointsScored;
    public LoadingDock loadZone;
    public Transform scoringZoneParent;
    public LayerMask pieceMask;
    public TextMeshProUGUI gameOverScore;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI stageText;
    public TextMeshProUGUI livesText;
    public List<Piece> pieceTypes;
    public LevelSetup levelSettings;
    public PopupText popupTextPrefab;
    public GameObject titleScreen;
    public TextMeshProUGUI titleText;
    public GameObject startButton;
    public GameObject pauseGroup;
    public GameObject audioPanel;
    public GameObject gameOverUI;
    private List<ScoringZone> scoringZones;
    private IEnumerator checkCoroutine;
    private int stage = 0;
    private int lives = 10;
    private List<Piece> countedPieces;
    private int levelScore;
    //private int totalScore;
    private float pitchShift = 0.25f;

    public bool paused = true;

    private void Awake()
    {
        instance = this;

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

        if ( !levelSettings.showMainMenu )
            StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetButtonDown("Cancel"))
        {
            if ( paused )
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void StartGame()
    {
        // Hide Main Menu
        titleScreen.SetActive(false);
        titleText.SetText("Resume");
        startButton.SetActive(false);
        pauseGroup.SetActive(true);
        // Show Side Stuff
        paused = true;
        StopAllCoroutines();
        loadZone.ClearPieces();
        levelScore = 0;
        lives = 10;
        stage = 0;
        countedPieces.Clear();
        paused = false;
        StartStage();
    }

    public void PauseGame()
    {
        paused = true;
        Time.timeScale = 0;
        titleScreen.SetActive(true);
    }

    public void ResumeGame()
    {
        paused = false;
        Time.timeScale = 1;
        titleScreen.SetActive(false);
    }

    public void RestartGame( bool showMainMenu )
    {
        // WebGL
        //levelSettings.showMainMenu = showMainMenu;
        //Time.timeScale = 1;
        //SceneManager.LoadScene(0);

        //Windows
        if ( !showMainMenu )
        {
            levelSettings.showMainMenu = showMainMenu;
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        }
        else
            Application.Quit();
    }

    public void PieceGrabbed( Piece p )
    {
        levelScore++;
        scoreText.SetText(levelScore.ToString());
        PopupText text = Instantiate(popupTextPrefab, p.transform.position, Quaternion.identity);
        text.Initialize(1);
        Audio.instance.PlaySFX(pointsScored, transform.position);
    }

    public void PieceDestroyed( Piece p )
    {
        loadZone.PieceDestroyed(p);
        levelScore--;
        scoreText.SetText(levelScore.ToString());
        lives--;
        livesText.SetText(lives.ToString());

        if ( lives <= 0 )
        {
            GameOver("Game Over");
        }
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
        int lastIndex = levelSettings.levelStages[stage].lastBlockAvailable;
        //List<Piece> available = new List<Piece>();
        //int typeCount = lastIndex + 1;
        int type = Random.Range(0, lastIndex + 1);
        //int times = ( levelSettings.levelStages[stage].numBlocks / typeCount ) + 1;

        for ( int i = 0; i < levelSettings.levelStages[stage].numBlocks; i++ )
        {
            pieces.Add(pieceTypes[type]);
            lastType = type;
            //int rand = Random.Range(1, lastIndex);
            //type = ( type + rand ) % lastIndex;
            type = ( type + Random.Range(1, lastIndex) ) % lastIndex;
            //Debug.Log($"Generated {type} from {lastType} and {rand}");
            //if ( type == lastType )
            //{
            //    type = ( type + Random.Range(0, lastIndex) ) % typeCount;
            //}
            //lastType = type;

            //Debug.Log($"Adding piece {pieceTypes[type]}");
        }

        //for ( int i = 0; i < levelSettings.levelStages[stage].numBlocks; i++ )
        //{
        //    type = Random.Range(0, typeCount);
        //    if( type == lastType )
        //    {
        //        type = (type + Random.Range(0, lastIndex )) % typeCount;
        //    }
        //    lastType = type;
        //    pieces.Add(pieceTypes[type]);
        //    //Debug.Log($"Adding piece {pieceTypes[type]}");
        //}

        // Set up Loading Zone
        loadZone.dropInterval = levelSettings.levelStages[stage].dropInterval;
        loadZone.dropGravity = levelSettings.levelStages[stage].dropGravity;
        loadZone.LoadPieces(pieces);
        stageText.SetText((stage + 1).ToString());
        player.paused = false;
    }

    private void GameOver( string message )
    {
        paused = true;
        Time.timeScale = 0;
        gameOverScore.SetText($"Score: {levelScore}");
        gameOverUI.SetActive(true);
        Debug.Log($"Level Score is: {levelScore}");
    }

    private void EndStage()
    {
        StopAllCoroutines();

        stage++;

        if ( stage >= levelSettings.levelStages.Count )
        {
            GameOver("Game Complete!");
        }
        else
            StartStage();
    }

    private IEnumerator CheckForMotion()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("Checking for Motion");
        while ( true )
        {
            if ( !paused )
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
            }
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
        float pitch = 1f;

        for ( int i = 0; i < scoringZones.Count; i++ )
        {
            if ( !paused )
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
                                p.scored = true;
                                p.ToggleOutline(true);
                                last = p;

                                pos = p.transform.position;
                                pos.z = -1;
                                text = Instantiate(popupTextPrefab, pos, Quaternion.identity);
                                text.Initialize(scoringZones[i].Score);
                                Audio.instance.PlaySFX(pointsScored, transform.position);
                                pitch += pitchShift;

                                scoreText.SetText(levelScore.ToString());
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
            else
                yield return new WaitForSeconds(0.25f);
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
            if ( !paused )
            {
                timer += Time.deltaTime;

                if ( timer > 1f )
                {
                    mainCam.transform.position = new Vector3(0, yTarg, -10);
                    EndStage();
                }
                else
                {
                    float y = Mathf.Lerp(yStart, yTarg, timer);
                    mainCam.transform.position = new Vector3(0, y, -10);
                }
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
