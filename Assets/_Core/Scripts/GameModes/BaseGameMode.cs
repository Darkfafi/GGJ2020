using UnityEngine;

public abstract class BaseGameMode<T> : BaseGameMode where T : IGameModeSettings
{
	public T CurrentSetting
	{
		get; private set;
	}

	public void StartGameMode(T settings)
	{
		CurrentSetting = settings;
		StartMode(CurrentSetting);
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
		StopMode();
		CurrentSetting = default;
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