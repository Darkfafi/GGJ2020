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

    [SerializeField]
    private int maxCounter = 4;

    private int counter;
    private Keys randomKey;
	private MiniGameFinishHandler _endCallback;

    private void Update()
    {
        if (randomKey != null)
        {
            if (Input.GetKey(randomKey.keycode))
            {
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
                ChangeNodeColour();
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
    }

    public void StopMiniGame(bool success)
    {
        miniGameContainer.SetActive(false);
        randomKey = null;
        if (repairNodes != null)
        {
            for (int i = 0; i < repairNodes.Length; i++)
            {
                Destroy(repairNodes[i].gameObject);
            }
            repairNodes = null;

			if(_endCallback != null)
			{
				MiniGameFinishHandler cb = _endCallback;
				_endCallback = null;
				cb(success);
			}
        }
    }

    public void ChangeNodeColour()
    {
        repairNodes[currentRepairNode].color = new Color32(8, 241, 36, 255);
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

    [System.Serializable]
    public class Keys
    {
        public KeyCode keycode;
        public Sprite sprite;
    }

}
