using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ReflectionProbeUpdaterInterval : UdonSharpBehaviour
{
    [SerializeField] float interval = 1f; // 全プローブを1周更新する周期
    [SerializeField] private ReflectionProbe[] reflectionProbes;

    private int currentProbeIndex = 0;
    private float timer = 0f;
    private float perProbeInterval = 1f;
    private int lastProbeCount = 0;
    private float lastInterval = 1f;

    void Start()
    {
        UpdateIntervalAndCount();
        timer = 0f;
        currentProbeIndex = 0;
    }

    void Update()
    {
        // インターバルやプローブ数に変更があれば再計算
        if (interval != lastInterval || (reflectionProbes != null && reflectionProbes.Length != lastProbeCount))
        {
            UpdateIntervalAndCount();
        }

        if (reflectionProbes == null || reflectionProbes.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= perProbeInterval)
        {
            UpdateNextReflectionProbe();
            timer = 0f;
        }
    }

    void UpdateNextReflectionProbe()
    {
        if (reflectionProbes == null || reflectionProbes.Length == 0) return;

        // 配列の範囲外参照を防ぐ
        if (currentProbeIndex >= reflectionProbes.Length)
        {
            currentProbeIndex = 0;
        }

        var probe = reflectionProbes[currentProbeIndex];
        if (probe != null)
        {
            probe.RenderProbe();
        }

        currentProbeIndex = (currentProbeIndex + 1) % reflectionProbes.Length;
    }

    // インターバルやプローブ数が変わった時に呼び出す
    void UpdateIntervalAndCount()
    {
        lastProbeCount = (reflectionProbes != null) ? reflectionProbes.Length : 0;
        lastInterval = interval;
        perProbeInterval = (lastProbeCount > 0) ? (interval / lastProbeCount) : 1f;
        // currentProbeIndexが範囲外ならリセット
        if (currentProbeIndex >= lastProbeCount)
        {
            currentProbeIndex = 0;
        }
    }
}