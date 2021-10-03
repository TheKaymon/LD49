using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoringZone : MonoBehaviour
{
    public TextMeshPro leftScore;
    public TextMeshPro rightScore;
    public Color idleColor;
    public Color scoringColor;
    public BoxCollider2D box;
    public int Score
    {
        get => points;
        set
        {
            points = value;
            leftScore.text = points.ToString();
            rightScore.text = points.ToString();
        }
    }
    private int points;

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
    public void SetScore( int score )
    {
        leftScore.text = score.ToString();
        rightScore.text = score.ToString();
    }

    public void Activate()
    {
        leftScore.color = scoringColor;
        rightScore.color = scoringColor;
    }

    public void Deactivate()
    {
        leftScore.color = idleColor;
        rightScore.color = idleColor;
    }
}
