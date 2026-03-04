using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierSystem : MiniGameBase
{
    private int onBarrier = 0;
    [SerializeField] Barrier[] barriers = new Barrier[8];

    private float spawnInterval;
    private float minCrashTime = 10f;
    private float maxCrashTime = 20f;
    private Coroutine spawnCoroutine;

    public override void Init()
    {
        base.Init();
        onBarrier = 0;
        spawnInterval = Random.Range(3f, 7f);
        foreach (var b in barriers) b.Init();
    }
    protected override void PowerDown()
    {
        base.PowerDown();
        AllBarrierOff();
    }

    void AllBarrierOff()
    {
        foreach (var b in barriers) b.OnBarrier(false);
        onBarrier = 0;
        EventManager.OnBarrierChange?.Invoke(onBarrier);
    }

    public override void GameUpdate()
    {
        foreach (var b in barriers)
        {
            b.GameUpdate();
        }

        if (spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnMeteor());
        }

        if (isError) return;

        foreach (var b in barriers)
        {
            if (Input.GetKeyDown(b.keyIndex))
            {
                b.OnBarrier(!b.isActive);
                if (b.isActive) onBarrier++;
                else onBarrier--;

                EventManager.OnBarrierChange?.Invoke(onBarrier);
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            if(0 < onBarrier)
            {
                foreach(var b in barriers)
                {
                    b.OnBarrier(false);
                    onBarrier = 0;
                }
            }
            else
            {
                foreach(var b in barriers)
                {
                    b.OnBarrier(true);
                    onBarrier = barriers.Length;
                }
            }

            EventManager.OnBarrierChange?.Invoke(onBarrier);
        }
    }

    private void SpawnMeteorRandom()
    {
        List<Barrier> available = new List<Barrier>();

        foreach (var b in barriers)
        {
            if (!b.comeMeteor) available.Add(b);
        }

        if(available.Count == 0) return;
        
        int spawnMeteorAmount = Random.Range(1, Mathf.Min(available.Count, 3));

        for(int i = 0; i < spawnMeteorAmount; i++)
        {
            int index = Random.Range(0, available.Count);
            float crashTime = Random.Range(minCrashTime, maxCrashTime);
            available[index].FindMeteor(crashTime);
            available.RemoveAt(index);
        }
    }

    private IEnumerator SpawnMeteor()
    {
        while (true)
        {
            yield return new CustomWaitForSeconds(spawnInterval, () => isError);
            spawnInterval = Random.Range(3f, 7f);
            SpawnMeteorRandom();
        }
    }
}
