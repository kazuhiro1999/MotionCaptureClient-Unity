using UnityEditor;
using UnityEngine;
using System;

public static class TimeUtil
{
    private static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

    // DateTime����UnixTime�֕ϊ�
    public static long GetUnixTime() {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    // UnixTime����DateTime�֕ϊ�
    public static DateTime GetDateTime(long unixTime) {
        return UnixEpoch.AddSeconds(unixTime);
    }
}