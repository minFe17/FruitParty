using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Utils;

public class CSVManager : MonoBehaviour
{
    // ╫л╠шео
    SoundManager _soundManager;
    ScoreManager _scoreManager;

    string _soundDataFilePath;
    string _highScoreDataFilePath;

    void Start()
    {
        CreateSoundDataFilePath();
        CreateHighScoreDataFilePath();
    }

    void CreateSoundDataFilePath()
    {
        string soundDataFileName = "SoundDataFile.csv";
        _soundDataFilePath = Application.persistentDataPath + soundDataFileName;
    }

    void CreateHighScoreDataFilePath()
    {
        string highScoreDataFile = "HighScoreDataFile.csv";
        _highScoreDataFilePath = Application.persistentDataPath + highScoreDataFile;
    }

    bool CheckDataFile(string filePath)
    {
        if (File.Exists(filePath))
            return true;
        return false;
    }

    string[] BaseReadData(string filePath)
    {
        if (!CheckDataFile(filePath))
            return null;
        else
        {
            string source;
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                string[] lines;
                source = streamReader.ReadToEnd();
                lines = Regex.Split(source, @"\r\n|\n\r|\n|\r");
                string[] header = Regex.Split(lines[0], ",");
                string[] value = Regex.Split(lines[1], ",");

                return value;
            }
        }
    }

    void BaseWriteData(List<string[]> datas, string filePath)
    {
        string delimiter = ",";
        string[][] outputs = datas.ToArray();

        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < outputs.Length; i++)
        {
            stringBuilder.AppendLine(string.Join(delimiter, outputs[i]));
        }
        using (StreamWriter outStream = File.CreateText(filePath))
            outStream.Write(stringBuilder);
    }

    public void ReadSoundData()
    {
        if (_soundManager == null)
            _soundManager = GenericSingleton<SoundManager>.Instance;
        if (_soundDataFilePath == null)
            CreateSoundDataFilePath();

        string[] value = BaseReadData(_soundDataFilePath);
        if (value != null)
        {
            _soundManager.BgmSound = float.Parse(value[0]);
            _soundManager.SFXSound = float.Parse(value[1]);
        }
        else
        {
            _soundManager.BgmSound = 0.5f;
            _soundManager.SFXSound = 0.5f;
        }
    }

    public void ReadHighScoreData()
    {
        if (_scoreManager == null)
            _scoreManager = GenericSingleton<ScoreManager>.Instance;
        if (_soundDataFilePath == null)
            CreateHighScoreDataFilePath();

        string[] value = BaseReadData(_highScoreDataFilePath);
        if (value != null)
            _scoreManager.HighScore = int.Parse(value[0]);
        else
            _scoreManager.HighScore = 0;
    }

    public void WriteSoundData()
    {
        List<string[]> datas = new List<string[]>();

        string[] header = new string[] { "BGMVolume", "SFXVolume" };
        datas.Add(header);
        string[] value = new string[] { _soundManager.BgmSound.ToString(), _soundManager.SFXSound.ToString() };
        datas.Add(value);

        BaseWriteData(datas, _soundDataFilePath);
    }

    public void WriteHighScoreData()
    {
        List<string[]> datas = new List<string[]>();

        string[] header = new string[] { "HighScore" };
        datas.Add(header);
        string[] value = new string[] { _scoreManager.HighScore.ToString() };
        datas.Add(value);

        BaseWriteData(datas, _highScoreDataFilePath);
    }
}