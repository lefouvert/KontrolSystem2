﻿using KontrolSystem.TO2.Binding;
using KSP.Modules;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public abstract class BaseEngineAdapter {
    protected readonly Data_Engine dataEngine;

    protected BaseEngineAdapter(Data_Engine dataEngine) {
        this.dataEngine = dataEngine;
    }

    [KSField] public bool IsShutdown => dataEngine.EngineShutdown;

    [KSField] public bool HasIgnited => dataEngine.EngineIgnited;

    [KSField] public bool IsFlameout => dataEngine.Flameout;

    [KSField] public bool IsStaged => dataEngine.staged;

    [KSField] public bool IsOperational => dataEngine.IsOperational;

    [KSField] public bool IsPropellantStarved => dataEngine.IsPropellantStarved;

    [KSField] public double CurrentThrottle => dataEngine.currentThrottle;

    [KSField] public double CurrentThrust => dataEngine.FinalThrustValue;

    [KSField] public double RealIsp => dataEngine.RealISPValue;

    [KSField] public double ThrottleMin => dataEngine.ThrottleMin;

    [KSField] public double MinFuelFlow => dataEngine.MinFuelFlow;

    [KSField] public double MaxFuelFlow => dataEngine.MaxFuelFlow;

    [KSField] public double MaxThrustOutputVac => dataEngine.MaxThrustOutputVac();

    [KSField] public double MaxThrustOutputAtm => dataEngine.MaxThrustOutputAtm();

    [KSField]
    public bool AutoSwitchMode {
        get => dataEngine.EngineAutoSwitchMode.GetValue();
        set => dataEngine.EngineAutoSwitchMode.SetValue(value);
    }

    [KSField]
    public bool IndependentThrottleEnabled {
        get => dataEngine.IndependentThrottle.GetValue();
        set => dataEngine.IndependentThrottle.SetValue(value);
    }

    [KSField]
    public double IndependentThrottle {
        get => dataEngine.IndependentThrottlePercentage.GetValue() / 100.0;
        set => dataEngine.IndependentThrottlePercentage.SetValue((float)(value * 100.0));
    }
}
