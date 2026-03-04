using UnityEngine;
using UnityEngine.UI;
using static SoundManager;

public class StateSystem : MiniGameBase
{
    [SerializeField] private Slider powerSlider;

    private float maxPowerAmount = 100f;
    [SerializeField] private float currentPowerAmount = 100f;
    private float defaultUsePowerAmount = 5f;
    private float powerGenerateAmount = 10f;
    private float barrierUsePowerAmount = 5f;
    [SerializeField] private int onBarrier = 0;

    public override void Init()
    {
        base.Init();
        currentPowerAmount = maxPowerAmount;
        powerSlider.maxValue = currentPowerAmount;
    }
    private void OnEnable()
    {
        base.OnEnable();
        EventManager.OnBarrierChange += UpdateBarrierCount;
    }

    public override void GameUpdate()
    {
        if (!isError && Input.GetKeyDown(KeyCode.Space)) GeneratePower();

        if (!isPowerDown)
        {
            currentPowerAmount -= (defaultUsePowerAmount + onBarrier * barrierUsePowerAmount) * Time.deltaTime;
            powerSlider.value = currentPowerAmount;

            if (currentPowerAmount <= 0)
            {
                currentPowerAmount = 0;
                EventManager.OnPowerDown?.Invoke();
                SoundManager.instance.PlaySFX(Conditions.PowerDown);
            }
        }
        else
        {
            if(20 <= currentPowerAmount) EventManager.OnPowerRecover?.Invoke();
            SoundManager.instance.PlaySFX(Conditions.PowerRecovery);
        }
    }
    private void UpdateBarrierCount(int count)
    {
        onBarrier = count;
    }

    public void GeneratePower()
    {
        if (!isPowerDown) currentPowerAmount += powerGenerateAmount;
        else currentPowerAmount += powerGenerateAmount / 4;

        if (currentPowerAmount > maxPowerAmount)
        {
            currentPowerAmount = maxPowerAmount;
        }
    }
}
