using UnityEngine;
using System.Collections;

public class Scoreboard : MonoBehaviour 
{

    private void Start()
    {
        EventManager.Instance.AddHandler<CoinEvent>(CoinEventHandler);
    }


    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, 24f), GUI.skin.box);

        GUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        DrawScores();

        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();

        GUILayout.EndArea();
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

    public void CoinEventHandler(object sender, CoinEvent evt)
    {
        if (evt.CoinEventType == CoinEvent.CoinEventTypes.LANDED_FACE_UP)
        {
            PlayerManager.Instance.GetPlayer(0).AddToScore(evt.Coin.Value);
        }
    }
}
