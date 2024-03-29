using UnityEditor;
using UnityEngine;
using System;

public static class TimeUtil
{
    private static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

    // DateTimeからUnixTimeへ変換
    public static long GetUnixTime() {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    // UnixTimeからDateTimeへ変換
    public static DateTime GetDateTime(long unixTime) {
        return UnixEpoch.AddSeconds(unixTime);
    }
}