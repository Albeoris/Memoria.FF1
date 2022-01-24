﻿using System;
using BepInEx.Configuration;
using Memoria.FFPR.Configuration;
using KeyCode = UnityEngine.KeyCode;
using Memoria.FFPR.BeepInEx;

namespace Memoria.FFPR.Configuration.Scopes;

public sealed class SpeedConfiguration
{
    private const String Section = "Speed";

    public ConfigEntry<Hotkey> ToggleKey { get; }
    public ConfigEntry<Hotkey> HoldKey { get; }
    public ConfigEntry<String> ToggleAction { get; }
    public ConfigEntry<String> HoldAction { get; }
    public ConfigEntry<Single> ToggleFactor { get; }
    public ConfigEntry<Single> HoldFactor { get; }

    public SpeedConfiguration(ConfigFileProvider provider)
        : this(provider.Get(Section))
    {
    }

    public SpeedConfiguration(ConfigFile file)
    {
        ToggleKey = file.Bind(Section, nameof(ToggleKey),  new Hotkey{Key = KeyCode.F1},
            $"Speed up toggle key.",
            new AcceptableHotkey(nameof(ToggleKey)));

        HoldKey = file.Bind(Section, nameof(HoldKey), new Hotkey(),
            $"Speed up hold key.",
            new AcceptableHotkey(nameof(HoldKey)));

        ToggleAction = file.Bind(Section, nameof(ToggleAction), "None",
            $"Speed up toggle action.",
            new AcceptableValueList<String>("None", "Enter", "Cancel", "Shortcut", "Menu", "Up", "Down", "Left", "Right", "SwitchLeft", "SwitchRight", "PageUp", "PageDown", "Start"));

        HoldAction = file.Bind(Section, nameof(HoldAction), "None",
            $"Speed up hold action.",
            new AcceptableValueList<String>("None", "Enter", "Cancel", "Shortcut", "Menu", "Up", "Down", "Left", "Right", "SwitchLeft", "SwitchRight", "PageUp", "PageDown", "Start"));

        ToggleFactor = file.Bind(Section, nameof(ToggleFactor), 3.0f,
            "Speed up toggle factor.",
            0.01f, 10.0f);

        HoldFactor = file.Bind(Section, nameof(HoldFactor), 5.0f,
            "Speed up hold factor.",
            0.01f, 10.0f);
    }

    public void CopyFrom(SpeedConfiguration other)
    {
        ToggleKey.Value = other.ToggleKey.Value;
        HoldKey.Value = other.HoldKey.Value;
        ToggleAction.Value = other.ToggleAction.Value;
        HoldAction.Value = other.HoldAction.Value;
        ToggleFactor.Value = other.ToggleFactor.Value;
        HoldFactor.Value = other.HoldFactor.Value;
    }
}