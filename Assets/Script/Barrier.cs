using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SoundManager;

public class Barrier : MonoBehaviour
{
    public KeyCode keyIndex;

    public Image icon;
    public TextMeshProUGUI keyText;
    public TextMeshProUGUI timeText;
    public Image barrier;

    public bool isActive = false;
    public bool comeMeteor = false;
    public float meteorTimer = 0f;

    public void Init()
    {
        isActive = false;
        comeMeteor = false;
        meteorTimer = 0f;
        ChangeUIColor(Color.white);
        barrier.color = Color.white;
    }

    private void ChangeUIColor(Color color)
    {
        icon.color = color;
        keyText.color = color;
        timeText.color = color;
    }

    public void FindMeteor(float crashTime)
    {
        comeMeteor = true;
        meteorTimer = crashTime;
    }

    public void OnBarrier(bool power)
    {
        isActive = power;
        if(power) barrier.color = Color.blue;
        else barrier.color = Color.white;
    }

    public void GameUpdate()
    {
        meteorTimer -= Time.deltaTime;
        float displayTime = Mathf.Max(Mathf.Round(meteorTimer * 100) / 100, 0f);
        timeText.text = displayTime.ToString("F2") +"s";

        if (!comeMeteor)
        {
            ChangeUIColor(Color.white);
        }
        else
        {
            if (displayTime <= 2f) ChangeUIColor(Color.red);
            else if (displayTime <= 5f) ChangeUIColor(Color.orange);
            else if (comeMeteor) ChangeUIColor(Color.yellow);
        }

        if (comeMeteor && meteorTimer <= 0f) Crash();
    }

    void Crash()
    {
        comeMeteor = false;
        ChangeUIColor(Color.white);

        if (!isActive)
        {
            SoundManager.instance.PlaySFX(Conditions.Crash);
            EventManager.OnRockCollision?.Invoke();
        }
    }
}
