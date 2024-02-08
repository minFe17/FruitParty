using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Utils;

public class CSVManager : MonoBehaviour
{
    // ╫л╠шео
    List<string[]> _writeDatas = new List<string[]>();
    string[] _writeHeader;
    string[] _writeValue;
    string[] _readValue;

    StringBuilder _stringBuilder = new StringBuilder();
    SoundManager _soundManager;
    ScoreManager _scoreManager;

    string _soundDataFilePath;
    string _highScoreDataFilePath;
    string _soundDataFileName = "SoundDataFile.csv";
    string _highScoreDataFile = "HighScoreDataFile.csv";

    void Awake()
    {
        CreateDataFilePath(out _soundDataFilePath, _soundDataFileName);
        CreateDataFilePath(out _highScoreDataFilePath, _highScoreDataFile);
    }

    void CreateDataFilePath(out string filePath, string fileName)
    {
        _stringBuilder.Clear();
        _stringBuilder.Append(Application.persistentDataPath);
        _stringBuilder.Append(fileName);
        filePath = _stringBuilder.ToString();
    }

    bool CheckDataFile(string filePath)
    {
        if (File.Exists(filePath))
            return true;
        return false;
    }

    void BaseReadData(string filePath)
    {
        _readValue = null;
        if (!CheckDataFile(filePath))
            return;
        else
        {
            string source;
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                string[] lines;
                source = streamReader.ReadToEnd();
                lines = Regex.Split(source, @"\r\n|\n\r|\n|\r");
                string[] header = Regex.Split(lines[0], ",");
                _readValue = Regex.Split(lines[1], ",");
            }
        }
    }

    void BaseWriteData(string filePath)
    {
        string delimiter = ",";
        string[][] outputs = _writeDatas.ToArray();

        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < outputs.Length; i++)
        {
            stringBuilder.AppendLine(string.Join(delimiter, outputs[i]));
        }
        using (StreamWriter outStream = File.CreateText(filePath))
            outStream.Write(stringBuilder);

        _writeDatas.Clear();
    }

    public void ReadSoundData()
    {
        if (_soundManager == null)
            _soundManager = GenericSingleton<SoundManager>.Instance;

        BaseReadData(_soundDataFilePath);

        if (_readValue != null)
        {
            _soundManager.BgmSound = float.Parse(_readValue[0]);
            _soundManager.SFXSound = float.Parse(_readValue[1]);
        }
        else
        {
            _soundManager.BgmSound = 0.5f;
            _soundManager.SFXSound = 0.5f;
        }

        _readValue = null;
    }

    public void ReadHighScoreData()
    {
        if (_scoreManager == null)
            _scoreManager = GenericSingleton<ScoreManager>.Instance;

        BaseReadData(_highScoreDataFilePath);

        if (_readValue != null)
            _scoreManager.HighScore = int.Parse(_readValue[0]);
        else
            _scoreManager.HighScore = 0;

        _readValue = null;
    }

    public void WriteSoundData()
    {
        _writeDatas.Clear();
        _writeHeader = new string[] { "BGMVolume", "SFXVolume" };
        _writeDatas.Add(_writeHeader);
        _writeValue = new string[] { _soundManager.BgmSound.ToString(), _soundManager.SFXSound.ToString() };
        _writeDatas.Add(_writeValue);

        BaseWriteData(_soundDataFilePath);
    }

    public void WriteHighScoreData()
    {
        _writeDatas.Clear();
        _writeHeader = new string[] { "HighScore" };
        _writeDatas.Add(_writeHeader);
        _writeValue = new string[] { _scoreManager.HighScore.ToString() };
        _writeDatas.Add(_writeValue);

        BaseWriteData(_highScoreDataFilePath);
    }
}