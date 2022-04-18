using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamAchievement : MonoBehaviour
{


    private void OnEnable()
    {
        if (!SteamManager.Initialized)
            return;

        
    }

    public static void UnlockAchievement(string id)
    {
        if (!SteamManager.Initialized)
            return;

        bool gotAchiev;
        SteamUserStats.GetAchievement(id, out gotAchiev);

        if (!gotAchiev)
        {
            SteamUserStats.SetAchievement(id);
            SteamUserStats.StoreStats();

        }

    }

    public static void UnlockAchievementValue(string id, int value)
    {
        if (!SteamManager.Initialized)
            return;

        SteamUserStats.GetStat(id, out int statValue);

        if(statValue < value)
        {
            SteamUserStats.SetStat(id, value);
            SteamUserStats.StoreStats();
        }

    }
}
