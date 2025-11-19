using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SleepPopup : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI sleepTimeText; // "7 시간" 표시할 텍스트
    public Button upButton;
    public Button downButton;
    public Button sleepButton;
    public Button closeButton; // 팝업 닫기(X) 버튼

    [Header("Settings")]
    private int currentTargetHour = 7; // 기본 7시간
    private const int MinSleep = 1;
    private const int MaxSleep = 24;

    private GameUIManager uiManager; // 스킵 연출 요청용

    private void Start()
    {
        // GameUIManager 찾기 (부모나 씬에서)
        uiManager = FindObjectOfType<GameUIManager>();

        // 버튼 리스너 연결
        upButton.onClick.AddListener(() => AdjustTime(1));
        downButton.onClick.AddListener(() => AdjustTime(-1));
        sleepButton.onClick.AddListener(ConfirmSleep);
        closeButton.onClick.AddListener(ClosePopup);

        // 시작 시 팝업 닫아두기
        gameObject.SetActive(false);
    }

    public void OpenPopup()
    {
        currentTargetHour = 7; // 열 때마다 7시간으로 초기화
        UpdateText();
        gameObject.SetActive(true);
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }

    private void AdjustTime(int amount)
    {
        currentTargetHour += amount;
        // 시간 범위 제한 (Clamp)
        if (currentTargetHour < MinSleep) currentTargetHour = MinSleep;
        if (currentTargetHour > MaxSleep) currentTargetHour = MaxSleep;

        UpdateText();
    }

    private void UpdateText()
    {
        sleepTimeText.text = $"{currentTargetHour} : 00";
    }

    private void ConfirmSleep()
    {
        // 1. 팝업 닫기
        ClosePopup();

        // 2. UI 매니저에게 '수면' 스킵 연출 요청
        if (uiManager != null)
        {
            uiManager.StartCoroutine(uiManager.ProcessTimeSkipEffect(1.5f, "수면 중... zZZ", () =>
            {
                // 3. 연출 도중(화면 암전 시) 실행될 실제 로직

                // 시간 스킵 (이 동안 배고픔은 감소함)
                GameManager.I.SkipTime(currentTargetHour);

                // 수면 회복 로직
                // 예: 1시간당 15 회복 (7시간이면 105로 완충)
                int recoveryAmount = currentTargetHour * 15;

                // PlayerStatsManager에 접근하여 수면 회복
                GameManager.I.playerStats.RestoreStat("Sleep", recoveryAmount);

                // (선택) 의지력도 소폭 회복?
                GameManager.I.playerStats.RestoreStat("WillPower", 5);
            }));
        }
    }
}