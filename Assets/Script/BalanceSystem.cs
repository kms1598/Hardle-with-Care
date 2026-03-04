using UnityEngine;
using UnityEngine.UI;
using static SoundManager;

public class BalanceSystem : MiniGameBase
{
    [SerializeField] RectTransform grid;
    [SerializeField] Image outline;
    private float warningRadius = 120f;
    private float dangerRadius = 180f;
    private float balanceLostRadius = 215f;

    private float defaultMoveStrength = 30f;
    private float controlStrength = 50f;
    private int lostCargo = 0;
    private float cargoStrength = 50f;

    private float maxDirectionChangeTime = 5f;
    private float minDirectionChangeTime = 2f;
    private float directionChangeTimer;
    private Vector2 pushVec = Vector2.zero;

    private void OnEnable()
    {
        base.OnEnable();
        EventManager.OnCargoUnitBroke += UpdateCargoCount;
    }

    public override void Init()
    {
        base.Init();
        directionChangeTimer = Random.Range(minDirectionChangeTime, maxDirectionChangeTime);
        pushVec = Random.insideUnitCircle.normalized;
        grid.anchoredPosition = Vector2.zero;
        lostCargo = 0;
    }

    public override void GameUpdate()
    {
        PushSpaceship();
        CheckBalanceLost();

        if (isPowerDown || isError) return;

        HandleSpaceship();
    }

    void PushSpaceship()
    {
        if(0 < directionChangeTimer)
        {
            directionChangeTimer -= Time.deltaTime;
            float moveStrength = defaultMoveStrength + (lostCargo * cargoStrength);
            grid.anchoredPosition += (pushVec * moveStrength) * Time.deltaTime;
        }
        else
        {
            directionChangeTimer = Random.Range(minDirectionChangeTime, maxDirectionChangeTime);
            pushVec = Random.insideUnitCircle.normalized;
        }
    }

    void HandleSpaceship()
    {
        float horizontal = 0;
        float vertical = 0;

        if (Input.GetKey(KeyCode.LeftArrow)) horizontal -= 1;
        if (Input.GetKey(KeyCode.RightArrow)) horizontal += 1;
        if (Input.GetKey(KeyCode.UpArrow)) vertical += 1;
        if (Input.GetKey(KeyCode.DownArrow)) vertical -= 1;

        Vector2 moveVec = new Vector2(horizontal, vertical).normalized;

        grid.anchoredPosition += (moveVec * controlStrength) * Time.deltaTime;
    }
    void CheckBalanceLost()
    {
        float distance = grid.anchoredPosition.magnitude;
        
        if (balanceLostRadius < distance)
        {
            EventManager.OnBalanceLost?.Invoke();
            grid.anchoredPosition = Vector2.zero;
            SoundManager.instance.PlaySFX(Conditions.Crash);
        }
        else if (dangerRadius < distance)
        {
            outline.color = Color.red;
            grid.gameObject.GetComponent<Image>().color = Color.red;
        }
        else if(warningRadius < distance)
        {
            outline.color = Color.yellow;
            grid.gameObject.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            outline.color = Color.white;
            grid.gameObject.GetComponent<Image>().color = Color.white;
        }
    }

    void UpdateCargoCount(int count)
    {
        lostCargo = count;
    }
}
