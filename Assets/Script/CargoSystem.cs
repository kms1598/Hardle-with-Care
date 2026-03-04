using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SoundManager;

public class CargoSystem : MiniGameBase
{
    [System.Serializable]
    public class CargoUnit
    {
        public KeyCode keyIndex;
        public Image gauge;
        public Image charge;
        public TextMeshProUGUI gaugeText;

        public int maxGauge = 10;
        public int currentGauge;
        public bool isBroken = false;

        public float maxTime = 10f;
        public float minTime = 5f;
        public float currentTimer;

        public float holdTime = 1f;
        public float holdTimer;
    }

    [SerializeField] CargoUnit[] cargoUnits = new CargoUnit[6];
    private int brokenUnit;
    public override void Init()
    {
        base.Init();
        brokenUnit = 0;
        foreach (var cargoUnit in cargoUnits)
        {
            cargoUnit.charge.fillAmount = 0f;
            cargoUnit.currentGauge = cargoUnit.maxGauge;
            cargoUnit.currentTimer = Random.Range(cargoUnit.minTime, cargoUnit.maxTime);
            cargoUnit.holdTimer = 0f;
        }
    }

    public override void GameUpdate()
    {
        if(isPowerDown) return;

        UpdateCargoUnit();

        if (!isError) ControlCargoUnit();
        else
        {
            foreach (var cargoUnit in cargoUnits)
            {
                cargoUnit.holdTimer = 0f;
            }
        }
    }

    private void UpdateCargoUnit()
    {
        foreach(var cargoUnit in cargoUnits)
        {
            if(!cargoUnit.isBroken && cargoUnit.currentTimer <= 0f)
            {
                cargoUnit.currentGauge--;
                if (cargoUnit.currentGauge <= 0)
                {
                    cargoUnit.isBroken = true;
                    brokenUnit++;
                    EventManager.OnCargoUnitBroke.Invoke(brokenUnit);
                    SoundManager.instance.PlaySFX(Conditions.CargoDown);
                }
                else
                {
                    cargoUnit.currentTimer = Random.Range(cargoUnit.minTime, cargoUnit.maxTime);
                }
                UpdateUI(cargoUnit);
            }

            cargoUnit.currentTimer -= Time.deltaTime;
        }
    }

    private void ControlCargoUnit()
    {
        foreach(var cargoUnit in cargoUnits)
        {
            if (Input.GetKey(cargoUnit.keyIndex))
            {
                cargoUnit.holdTimer += Time.deltaTime;
                cargoUnit.charge.fillAmount = cargoUnit.holdTimer / cargoUnit.holdTime;
                if (cargoUnit.holdTimer >= cargoUnit.holdTime)
                {
                    if(cargoUnit.isBroken)
                    {
                        cargoUnit.isBroken = false;
                        brokenUnit--;
                        EventManager.OnCargoUnitBroke.Invoke(brokenUnit);
                    }

                    cargoUnit.currentGauge = Mathf.Min(cargoUnit.currentGauge + 3, cargoUnit.maxGauge);
                    UpdateUI(cargoUnit);
                    cargoUnit.holdTimer = 0f;
                    cargoUnit.charge.fillAmount = 0;
                }
            }
            else
            {
                cargoUnit.holdTimer = 0f;
                cargoUnit.charge.fillAmount = 0;
            }
        }
    }

    private void UpdateUI(CargoUnit unit)
    {
        unit.gauge.fillAmount = (float)unit.currentGauge / unit.maxGauge;
        unit.gaugeText.text = unit.currentGauge.ToString();

        if (unit.currentGauge < 3)
        {
            unit.gauge.color = Color.red;
            unit.gaugeText.color = Color.red;
        }
        else
        {
            unit.gauge.color = Color.white;
            unit.gaugeText.color = Color.white;
        }
    }
}
