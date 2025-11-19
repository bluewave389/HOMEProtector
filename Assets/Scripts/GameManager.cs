using UnityEngine;
using TMPro; // UI 표시용 (TextMeshPro 사용 가정)

public class GameManager : Singleton<GameManager>
{

    [Header("References")]
    public PlayerStatsManager playerStats;

    [Header("Time Settings")]
    // 실제 1분(60초) = 게임 6시간
    // 실제 1초 = 게임 0.1시간 (6 / 60)
    private const float GameHoursPerRealSec = 0.1f;

    [Range(1, 4)]
    public float timeSpeedMultiplier = 1.0f; // 현재 배속 (1, 2, 4)

    [Header("Current Game Time")]
    public int Day = 1;
    public float Hour = 8.0f; // 아침 8시 시작 가정

    private bool isGamePaused = false;

    public override void Awake()
    {
        base.Awake();

        // 프레임 속도 설정 (모바일 배터리 절약)
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        if (playerStats != null)
        {
            playerStats.Initialize();
        }
    }

    private void Update()
    {
        if (isGamePaused) return;

        // 1. 흐른 시간 계산
        // Time.deltaTime(지난 프레임 시간) * 기본비율 * 배속
        float passedGameHours = Time.deltaTime * GameHoursPerRealSec * timeSpeedMultiplier;

        // 2. 시간 갱신
        Hour += passedGameHours;

        // 3. 스탯 매니저에게 시간 경과 알림 (배고픔, 수면 감소)
        if (playerStats != null)
        {
            playerStats.ProcessTimeDecay(passedGameHours);
        }

        // 4. 날짜 변경 체크
        if (Hour >= 24.0f)
        {
            Hour -= 24.0f;
            Day++;
            CheckDailyEvents(); // 일일 이벤트 체크
        }
    }

    // --- UI 버튼 연결용 메서드 ---

    public void SetSpeed1x() { timeSpeedMultiplier = 1.0f; Debug.Log("속도: 1배"); }
    public void SetSpeed2x() { timeSpeedMultiplier = 2.0f; Debug.Log("속도: 2배"); }
    public void SetSpeed4x() { timeSpeedMultiplier = 4.0f; Debug.Log("속도: 4배"); }

    // 시간을 강제로 흐르게 함 (아르바이트, 공부 등)
    public void SkipTime(float hoursToSkip)
    {
        // 스킵하는 동안의 스탯 감소도 적용해야 함
        float totalDecayLoop = 0;

        // 한꺼번에 너무 큰 값을 넘기면 계산이 튈 수 있으므로 반복문이나 수식 처리
        // 여기선 단순하게 처리하되, 스킵 시 스탯 감소 로직은 기획에 따라 다를 수 있음.
        // (보통 스킵 시엔 배고픔이 더 빨리 닳거나 하는 식)

        Hour += hoursToSkip;
        playerStats.ProcessTimeDecay(hoursToSkip); // 스킵한 시간만큼 배고픔/수면 감소

        while (Hour >= 24.0f)
        {
            Hour -= 24.0f;
            Day++;
            CheckDailyEvents();
        }
    }

    private void CheckDailyEvents()
    {
        Debug.Log($"Day {Day} 시작! 주간/월간 이벤트 체크 로직이 여기에 들어갑니다.");
        // TODO: 주간/월간 이벤트 로직 호출
    }

    public void TriggerGameOver(string reason)
    {
        isGamePaused = true;
        timeSpeedMultiplier = 0f;
        Debug.Log($"게임 오버: {reason}");
        // TODO: 게임 오버 UI 팝업
    }
}