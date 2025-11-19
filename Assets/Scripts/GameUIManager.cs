using UnityEngine;
using UnityEngine.UI; // Slider 사용
using TMPro;          // TextMeshPro 사용
using System.Collections; // 코루틴 사용 (스킵 연출)

public class GameUIManager : MonoBehaviour
{
    [Header("Managers")]
    public PlayerStatsManager playerStats;
    public PlayerStats_SO statsDefinition; // 최대값(Max) 참조용

    [Header("UI Elements - Stats")]
    public Slider hungerSlider;
    public Slider sleepSlider;
    public Slider happinessSlider;
    public Slider willPowerSlider;
    public TextMeshProUGUI moneyText;

    [Header("UI Elements - Time/System")]
    public TextMeshProUGUI timeText;     // 예: "Day 1 - 13:00"
    public GameObject pausePanel;        // 일시정지 시 뜰 반투명 검은 배경
    public GameObject skipDimPanel;      // 시간 스킵 시 화면 가릴 패널 (검은색 이미지)
    public TextMeshProUGUI skipText;     // 스킵 중 표시할 텍스트 ("아르바이트 중...")

    private void Start()
    {
        // 게임 시작 시 UI 갱신
        UpdateAllStatUI();

        // PlayerStatsManager의 이벤트에 리스너 등록
        // (스탯이 변할 때마다 UpdateAllStatUI 함수가 자동으로 실행됨)
        playerStats.OnStatsChanged.AddListener(UpdateAllStatUI);

        // 패널 초기화
        if (pausePanel) pausePanel.SetActive(false);
        if (skipDimPanel) skipDimPanel.SetActive(false);
    }

    private void Update()
    {
        // 시간 텍스트는 매 프레임 갱신 (GameManager에서 시간 가져오기)
        UpdateDayTimeUI();
    }

    // --- 1. 스탯 UI 업데이트 로직 ---
    public void UpdateAllStatUI()
    {
        // 슬라이더: 현재값 / 최대값 (0.0 ~ 1.0 사이 비율)
        if (hungerSlider) hungerSlider.value = playerStats.CurrentHunger / statsDefinition.maxHunger;
        if (sleepSlider) sleepSlider.value = playerStats.CurrentSleep / statsDefinition.maxSleep;
        if (happinessSlider) happinessSlider.value = (float)playerStats.CurrentHappiness / statsDefinition.maxHappiness;
        if (willPowerSlider) willPowerSlider.value = (float)playerStats.CurrentWillPower / statsDefinition.maxWillPower;

        // 돈 텍스트: 천 단위 콤마 포맷 (N0)
        if (moneyText) moneyText.text = $"$ {playerStats.CurrentMoney:N0}";
    }

    private void UpdateDayTimeUI()
    {
        // GameManager의 시간을 가져와서 "00:00" 형식으로 변환
        float hour = GameManager.I.Hour;
        int day = GameManager.I.Day;

        // 13.5시 -> 13시 30분 처럼 보이게 하거나 그냥 13시로 표기
        // 여기선 깔끔하게 정수 시간만 표기
        timeText.text = $"Day {day}\n{(int)hour}:00";
    }

    // --- 2. 일시정지 & 스킵 연출 ---

    public void TogglePauseUI(bool isPaused)
    {
        if (pausePanel) pausePanel.SetActive(isPaused);
    }

    // 시간 스킵 코루틴 (외부에서 호출: StartCoroutine(uiManager.ProcessTimeSkip(...)))
    public IEnumerator ProcessTimeSkipEffect(float skipDuration, string actionName, System.Action onSkipComplete)
    {
        // 1. 화면 암전 (Fade Out)
        if (skipDimPanel) skipDimPanel.SetActive(true);
        if (skipText)
        {
            skipText.text = actionName;
            skipText.gameObject.SetActive(true);
        }

        // 2. 대기 (연출 시간, 실제 시간 1~2초 정도)
        yield return new WaitForSeconds(1.5f);

        // 3. 실제 데이터 처리 (콜백 함수 실행 - 여기서 시간 스킵 로직 수행)
        onSkipComplete?.Invoke();

        // 4. UI 갱신 (스킵 후 변경된 스탯 반영)
        UpdateAllStatUI();

        // 5. 화면 밝아짐 (Fade In)
        if (skipText) skipText.gameObject.SetActive(false);
        if (skipDimPanel) skipDimPanel.SetActive(false);
    }
}