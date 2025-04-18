using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class GameLogger : MonoBehaviour
{
    public static GameLogger Instance { get; private set; }

    private string logFilePath;
    public string LogFilePath => logFilePath;

    private bool isHeaderWritten = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        string logDir = Path.Combine(Application.persistentDataPath, "log"); // 빌드 호환
        Directory.CreateDirectory(logDir);

        string fileName = $"GameLog_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv";
        logFilePath = Path.Combine(logDir, fileName);

        Application.logMessageReceived += HandleUnityLog;
        Log("System|Initialized|Status=OK", LogType.Log); // 초기 로그
    }

    public void Log(string message, LogType type = LogType.Log, string stackTrace = "")
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string formatted = $"[{timestamp}] {message}";
        Debug.Log(formatted);

        WriteToCsv(timestamp, type.ToString(), message, stackTrace);
    }

    private void HandleUnityLog(string logString, string stackTrace, LogType type)
    {
        if (logString.StartsWith("["))
            return;

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        WriteToCsv(timestamp, type.ToString(), logString, stackTrace);
    }

    private void WriteToCsv(string timestamp, string logType, string message, string stackTrace)
    {
        try
        {
            // 기본 열
            List<string> csvColumns = new List<string> { timestamp, logType };
            List<string> headerColumns = new List<string> { "Timestamp", "LogType" };

            // 메시지 파싱
            bool isStructured = ParseMessage(message, out Dictionary<string, string> fields);

            if (isStructured)
            {
                // 구조화된 메시지: Category, Action, Detail1, Detail2, ...
                csvColumns.Add(EscapeCsvField(fields.GetValueOrDefault("Category", "Unknown")));
                csvColumns.Add(EscapeCsvField(fields.GetValueOrDefault("Action", "Unknown")));
                headerColumns.Add("Category");
                headerColumns.Add("Action");

                // 세부사항(키-값 쌍) 추가
                foreach (var kvp in fields)
                {
                    if (kvp.Key != "Category" && kvp.Key != "Action")
                    {
                        csvColumns.Add(EscapeCsvField(kvp.Value));
                        headerColumns.Add(kvp.Key);
                    }
                }

                // StackTrace 추가
                csvColumns.Add(EscapeCsvField(stackTrace));
                headerColumns.Add("StackTrace");
            }
            else
            {
                // 비구조화된 메시지: 기존 방식
                csvColumns.Add(EscapeCsvField(message));
                csvColumns.Add(EscapeCsvField(stackTrace));
                headerColumns.Add("Message");
                headerColumns.Add("StackTrace");
            }

            // 헤더 작성
            if (!isHeaderWritten)
            {
                File.WriteAllText(logFilePath, string.Join(",", headerColumns) + "\n");
                isHeaderWritten = true;
            }

            // CSV 행 추가
            File.AppendAllText(logFilePath, string.Join(",", csvColumns) + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to write to CSV: {ex.Message}");
        }
    }

    private bool ParseMessage(string message, out Dictionary<string, string> fields)
    {
        fields = new Dictionary<string, string>();

        // 메시지가 형식에 맞는지 확인 (최소 2개 필드: Category|Action)
        string[] parts = message.Split('|');
        if (parts.Length < 2)
            return false;

        // Category와 Action 추가
        fields["Category"] = parts[0].Trim();
        fields["Action"] = parts[1].Trim();

        // 세부사항 파싱 (Detail1=Value1|Detail2=Value2)
        for (int i = 2; i < parts.Length; i++)
        {
            string[] kvp = parts[i].Split('=');
            if (kvp.Length == 2)
            {
                fields[kvp[0].Trim()] = kvp[1].Trim();
            }
        }

        return true;
    }

    private string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field))
            return "\"\"";
        return $"\"{field.Replace("\"", "\"\"")}\"";
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleUnityLog;
    }
}