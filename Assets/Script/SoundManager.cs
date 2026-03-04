using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum Conditions
    {
        Warp,
        PowerDown,
        PowerRecovery,
        OccuerError,
        RightCode,
        WrongCode,
        Crash,
        CargoDown
    }

    [System.Serializable]
    public struct SFX
    {
        public Conditions type;
        public AudioClip clip;
    }
    public SFX[] SFXs;
    public static SoundManager instance;
    [SerializeField] private AudioSource SFXSource;
    void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(Conditions condition)
    {
        foreach(SFX sfx in SFXs)
        {
            if (sfx.type == condition) SFXSource.PlayOneShot(sfx.clip);
        }
    }
}
