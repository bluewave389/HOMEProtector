using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStats_SO : ScriptableObject
{
    [Header("--- Basic Stats (Max Values) ---")]
    public int maxHunger = 100;
    public int maxSleep = 100;
    public int maxHappiness = 100;
    public int maxWillPower = 100;

    [Header("--- Initial Values (Start of Game) ---")]
    public int initialHunger = 80;
    public int initialSleep = 80;
    public int initialHappiness = 50;
    public int initialWillPower = 50;
    public int initialMoney = 100;

    [Header("--- Decay Rates (Per Game Hour) ---")]
    public float baseHungerDecayRate = 2.0f; // 게임 시간 1시간당 배고픔 감소량
    public float baseSleepDecayRate = 1.5f;  // 게임 시간 1시간당 수면 감소량
}
