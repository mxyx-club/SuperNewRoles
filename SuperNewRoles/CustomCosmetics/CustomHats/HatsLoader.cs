using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BepInEx.Unity.IL2CPP.Utils;
using UnityEngine;
using UnityEngine.Networking;
using static SuperNewRoles.Logger;
using static SuperNewRoles.CustomCosmetics.CustomHats.CustomHatManager;

namespace SuperNewRoles.CustomCosmetics.CustomHats;
public class HatsLoader : MonoBehaviour
{
    private bool isRunning;
    private bool isSuccessful = true;
    public void FetchHats()
    {
        if (isRunning) return;
        this.StartCoroutine(CoFetchHats());
    }

    private IEnumerator CoFetchHats()
    {
        isRunning = true;
        string localFilePath = Path.Combine(CustomHatsDir, ManifestFileName);

        /*if (ModOption.localHats)
        {
            LoadLocalHats();
            isRunning = false;
            yield break;
        }*/

        yield return DownloadHatsConfig(localFilePath);
        isRunning = false;
    }

    private void LoadLocalHats()
    {
        try
        {
            var path = Path.Combine(CustomHatsDir, ManifestFileName);
            Logger.Msg($"加载本地帽子文件 {path}");
            var localFileContent = File.ReadAllText(path);
            var response = JsonSerializer.Deserialize<HatsConfigFile>(localFileContent, new JsonSerializerOptions
            {
                AllowTrailingCommas = true
            });
            ProcessHatsData(response);
        }
        catch
        {
            Error("不存在本地帽子配置文件.");
        }
    }

    private IEnumerator DownloadHatsConfig(string path)
    {
        var www = new UnityWebRequest
        {
            method = UnityWebRequest.kHttpVerbGET,
            downloadHandler = new DownloadHandlerBuffer()
        };

        Msg($"正在下载帽子配置文件: {RepositoryUrl}/{ManifestFileName}");
        www.url = $"{RepositoryUrl}/{ManifestFileName}";

        var operation = www.SendWebRequest();

        while (!operation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (www.isNetworkError || www.isHttpError)
        {
            Error($"下载帽子配置文件时出错: {www.error}");
            isSuccessful = false;
            LoadLocalHats();
            yield break;
        }

        try
        {
            if (!Directory.Exists(CustomHatsDir))
            {
                Directory.CreateDirectory(CustomHatsDir);
            }

            File.WriteAllBytes(path, www.downloadHandler.data);
            Msg($"帽子清单已保存到: {path}");

            var downloadedFileContent = File.ReadAllText(path);
            var response = JsonSerializer.Deserialize<HatsConfigFile>(downloadedFileContent, new JsonSerializerOptions
            {
                AllowTrailingCommas = true
            });

            ProcessHatsData(response);
        }
        catch (Exception ex)
        {
            isSuccessful = false;
            Error($"未能保存或加载帽子配置文件: {ex.Message}");
            LoadLocalHats();
        }
        finally
        {
            www.downloadHandler.Dispose();
            www.Dispose();
        }
    }

    private void ProcessHatsData(HatsConfigFile response)
    {
        UnregisteredHats.AddRange(SanitizeHats(response));
        Msg($"读取了 {UnregisteredHats.Count} 项帽子");

        /*if (!isSuccessful || ModOption.localHats)
        {
            Msg("在线配置文件无效，取消下载任务。");
            return;
        };*/

        var toDownload = GenerateDownloadList(UnregisteredHats);

        Msg($"准备下载 {toDownload.Count} 项帽子文件");

        foreach (var fileName in toDownload)
        {
            this.StartCoroutine(CoDownloadHatAsset(fileName));
        }
    }

    private static IEnumerator CoDownloadHatAsset(string fileName)
    {
        var www = new UnityWebRequest();
        www.SetMethod(UnityWebRequest.UnityWebRequestMethod.Get);
        fileName = fileName.Replace(" ", "%20");
        www.SetUrl($"{RepositoryUrl}/hats/{fileName}");
        www.downloadHandler = new DownloadHandlerBuffer();
        var operation = www.SendWebRequest();

        while (!operation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        if (www.isNetworkError || www.isHttpError)
        {
            Error(www.error);
            yield break;
        }

        var filePath = Path.Combine(CustomHatsDir, fileName);
        filePath = filePath.Replace("%20", " ");
        var persistTask = File.WriteAllBytesAsync(filePath, www.downloadHandler.data);
        while (!persistTask.IsCompleted)
        {
            Msg($"正在下载: {fileName}");
            if (persistTask.Exception != null)
            {
                Error(persistTask.Exception.Message);
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        www.downloadHandler.Dispose();
        www.Dispose();
    }
}