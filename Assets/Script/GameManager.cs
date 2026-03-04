using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Events;
using System.Collections;

[System.Serializable]
public class DialogueText
{
    public string text;
    public bool isNeedChoice = false;
    public DialogueChoice[] choices;
}

[System.Serializable]
public class DialogueChoice
{
    public KeyCode targetKey;
    public UnityEvent onTargetKeyDown;
}

public class GameManager : MonoBehaviour
{
    static bool isHeardTutorial = false;
    private bool isGameStart = false;

    [SerializeField] DialogueText[] tutorialDialogues;
    [SerializeField] DialogueText[] readyDialogues;
    [SerializeField] DialogueText[] gameOverDialogues;
    [SerializeField] DialogueText[] clearDialogues;

    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] TextMeshProUGUI spaceText;

    [SerializeField] Slider durabilitySlider;
    [SerializeField] Slider distanceSlider;
    float maxDurability = 100f;
    float currentDurability = 100f;
    float targetDistance = 100f;
    float currentDistance = 0f;
    float movePerWarpDistance = 20f;

    [SerializeField] private List<MiniGameBase> miniGames = new List<MiniGameBase>();
    private void OnEnable()
    {
        EventManager.OnWarp += Warp;
        EventManager.OnRockCollision += Damage;
        EventManager.OnBalanceLost += Damage;
    }

    private void OnDisable()
    {
        EventManager.OnWarp -= Warp;
        EventManager.OnRockCollision -= Damage;
        EventManager.OnBalanceLost -= Damage;
    }

    void Start()
    {
        Init();
    }

    private void Init()
    {
        isGameStart = false;
        distanceSlider.value = 0;
        currentDurability = maxDurability;
        durabilitySlider.value = currentDurability;
        currentDistance = 0f;
        durabilitySlider.maxValue = maxDurability;
        distanceSlider.maxValue = targetDistance;

        foreach (var miniGame in miniGames)
        {
            miniGame.Init();
        }

        if (!isHeardTutorial) ShowTutorialDialogues();
        else ShowReadyDialogues();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) QuitGame();
        if (!isGameStart) return;

        foreach (var miniGame in miniGames)
        {
            miniGame.GameUpdate();
        }
    }
    
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator ShowDialogues(DialogueText[] dialogues)
    {
        dialoguePanel.SetActive(true);

        yield return null;

        int currentTextIndex = 0;

        while(currentTextIndex < dialogues.Length)
        {
            DialogueText currentText = dialogues[currentTextIndex];
            dialogueText.text = currentText.text;

            if (currentText.isNeedChoice)
            {
                if(currentText.choices[0].targetKey != KeyCode.Return) spaceText.gameObject.SetActive(false);

                foreach (var choice in currentText.choices)
                {
                    if (Input.GetKeyDown(choice.targetKey))
                    {
                        choice.onTargetKeyDown?.Invoke();
                        yield break;
                    }
                }
            }
            else
            {
                spaceText.gameObject.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    currentTextIndex++;
                    dialogueText.text = currentText.text;
                }
            }

            yield return null;
        }
    }
    private void Warp()
    {
        currentDistance += movePerWarpDistance;
        distanceSlider.value = currentDistance;

        if (targetDistance <= currentDistance)
        {
            ShowClearDialogues();
        }
    }

    private void Damage()
    {
        currentDurability -= 10;
        durabilitySlider.value = currentDurability;

        if (currentDurability <= 0)
        {
            ShowGameOverDialogues();
        }
    }

    public void ShowTutorialDialogues()
    {
        isHeardTutorial = true;
        StartCoroutine(ShowDialogues(tutorialDialogues));
    }
    public void ShowReadyDialogues()
    {
        StartCoroutine(ShowDialogues(readyDialogues));
    }
    private void ShowGameOverDialogues()
    {
        StopGame();
        StartCoroutine(ShowDialogues(gameOverDialogues));
    }
    private void ShowClearDialogues()
    {
        StopGame();
        StartCoroutine(ShowDialogues(clearDialogues));
    }

    public void StartGame()
    {
        isGameStart = true;
        dialoguePanel.SetActive(false);
    }

    private void StopGame()
    {
        isGameStart = false;
        dialoguePanel.SetActive(true);
        spaceText.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        EventManager.ClearAllEvent();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
