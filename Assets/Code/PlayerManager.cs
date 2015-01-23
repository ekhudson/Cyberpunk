using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : Singleton<PlayerManager> 
{
	private List<PlayerData> mPlayerList = new List<PlayerData>()
    {
        new PlayerData("ekhudson"),
    };

    public int GetPlayerCount()
    {
        return mPlayerList.Count;
    }

    public PlayerData GetPlayer(int index)
    {
        if (index > mPlayerList.Count)
        {
            return null;
        }

        return mPlayerList[index];
    }
}
