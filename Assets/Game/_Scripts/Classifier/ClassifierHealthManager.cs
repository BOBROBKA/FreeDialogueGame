
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

public class ClassifierHealthManager
{
    private const string SERVER_URL = "http://127.0.0.1:8080/health";
    private const int SERVER_PORT = 8080;

    private Process serverProcess;
    private bool serverReady;

    public string SystemPrompt { get; private set; }
    public bool IsReady => serverReady;

    // Загружает промпт и запускает или находит сервер.
    public IEnumerator Initialize(Action<bool> onComplete)
    {
        serverReady = false;

        if (!LoadSystemPrompt())
        {
            onComplete?.Invoke(false);
            yield break;
        }

        // Проверяем, работает ли сервер.
        using (UnityWebRequest check = UnityWebRequest.Get(SERVER_URL))
        {
            check.timeout = 2;
            yield return check.SendWebRequest();

            if (check.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("LLM server is already running.");

                serverProcess = FindServerProcessOnPort(SERVER_PORT);

                if (serverProcess != null)
                {
                    Debug.Log(
                        "Found LLM server. PID: " + serverProcess.Id
                    );
                }
                else
                {
                    Debug.LogWarning(
                        "Server responds, but its process was not found. " +
                        "Automatic shutdown may not be possible."
                    );
                }

                serverReady = true;
                onComplete?.Invoke(true);
                yield break;
            }
        }

        // Если сервера нет, запускаем его.
        string projectFolder =
            Directory.GetParent(Application.dataPath).FullName;

        string serverPath = Path.Combine(
            projectFolder,
            "LLM",
            "llama",
            "llama-server.exe"
        );

        string modelPath = Path.Combine(
            projectFolder,
            "LLM",
            "model.gguf"
        );

        if (!File.Exists(serverPath))
        {
            Debug.LogError(
                "llama-server.exe not found: " + serverPath
            );

            onComplete?.Invoke(false);
            yield break;
        }

        if (!File.Exists(modelPath))
        {
            Debug.LogError(
                "Model not found: " + modelPath
            );

            onComplete?.Invoke(false);
            yield break;
        }

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = serverPath,

            Arguments =
                "-m \"" + modelPath + "\" " +
                "--host 127.0.0.1 " +
                "--port 8080 " +
                "-ngl 99 " +
                "-c 2048 " +
                "--reasoning off " +
                "--reasoning-budget 0",

            WorkingDirectory = Path.GetDirectoryName(serverPath),
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            serverProcess = Process.Start(startInfo);
        }
        catch (Exception e)
        {
            Debug.LogError(
                "Failed to start LLM server: " + e.Message
            );

            onComplete?.Invoke(false);
            yield break;
        }

        if (serverProcess == null)
        {
            Debug.LogError("Failed to create LLM server process.");
            onComplete?.Invoke(false);
            yield break;
        }

        Debug.Log(
            "Starting LLM server. PID: " + serverProcess.Id
        );

        // Ждём, пока сервер станет доступен.
        float timeout = 120f;
        float elapsed = 0f;

        while (elapsed < timeout)
        {
            if (serverProcess == null || serverProcess.HasExited)
            {
                Debug.LogError(
                    "LLM server process exited unexpectedly."
                );

                serverProcess?.Dispose();
                serverProcess = null;

                onComplete?.Invoke(false);
                yield break;
            }

            using (UnityWebRequest check =
                UnityWebRequest.Get(SERVER_URL))
            {
                check.timeout = 2;
                yield return check.SendWebRequest();

                if (check.result == UnityWebRequest.Result.Success)
                {
                    serverReady = true;

                    Debug.Log("LLM server is ready.");

                    onComplete?.Invoke(true);
                    yield break;
                }
            }

            elapsed += 1f;
            yield return new WaitForSeconds(1f);
        }

        Debug.LogError("LLM server startup timed out.");

        StopServer();
        onComplete?.Invoke(false);
    }

    // Загружает системный промпт из LLM/system_prompt.txt.
    private bool LoadSystemPrompt()
    {
        try
        {
            string projectFolder =
                Directory.GetParent(Application.dataPath).FullName;

            string promptPath = Path.Combine(
                projectFolder,
                "LLM",
                "system_prompt.txt"
            );

            if (!File.Exists(promptPath))
            {
                Debug.LogError(
                    "System prompt not found: " + promptPath
                );

                return false;
            }

            SystemPrompt = File.ReadAllText(promptPath);

            if (string.IsNullOrWhiteSpace(SystemPrompt))
            {
                Debug.LogError("System prompt is empty.");
                return false;
            }

            Debug.Log("System prompt loaded successfully.");

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(
                "Failed to load system prompt: " + e.Message
            );

            return false;
        }
    }

    // Ищет процесс, который слушает указанный порт.
    private Process FindServerProcessOnPort(int port)
    {
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "netstat.exe",
                Arguments = "-ano -p tcp",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using (Process netstat = Process.Start(startInfo))
            {
                if (netstat == null)
                    return null;

                string output = netstat.StandardOutput.ReadToEnd();
                netstat.WaitForExit();

                string[] lines = output.Split('\n');

                foreach (string line in lines)
                {
                    string[] parts = line.Split(
                        new char[] { ' ', '\t' },
                        StringSplitOptions.RemoveEmptyEntries
                    );

                    if (parts.Length < 5)
                        continue;

                    if (!parts[0].Equals(
                        "TCP",
                        StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (!parts[3].Equals(
                        "LISTENING",
                        StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (!parts[1].EndsWith(
                        ":" + port,
                        StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (!int.TryParse(parts[4], out int processId))
                        continue;

                    Process process =
                        Process.GetProcessById(processId);

                    if (process.ProcessName.Equals(
                        "llama-server",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return process;
                    }

                    process.Dispose();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(
                "Could not find LLM server process: " + e.Message
            );
        }

        return null;
    }

    // Останавливает сервер, которым владеет этот менеджер.
    public void StopServer()
    {
        serverReady = false;

        if (serverProcess == null)
        {
            Debug.LogWarning(
                "STOP SERVER: process reference is null."
            );

            return;
        }

        try
        {
            if (!serverProcess.HasExited)
            {
                Debug.Log(
                    "Stopping LLM server. PID: " + serverProcess.Id
                );

                serverProcess.Kill();
                serverProcess.WaitForExit(3000);

                if (serverProcess.HasExited)
                    Debug.Log("LLM server stopped.");
                else
                    Debug.LogError("LLM server did not stop in time.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(
                "Could not stop LLM server: " + e.Message
            );
        }
        finally
        {
            serverProcess.Dispose();
            serverProcess = null;
        }
    }
}
