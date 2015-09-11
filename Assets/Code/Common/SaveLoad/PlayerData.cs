using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerData 
{
    public string PlayerName = "New Player";
    private int mCurrentScore = 0;
    private bool mInvertYAxis = false;
    
    public PlayerData (string name, int score)
    {
        PlayerName = name;
        mCurrentScore = score;
    }

    public int GetCurrentScore()
    {
        return mCurrentScore;
    }

    public void AddToScore(int amount)
    {
        mCurrentScore += amount;
    }
}
