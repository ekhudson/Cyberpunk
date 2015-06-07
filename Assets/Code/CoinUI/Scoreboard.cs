using UnityEngine;
using System.Collections;

public class Scoreboard : MonoBehaviour 
{
    private static Vector2 sGUICoinPos = Vector2.zero;

    private void Start()
    {
        EventManager.Instance.AddHandler<CoinEvent>(CoinEventHandler);
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, 64f), GUI.skin.box);

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

        GUILayout.BeginHorizontal();

        Rect coinRect = GUILayoutUtility.GetRect(64f, 64f);

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = ScoreboardCoin.CoinCam.targetTexture;
        
        // Render the camera's view.
        ScoreboardCoin.CoinCam.Render();
        
        // Make a new texture and read the active Render Texture into it.
        Texture2D image = new Texture2D(ScoreboardCoin.CoinCam.targetTexture.width, ScoreboardCoin.CoinCam.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, ScoreboardCoin.CoinCam.targetTexture.width, ScoreboardCoin.CoinCam.targetTexture.height), 0, 0);
        image.Apply();
        
        // Replace the original active Render Texture.
        RenderTexture.active = currentRT;
       

        GUI.Button(coinRect,image);

        sGUICoinPos = GUIUtility.GUIToScreenPoint(coinRect.center);

        GUILayout.Label(tempText, GUI.skin.button, GUILayout.Height(64f));

        GUILayout.EndHorizontal();
    }

    public void CoinEventHandler(object sender, CoinEvent evt)
    {
        if (evt.CoinEventType == CoinEvent.CoinEventTypes.LANDED_FACE_UP)
        {
            PlayerManager.Instance.GetPlayer(0).AddToScore(evt.Coin.Value);
        }
    }
}
