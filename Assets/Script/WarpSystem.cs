using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static SoundManager;

public class WarpSystem : MiniGameBase
{
    [SerializeField] private RectTransform targetPosition;
    [SerializeField] private RectTransform spaceshipPosition;
    [SerializeField] private Image spaceshipIamge;
    private float maxX = 250f;
    private float maxY = 180f;
    private float radius = 40f;
    private float moveStrength = 100f;

    private float minFindWarpTime = 5f;
    private float maxFindWarpTime = 15f;
    private float findInterval;
    private Coroutine findWarp;

    float holdTime;

    public override void Init()
    {
        base.Init();
        holdTime = 0;
        targetPosition.gameObject.SetActive(false);
        spaceshipIamge.fillAmount = 0;
        spaceshipPosition.gameObject.SetActive(false);
        findInterval = Random.Range(minFindWarpTime, maxFindWarpTime);
        findWarp = null;
    }

    private void PrepareWarp()
    {
        spaceshipPosition.gameObject.SetActive(true);

        int spawnTargetTry = 30;
        bool isSpawned = false;
        Vector2 randomVec = Vector2.zero;

        while(0 < spawnTargetTry && !isSpawned)
        {
            spawnTargetTry--;
            float randomX = Random.Range(-maxX, maxX);
            float randomY = Random.Range(-maxY, maxY);
            randomVec = new Vector2(randomX, randomY);

            float distance = Vector2.Distance(randomVec, spaceshipPosition.anchoredPosition);

            if(distance > radius * 5)
            {
                isSpawned = true;
            }
        }

        targetPosition.anchoredPosition = randomVec;
        targetPosition.gameObject.SetActive(true);
    }

    private IEnumerator WarpCoroutine()
    {
        yield return new CustomWaitForSeconds(findInterval, () => isError);
        PrepareWarp();
    }

    public override void GameUpdate()
    {
        if(!isError)
        {
            if(findWarp == null) findWarp = StartCoroutine(WarpCoroutine());
            moveSpaceship();
            CheckWarpLogic();
        }
    }

    void moveSpaceship()
    {
        Vector2 moveInput = Vector2.zero;

        if(Input.GetKey(KeyCode.W)) moveInput.y += 1f;
        if (Input.GetKey(KeyCode.A)) moveInput.x -= 1f;
        if (Input.GetKey(KeyCode.S)) moveInput.y -= 1f;
        if(Input.GetKey(KeyCode.D)) moveInput.x += 1f;
        if(Input.GetKey(KeyCode.Q)) moveInput = Vector2.zero;

        moveInput.Normalize();
        Vector2 currentPos = spaceshipPosition.anchoredPosition;
        currentPos += moveInput * moveStrength * Time.deltaTime;

        currentPos.x = Mathf.Clamp(currentPos.x, -maxX, maxX);
        currentPos.y = Mathf.Clamp(currentPos.y, -maxY, maxY);

        spaceshipPosition.anchoredPosition = currentPos;
    }

    private void CheckWarpLogic()
    {
        float distance = Vector2.Distance(spaceshipPosition.anchoredPosition, targetPosition.anchoredPosition);
        float requiredHoldTime = 3f;

        if (distance <= radius)
        {
            targetPosition.gameObject.GetComponent<Image>().color = Color.yellow;
            if (Input.GetKey(KeyCode.Q))
            {
                holdTime += Time.deltaTime;
                spaceshipIamge.fillAmount = holdTime / requiredHoldTime;

                if (holdTime >= requiredHoldTime)
                {
                    EventManager.OnWarp?.Invoke();
                    SoundManager.instance.PlaySFX(Conditions.Warp);
                    Init();
                }
            }
            else
            {
                spaceshipIamge.fillAmount = 0;
                holdTime = 0;
            }
        }
        else
        {
            targetPosition.gameObject.GetComponent<Image>().color = Color.white;
        }
    }
}
