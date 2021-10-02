using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool paused = false;

    public static GameManager instance;
    public static Level level;

    // Start is called before the first frame update
    void Awake()
    {
        if ( instance != null )
            Debug.Log("Multiple GameManagers!");
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerDeath()
    {

    }

    public void LevelEnd()
    {

    }
}
