using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SoundManager;


public class AISystem : MiniGameBase
{
    [SerializeField] TextMeshProUGUI logText;
    [SerializeField] TextMeshProUGUI codeText;
    [SerializeField] GameObject errorPanel;
    [SerializeField] Image[] life = new Image[3];

    [SerializeField] string[] logs;
    private int maxLogIines = 5;
    private float logPrintDelay = 0.2f;
    private Queue<string> logQueue = new Queue<string>();
    private float logTimer = 0f;

    private int codeLength = 4;
    private float maxMistakeCount = 3;
    private int currentMistakeCount = 0;
    public string targetCode = "";
    private int currentCodeIndex = 0;
    private bool isErrorOccur = false;

    private float minErrorInterval = 10f;
    private float maxErrorInterval = 20f;
    private float errorTimer = 0f;
    private float waitTimer = 0.5f;

    public override void Init()
    {
        base.Init();
        SetUpNormal();
        ResetErrorTimer();
    }
    protected override void PowerDown()
    {
        base.PowerDown();
        SetUpNormal();
        EventManager.OnErrorRecovery?.Invoke();
        SoundManager.instance.PlaySFX(Conditions.RightCode);
    }
    protected override IEnumerator RebootCoroutine()
    {
        errorPanel.gameObject.SetActive(false);
        yield return base.RebootCoroutine();
        SetUpNormal();
    }

    private void SetUpNormal()
    {
        waitTimer = 0.5f;
        rebootingText.gameObject.SetActive(false);
        isErrorOccur = false;
        logText.gameObject.SetActive(true);
        logText.text = "";
        currentMistakeCount = 0;
        currentCodeIndex = 0;
        errorPanel.SetActive(false);
        codeText.gameObject.SetActive(false);
        logQueue.Clear();
        foreach(var i in life)
        {
            i.color = Color.white;
        }
    }

    private void ResetErrorTimer()
    {
        errorTimer = UnityEngine.Random.Range(minErrorInterval, maxErrorInterval);
    }

    public override void GameUpdate()
    {
        if (isPowerDown) return;

        if(isErrorOccur)
        {
            if (0 < waitTimer)
            {
                waitTimer -= Time.deltaTime;
                return;
            }

            TypingCode();
        }
        else
        {
            TypingLog();

            errorTimer -= Time.deltaTime;
            if (errorTimer <= 0f)
            {
                TriggerError();
                ResetErrorTimer();
            }
        }
    }

    private void TypingCode()
    {
        string input = Input.inputString.ToUpper();

        foreach (char c in input)
        {
            if((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))
            {
                if (c == targetCode[currentCodeIndex])
                {
                    currentCodeIndex++;
                    UpdateUI();
                    if (currentCodeIndex >= targetCode.Length)
                    {
                        EventManager.OnErrorRecovery?.Invoke();
                        SoundManager.instance.PlaySFX(Conditions.RightCode);
                        SetUpNormal();
                    }
                }
                else
                {
                    SoundManager.instance.PlaySFX(Conditions.WrongCode);
                    currentMistakeCount++;
                    currentCodeIndex = 0;

                    if (maxMistakeCount < currentMistakeCount)
                    {
                        EventManager.OnRebooting?.Invoke();
                        SoundManager.instance.PlaySFX(Conditions.OccuerError);
                        currentMistakeCount = 0;
                    }

                    GeneratedCode();
                }
            }
        }
    }

    private void TypingLog()
    {
        logTimer += Time.deltaTime;
        if(logPrintDelay < logTimer)
        {
            logTimer = 0f;
            string newLog = logs[UnityEngine.Random.Range(0, logs.Length)];
            logQueue.Enqueue(newLog);
            if(maxLogIines < logQueue.Count) logQueue.Dequeue();
            logText.text = string.Join("\n", logQueue);
        }
    }

    private void TriggerError()
    {
        isErrorOccur = true;
        logText.gameObject.SetActive(false);
        errorPanel.SetActive(true);
        codeText.gameObject.SetActive(true);
        EventManager.OnErrorOccur?.Invoke();
        SoundManager.instance.PlaySFX(Conditions.OccuerError);

        GeneratedCode();
    }

    private void GeneratedCode()
    {
        targetCode = "";
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        for (int i = 0; i < codeLength; i++)
        {
            targetCode += chars[UnityEngine.Random.Range(0, chars.Length)];
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        string correctPart = "<color=#00FF00>" + targetCode.Substring(0, currentCodeIndex) + "</color>";
        string remainingPart = "<color=#FF0000>" + targetCode.Substring(currentCodeIndex) + "</color>";
        codeText.text = correctPart + remainingPart;

        for(int i = 0; i < currentMistakeCount; i++) life[i].color = Color.red;
    }
}
