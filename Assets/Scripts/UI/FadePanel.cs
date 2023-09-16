using UnityEngine;
using Utils;

public class FadePanel : MonoBehaviour
{
    void GameStart()
    {
        StartBGM();
        GameManager gameManager = GenericSingleton<GameManager>.Instance;
        gameManager.GameState = EGameStateType.Move;
        gameManager.IsGameStart = true;
    }

    void StartBGM()
    {
        SoundManager soundManager = GenericSingleton<SoundManager>.Instance;
        AudioClip bgmAudio = Resources.Load("Prefabs/AudioClip/BGM") as AudioClip;
        soundManager.Init();    // �κ񿡼� Init()�̶� csv ���� üũ
        soundManager.StartBGM(bgmAudio);
    }
}