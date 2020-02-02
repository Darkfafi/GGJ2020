using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MiniGame : MonoBehaviour
{
	public delegate void MiniGameFinishHandler(bool successful);

    public Keys[] miniGameKeys;
    public Image currentKeyImage;

    public Image[] repairNodes;
    private int currentRepairNode;

    public GameObject miniGameContainer;
    public GameObject countContainer;
    public Image nodePrefab;

    public GameManager gameManager;

    private AudioSource _audio;
    public AudioClip buttonPressSFX;
    public AudioClip finishedGameSFX;
    public AudioClip wrongSFX;

	public bool IsMinigameActive
	{
		get
		{
			return miniGameContainer.activeInHierarchy;
		}
	}

    [SerializeField]
    private int maxCounter = 4;

    private int counter;
    private Keys randomKey;
	private MiniGameFinishHandler _endCallback;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (randomKey != null)
        {
            if (Input.GetKey(randomKey.keycode))
            {
                _audio.PlayOneShot(buttonPressSFX);
                Keys preKey = randomKey;
                if (miniGameKeys.Length > 2)
                {
                    while(randomKey == preKey)
                    {
                        SelectRandomKey();
                    }
                }
                else
                {
                    SelectRandomKey();
                }

                counter++;
                currentRepairNode++;

				PulseAnimation(currentKeyImage.transform);

				ChangeNodeColour();
                
                //wrench
                if (counter >= maxCounter)
                {
                    StopMiniGame(true);
                }
            }
            else
            {
                for(int i = 0; i < miniGameKeys.Length; i++)
                {
                    if (Input.GetKeyDown(miniGameKeys[i].keycode))
                    {
						MiniGameFinishHandler callback = _endCallback;
						_endCallback = null;
                        _audio.PlayOneShot(wrongSFX);
                        StartMiniGame(callback);
                        break;
                    }
                }
            }
        }
    }

    public void StartMiniGame(MiniGameFinishHandler endCallback)
    {
        StopMiniGame(false);
		_endCallback = endCallback;
		counter = 0;
        currentRepairNode = -1;
        miniGameContainer.SetActive(true);
        GenerateRepairNodes();
        SelectRandomKey();
		PulseAnimation(currentKeyImage.transform);

	}

    public void StopMiniGame(bool success)
    {
        miniGameContainer.SetActive(false);
        randomKey = null;
        if (repairNodes != null)
        {
            for (int i = 0; i < repairNodes.Length; i++)
            {
				repairNodes[i].transform.DOKill();
				Destroy(repairNodes[i].gameObject);
            }
            repairNodes = null;

			if(_endCallback != null)
			{
				MiniGameFinishHandler cb = _endCallback;
				_endCallback = null;
				if(success)
				{
                    _audio.PlayOneShot(finishedGameSFX);
					gameManager.WrenchActivate();
				}
                cb(success);
            }
        }
    }

    public void ChangeNodeColour()
    {
        repairNodes[currentRepairNode].color = new Color32(8, 241, 36, 255);
		PulseAnimation(repairNodes[currentRepairNode].transform);
	}

    public void GenerateRepairNodes()
    {
        repairNodes = new Image[maxCounter];

        for(int i  = 0; i < maxCounter; i++)
        {
            repairNodes[i] = Instantiate(nodePrefab, countContainer.transform);
        }
    }

	private void SelectRandomKey()
    {
        int rand = Random.Range(0, miniGameKeys.Length);
        randomKey = miniGameKeys[rand];
        currentKeyImage.sprite = randomKey.sprite;
    }

	private void PulseAnimation(Transform t)
	{
		t.localScale = Vector3.one * 0.15f;
		t.DOComplete(true);
		t.DOKill();
		t.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
	}

    [System.Serializable]
    public class Keys
    {
        public KeyCode keycode;
        public Sprite sprite;
    }

}
