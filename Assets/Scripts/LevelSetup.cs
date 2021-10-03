using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level")]
public class LevelSetup : ScriptableObject
{
    [System.Serializable]
    public struct LevelStage
    {
        public int numBlocks;
        public int lastBlockAvailable;
        public float dropInterval;
        public float dropGravity;
    }

    public List<LevelStage> levelStages;
}
