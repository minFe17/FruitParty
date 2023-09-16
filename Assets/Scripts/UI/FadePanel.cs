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
        soundManager.Init();    // 로비에서 Init()이랑 csv 파일 체크
        soundManager.StartBGM(bgmAudio);
    }
}