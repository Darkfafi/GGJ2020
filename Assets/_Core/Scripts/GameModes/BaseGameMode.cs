﻿using UnityEngine;

public abstract class BaseGameMode<T> : BaseGameMode where T : IGameModeSettings
{
	public enum State
	{
		NotActive,
		Active
	}

	public T CurrentSetting
	{
		get; private set;
	}

	public State GameModeState
	{
		get; private set;
	}

	public void StartGameMode(T settings)
	{
		if(GameModeState != State.Active)
		{
			GameModeState = State.Active;
			CurrentSetting = settings;
			StartMode(CurrentSetting);
		}
	}

	public override void StartGameMode(IGameModeSettings settings)
	{
		if(!typeof(T).IsAssignableFrom(settings.GetType()))
		{
			Debug.LogErrorFormat("Settings {0} is not of type {1}, Start Game Mode Canceled!", settings.GetType(), typeof(T));
			return;
		}
		StartGameMode((T)settings);
	}

	public override void StopGameMode()
	{
		if (GameModeState != State.NotActive)
		{
			GameModeState = State.NotActive;
			StopMode();
			CurrentSetting = default;
		}
	}

	protected abstract void StartMode(T settings);
	protected abstract void StopMode();
}

public abstract class BaseGameMode : MonoBehaviour
{
	public delegate void GameModeEndHandler(bool win);
	public GameModeEndHandler GameModeEndedEvent;

	public abstract void StartGameMode(IGameModeSettings settings);
	public abstract void StopGameMode();

	protected void CallWinCondition()
	{
		if(GameModeEndedEvent != null)
		{
			GameModeEndedEvent(true);
		}
	}

	protected void CallLoseCondition()
	{
		if (GameModeEndedEvent != null)
		{
			GameModeEndedEvent(false);
		}
	}
}

public interface IGameModeSettings
{

}