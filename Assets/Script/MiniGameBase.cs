using System.Collections;
using TMPro;
using UnityEngine;

public abstract class MiniGameBase : MonoBehaviour
{
    [SerializeField] protected CanvasGroup gameObjects;
    [SerializeField] protected GameObject warning;
    [SerializeField] protected TextMeshProUGUI rebootingText;

    protected bool isPowerDown = false;
    protected bool isError = false;
    public virtual void Init()
    {
        isPowerDown = false;
        isError = false;
        gameObjects.GetComponent<CanvasGroup>().alpha = 1;
        warning.SetActive(false);
        rebootingText.gameObject.SetActive(false);
    }

    protected void OnEnable()
    {
        EventManager.OnPowerDown += PowerDown;
        EventManager.OnPowerRecover += PowerRecover;
        EventManager.OnErrorOccur += ErrorOccur;
        EventManager.OnErrorRecovery += ErrorRecover;
        EventManager.OnRebooting += Rebooting;
    }

    protected void OnDisable()
    {
        EventManager.OnPowerDown -= PowerDown;
        EventManager.OnPowerRecover -= PowerRecover;
        EventManager.OnErrorOccur -= ErrorOccur;
        EventManager.OnErrorRecovery -= ErrorRecover;
        EventManager.OnRebooting -= Rebooting;
    }

    public abstract void GameUpdate();

    protected virtual void PowerDown()
    {
        isPowerDown = true;
        SetupUI(false);
        warning.SetActive(true);
    }

    protected virtual void PowerRecover()
    {
        isPowerDown = false;
        SetupUI(true);
        warning.SetActive(false);
    }

    protected virtual void ErrorOccur()
    {
        isError = true;
    }

    protected virtual void ErrorRecover()
    {
        isError = false;
    }

    protected virtual void Rebooting()
    {
        SetupUI(false);
        warning.SetActive(false);
        rebootingText.gameObject.SetActive(true);

        StartCoroutine(RebootCoroutine());
    }

    protected virtual IEnumerator RebootCoroutine()
    {
        yield return new WaitForSeconds(3f);
        rebootingText.gameObject.SetActive(false);
        SetupUI(true);
        ErrorRecover();
    }

    protected void SetupUI(bool isOn)
    {
        if(isOn) gameObjects.GetComponent<CanvasGroup>().alpha = 1;
        else gameObjects.GetComponent<CanvasGroup>().alpha = 0;
    }
}
