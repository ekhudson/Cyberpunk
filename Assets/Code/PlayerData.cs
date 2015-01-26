using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerData 
{
    public string PlayerName = "New Player";
    private int mCurrentScore = 0;
    private bool mInvertYAxis = false;

    public PlayerData (string name)
    {
        PlayerName = name;
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
