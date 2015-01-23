using UnityEngine;
using System.Collections;

public class Scoreboard : MonoBehaviour 
{
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        DrawScores();

        GUILayout.EndHorizontal();
    }

    private void DrawScores()
    {


        int numPlayers = PlayerManager.Instance.GetPlayerCount();

        for (int i = 0; i < numPlayers; i++)
        {
            DrawPlayerScorecard(i);
        }
    }

    private void DrawPlayerScorecard(int playerIndex)
    {
        PlayerData playerData = PlayerManager.Instance.GetPlayer(playerIndex);

        string tempText = string.Format("{0}: {1}", playerData.PlayerName, playerData.GetCurrentScore().ToString());
        
        GUILayout.Label(tempText, GUI.skin.button);
    }
}
