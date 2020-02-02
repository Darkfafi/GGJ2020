using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Image[] exclamationMarks;
    public Image[] wrenches;

	[SerializeField]
    private Image _endScreen;

	[SerializeField]
    private Image _winScreen;

    private int currentExclamation;
    private int currentWrench;
    // Start is called before the first frame update
    private void Awake()
    {
        _endScreen.gameObject.SetActive(false);
        _winScreen.gameObject.SetActive(false);
        NPCCommunicator.Instance.NPCSeenBrokenBreakableEvent += OnShock;
        currentExclamation = 0;
        currentWrench = 0;
    }

    private void OnDestroy()
    {
        NPCCommunicator.Instance.NPCSeenBrokenBreakableEvent -= OnShock;
    }

    private void OnShock(NPC npc, Breakable breakableSeen)
    {
        ExclamationActivate();
    }

    private void ExclamationActivate()
    {
        if (exclamationMarks != null)
        {
            exclamationMarks[currentExclamation].gameObject.SetActive(true);
            currentExclamation++;
            if (currentExclamation > exclamationMarks.Length - 1)
            {
                exclamationMarks = null;
                GameOverScreen();
                currentExclamation = 0;
            }
        }
    }


    public void WrenchActivate()
    {
        if (wrenches != null)
        {
            wrenches[currentWrench].gameObject.SetActive(true);
            currentWrench++;
            if (currentWrench > wrenches.Length - 1)
            {
                wrenches = null;
                currentWrench = 0;
                WinScreen();
            }
        }
    }

    public void GameOverScreen()
    {
        _endScreen.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void WinScreen()
    {
        _winScreen.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }
}
