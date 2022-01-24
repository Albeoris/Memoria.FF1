﻿using System.Reflection;
using Il2CppSystem;
using Il2CppSystem.Input;
using Last.Data;
using Last.Defaine.Audio;
using Last.Interpreter;
using Last.Management;
using Last.Map;
using Last.Systems;
using Last.Systems.Indicator;
using Last.UI;
using Last.UI.Common.Library;
using Last.UI.KeyInput;
using Memoria.FFPR.Configuration;
using Memoria.FFPR.IL2CPP;
using Memoria.FFPR.IL2CPP.HarmonyHooks;
using UnhollowerBaseLib;
using UnityEngine;
using Action = System.Action;
using Boolean = System.Boolean;
using Exception = System.Exception;
using Int32 = System.Int32;
using Object = UnityEngine.Object;
using String = System.String;

namespace Memoria.FFPR.Core;

public sealed class GameSaveLoadControl
{
    public GameSaveLoadControl()
    {
    }

    private Boolean _isDisabled;

    public void Update()
    {
        try
        {
            if (_isDisabled)
                return;

            ProcessSave();
            ProcessLoad();
        }
        catch (Exception ex)
        {
            _isDisabled = true;
            ModComponent.Log.LogError($"[{nameof(GameSaveLoadControl)}].{nameof(Update)}(): {ex}");
        }
    }

    private void ProcessSave()
    {
        var config = ModComponent.Instance.Config.Saves;

        Hotkey toggleKey = config.QuickSaveKey;
        String toggleAction = config.QuickSaveAction;
        Boolean isToggled = InputManager.IsToggled(toggleKey) || InputManager.GetKeyUp(toggleAction);

        if (!isToggled)
            return;

        if (!IsOperable(out SaveSlotData slot))
        {
            SingletonMonoBehaviour<AudioManager>.Instance.PlaySe(SystemSeDefine.BeepId);
            return;
        }

        SaveSlotManager.instance.Save(slot.Id);
        SingletonMonoBehaviour<AudioManager>.Instance.PlaySe(SystemSeDefine.SaveComplete);
        SystemIndicatorAccessor.Show(UiMessageConstants.MENU_REST_COMPLETED, 3.0f, Color.green, Color.black);

        String message = MessageManager.Instance.GetMessageByMessageConclusion(UiMessageConstants.MENU_REST_COMPLETED);
        ModComponent.Log.LogInfo(message);
    }

    private void ProcessLoad()
    {
        var config = ModComponent.Instance.Config.Saves;

        Hotkey toggleKey = config.QuickLoadKey;
        String toggleAction = config.QuickLoadAction;
        Boolean isToggled = InputManager.IsToggled(toggleKey) || InputManager.GetKeyUp(toggleAction);

        if (!isToggled)
            return;

        if (!IsOperable(out SaveSlotData slot))
        {
            SingletonMonoBehaviour<AudioManager>.Instance.PlaySe(SystemSeDefine.BeepId);
            return;
        }
        
        
        var title = PlayTimeStringProvider.GetPlayTime(slot.PlayTime);
        var question = MessageManager.Instance.GetMessageByMessageConclusion(UiMessageConstants.MENU_LOAD_GAME_THIS_BY_START_IS);

        FieldMap_UpdatePlayerStatePlay.Ask(title, question, ()=>LoadSafe(slot));
    }

    private static void LoadSafe(SaveSlotData slot)
    {
        // Suppress NullReferenceException
        Animator_SetSpeed.SuppressError = true;
        try
        {
            SaveSlotManager.instance.GotoLoadSaveData(slot);
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogWarning($"Game loaded with error: {ex.Message}");
        }
        finally
        {
            Animator_SetSpeed.SuppressError = false;
        }
    }

    private static Boolean IsOperable(out SaveSlotData suspended)
    {
        suspended = null;
        
        FieldMap fieldMap = Object.FindObjectOfType<FieldMap>();
        if (fieldMap is null)
            return false;

        if (!fieldMap.CheckCurrentState(SubSceneManagerMainGame.State.Player))
            return false;
        
        if (!fieldMap.IsGameUiOperable())
            return false;
        
        if (!FieldMap_UpdatePlayerStatePlay.CanAsk)
            return false;

        suspended = SaveSlotManager.instance.SuspendedSlot;
        return suspended is not null;
    }
}