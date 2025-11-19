using UnityEngine;
using UnityEngine.Events;

public class PlayerStatsManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private PlayerStats_SO statsDefinition; // 인스펙터에서 SO 연결

    // 런타임 스탯 (외부에서 읽기만 가능, 수정은 메서드로)
    // 정밀한 감소 계산을 위해 내부적으로 float 사용
    public float CurrentHunger { get; private set; }
    public float CurrentSleep { get; private set; }
    public int CurrentHappiness { get; private set; }
    public int CurrentWillPower { get; private set; }
    public int CurrentMoney { get; private set; }

    [Header("Penalty Rates")]
    public float happinessPenaltyRate = 10.0f; // 배고픔/수면 부족 시 시간당 행복 감소량
    public float willPowerPenaltyRate = 15.0f; // 행복 부족 시 시간당 의지력 감소량

    // 취업 확률 (공부하면 증가)
    public float CurrentEmploymentChance { get; private set; }

    // 스탯 변경 시 UI 업데이트 등을 위한 이벤트 (옵션)
    public UnityEvent OnStatsChanged;

    // 초기화
    public void Initialize()
    {
        if (statsDefinition == null)
        {
            Debug.LogError("PlayerStats_SO가 연결되지 않았습니다!");
            return;
        }

        CurrentHunger = statsDefinition.initialHunger;
        CurrentSleep = statsDefinition.initialSleep;
        CurrentHappiness = statsDefinition.initialHappiness;
        CurrentWillPower = statsDefinition.initialWillPower;
        CurrentMoney = statsDefinition.initialMoney;
        CurrentEmploymentChance = 5.0f; // 기본 취업 확률 5%

        UpdateUI();
    }

    // GameManager에서 매 프레임 호출하여 시간 경과에 따른 스탯 감소 처리
    public void ProcessTimeDecay(float passedGameHours)
    {
        // 1. 기본 감소 (배고픔, 수면)
        DecreaseHunger(statsDefinition.baseHungerDecayRate * passedGameHours);
        DecreaseSleep(statsDefinition.baseSleepDecayRate * passedGameHours);

        // 2. 연쇄 감소 로직 (Penalty Logic)
        float currentHappinessDecay = 0f;

        // 배고픔이 0인가?
        if (CurrentHunger <= 0)
        {
            currentHappinessDecay += happinessPenaltyRate;
        }

        // 수면이 0인가?
        if (CurrentSleep <= 0)
        {
            currentHappinessDecay += happinessPenaltyRate;
        }

        // 행복 감소 적용
        if (currentHappinessDecay > 0)
        {
            DecreaseHappiness(currentHappinessDecay * passedGameHours);
        }

        // 3. 의지력 감소 로직 (행복이 0일 때)
        if (CurrentHappiness <= 0)
        {
            DecreaseWillPower(willPowerPenaltyRate * passedGameHours);
        }
    }

    


    // --- 스탯 조작 메서드들 ---

    public void DecreaseHunger(float amount)
    {
        CurrentHunger -= amount;
        if (CurrentHunger < 0) CurrentHunger = 0;
        // TODO: 배고픔이 0일 때 행복/의지력 추가 감소 로직 연결 가능
        UpdateUI();
    }

    public void DecreaseSleep(float amount)
    {
        CurrentSleep -= amount;
        if (CurrentSleep < 0) CurrentSleep = 0;
        UpdateUI();
    }
    
    public void DecreaseHappiness(float amount)
    {
        CurrentHappiness = Mathf.Max(0, CurrentHappiness - (int)amount);
        CheckGameOverCondition(); // 행복이 0이 되었으니 게임오버 체크
        UpdateUI(); // UI 갱신
    }
    
    public void DecreaseWillPower(float amount)
    {
        CurrentWillPower = Mathf.Max(0, CurrentWillPower - (int)amount);
        CheckGameOverCondition();
        UpdateUI();
    }

    // 밥 먹기, 잠자기 등으로 회복할 때 사용
    public void RestoreStat(string statName, int amount)
    {
        switch (statName)
        {
            case "Hunger":
                CurrentHunger = Mathf.Min(CurrentHunger + amount, statsDefinition.maxHunger);
                break;
            case "Sleep":
                CurrentSleep = Mathf.Min(CurrentSleep + amount, statsDefinition.maxSleep);
                break;
            case "Happiness":
                CurrentHappiness = Mathf.Min(CurrentHappiness + amount, statsDefinition.maxHappiness);
                break;
            case "WillPower":
                CurrentWillPower = Mathf.Min(CurrentWillPower + amount, statsDefinition.maxWillPower);
                break;
        }
        UpdateUI();
    }

    // 행동에 따른 소모 (행복, 의지력)
    public void ConsumeActionStats(int happinessCost, int willPowerCost)
    {
        CurrentHappiness = Mathf.Max(0, CurrentHappiness - happinessCost);
        CurrentWillPower = Mathf.Max(0, CurrentWillPower - willPowerCost);

        CheckGameOverCondition();
        UpdateUI();
    }

    public void ModifyMoney(int amount)
    {
        CurrentMoney += amount;
        // 돈은 마이너스가 될 수 있다면 조건문 제거, 0 이하라면 아래 유지
        // if (CurrentMoney < 0) CurrentMoney = 0; 
        UpdateUI();
    }

    public void IncreaseEmploymentChance(float amount)
    {
        CurrentEmploymentChance += amount;
        Debug.Log($"취업 확률 증가! 현재 확률: {CurrentEmploymentChance}%");
    }

    private void CheckGameOverCondition()
    {
        if (CurrentHappiness <= 0 && CurrentWillPower <= 0)
        {
            GameManager.I.TriggerGameOver("의지력과 행복이 모두 바닥나서 아무것도 할 수 없습니다...");
        }
    }

    private void UpdateUI()
    {
        OnStatsChanged?.Invoke(); // UI 매니저가 있다면 여기서 리스닝하여 갱신
    }
}