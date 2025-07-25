﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
#if UNITY_2021_2_OR_NEWER
using UnityEditor.Build;
#endif
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace AssetInventory
{
    public static class AssetUtils
    {
        private static readonly Regex NoSpecialChars = new Regex("[^a-zA-Z0-9 -]"); // private static Regex AssetStoreContext.s_InvalidPathCharsRegExp = new Regex("[^a-zA-Z0-9() _-]");
        private static readonly Dictionary<string, Texture2D> PreviewCache = new Dictionary<string, Texture2D>();

        public static string GetProjectRoot()
        {
            return Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length).Replace("\\", "/");
        }

        public static string AddProjectRoot(string path)
        {
            if (!path.ToLowerInvariant().StartsWith("asset")) return path;

            return Path.Combine(GetProjectRoot(), path);
        }

        public static string RemoveProjectRoot(string path)
        {
            path = IOUtils.ToShortPath(path);
            if (!path.StartsWith(GetProjectRoot())) return path;

            return path.Substring(GetProjectRoot().Length);
        }

        public static void RemoveLODGroups(string path)
        {
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            LODGroup[] groups = root.GetComponentsInChildren<LODGroup>(true);

            foreach (LODGroup group in groups)
            {
                if (group == null) continue;

                // handle prefabs recursively as GameObjects cannot be removed otherwise
                if (PrefabUtility.IsPartOfPrefabInstance(group.gameObject))
                {
                    string nestedPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(group.gameObject);
                    if (!string.IsNullOrEmpty(nestedPath) && nestedPath != path)
                    {
                        RemoveLODGroups(nestedPath);
                    }
                }
                else
                {
                    LOD[] lods = group.GetLODs();
                    for (int i = 1; i < lods.Length; i++)
                    {
                        Renderer[] renderers = lods[i].renderers;
                        for (int j = 0; j < renderers.Length; j++)
                        {
                            if (renderers[j] != null)
                            {
                                GameObject go = renderers[j].gameObject;

                                // try deleting directly first
                                bool retryNested = false;
                                try
                                {
                                    Object.DestroyImmediate(go);
                                }
                                catch (Exception)
                                {
                                    // this will happen if the object to delete is a child of the actual object to be deleted, e.g. a child inside a model file
                                    retryNested = true;
                                }

                                if (retryNested)
                                {
                                    // work our way up to the actual prefab instance root
                                    while (!PrefabUtility.IsAnyPrefabInstanceRoot(go) && go.transform.parent != null)
                                    {
                                        go = go.transform.parent.gameObject;
                                    }
                                    if (go.transform.parent != null) Object.DestroyImmediate(go);
                                }
                            }
                        }
                    }
                    Object.DestroyImmediate(group);
                }
            }

            // cannot be saved otherwise
            root.transform.RemoveMissingScripts();

            PrefabUtility.SaveAsPrefabAsset(root, path);
            PrefabUtility.UnloadPrefabContents(root);
        }

#if UNITY_2021_2_OR_NEWER
        private static List<string> GetCurrentDefines() => PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup)).Split(';').ToList();
        private static void SetCurrentDefines(IEnumerable<string> keywords)
        {
            BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            if (selectedBuildTargetGroup == BuildTargetGroup.Unknown) return;

            NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(selectedBuildTargetGroup);
            if (namedBuildTarget == NamedBuildTarget.Unknown) return;

            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, string.Join(";", keywords));
        }
#else
        private static List<string> GetCurrentDefines() => PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';').ToList();

        private static void SetCurrentDefines(IEnumerable<string> keywords)
        {
            BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            if (selectedBuildTargetGroup == BuildTargetGroup.Unknown) return;
            
            PlayerSettings.SetScriptingDefineSymbolsForGroup(selectedBuildTargetGroup, string.Join(";", keywords));
        }
#endif

        public static bool HasDefine(string keyword) => GetCurrentDefines().Contains(keyword);

        public static void AddDefine(string keyword) => SetCurrentDefines(GetCurrentDefines().Union(new List<string> {keyword}));
        public static void RemoveDefine(string keyword) => SetCurrentDefines(GetCurrentDefines().Where(d => d != keyword));

#if UNITY_2021_3_OR_NEWER
        private static List<string> GetCurrentCompilerArguments() => PlayerSettings.GetAdditionalCompilerArguments(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup)).ToList();
        private static void SetCurrentCompilerArguments(IEnumerable<string> args)
        {
            BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            if (selectedBuildTargetGroup == BuildTargetGroup.Unknown) return;

            NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(selectedBuildTargetGroup);
            if (namedBuildTarget == NamedBuildTarget.Unknown) return;

            PlayerSettings.SetAdditionalCompilerArguments(namedBuildTarget, args.ToArray());
        }

        public static bool HasCompilerArgument(string arg) => GetCurrentCompilerArguments().Contains(arg);
        public static void AddCompilerArgument(string arg) => SetCurrentCompilerArguments(GetCurrentCompilerArguments().Union(new List<string> {arg}));
        public static void RemoveCompilerArgument(string arg) => SetCurrentCompilerArguments(GetCurrentCompilerArguments().Where(d => d != arg));
#endif

        public static int GetPageCount(int resultCount, int maxResults)
        {
            return (int)Math.Ceiling((double)resultCount / (maxResults > 0 ? maxResults : int.MaxValue));
        }

        public static void ClearCache()
        {
            foreach (Texture2D texture in PreviewCache.Values)
            {
                if (texture != null)
                {
                    Object.DestroyImmediate(texture);
                }
            }
            PreviewCache.Clear();
        }

        public static int RemoveMissingScripts(this Transform obj)
        {
            int result = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj.gameObject);
            for (int i = 0; i < obj.childCount; i++)
            {
                result += RemoveMissingScripts(obj.GetChild(i));
            }
            return result;
        }

        public static async Task<AudioClip> LoadAudioFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return null;

            // workaround for Unity not supporting loading local files with # or + or unicode chars in the name
            if (filePath.Contains("#") || filePath.Contains("+") || filePath.IsUnicode())
            {
                string newName = Path.Combine(Application.temporaryCachePath, "AIAudioPreview" + Path.GetExtension(filePath));
                File.Copy(filePath, newName, true);
                filePath = newName;
            }

            // use uri form to support network shares
            filePath = IOUtils.ToShortPath(filePath);
            string fileUri;
            try
            {
                fileUri = new Uri(filePath).AbsoluteUri;
            }
            catch (UriFormatException e)
            {
                Debug.LogError($"Could not convert path to URI '{filePath}': {e.Message}");
                return null;
            }

            // select appropriate audio type from extension where UNKNOWN heuristic can fail, especially for AIFF

            // retry with other types since some authors store especially wav files under the wrong format (e.g. ogg)
            List<AudioType> fallbackChain;
            switch (Path.GetExtension(filePath).ToLowerInvariant())
            {
                case ".aiff":
                case ".aif":
                    fallbackChain = new List<AudioType> {AudioType.AIFF, AudioType.OGGVORBIS, AudioType.WAV, AudioType.UNKNOWN};
                    break;

                case ".ogg":
                    fallbackChain = new List<AudioType> {AudioType.OGGVORBIS, AudioType.WAV, AudioType.UNKNOWN, AudioType.AIFF};
                    break;

                case ".wav":
                    fallbackChain = new List<AudioType> {AudioType.WAV, AudioType.OGGVORBIS, AudioType.UNKNOWN, AudioType.AIFF};
                    break;

                default:
                    fallbackChain = new List<AudioType> {AudioType.UNKNOWN, AudioType.OGGVORBIS, AudioType.WAV, AudioType.AIFF};
                    break;
            }
            fallbackChain.AddRange(new List<AudioType> {AudioType.MPEG, AudioType.IT, AudioType.S3M, AudioType.XM, AudioType.ACC, AudioType.MOD, AudioType.VAG, AudioType.XMA, AudioType.AUDIOQUEUE});

            foreach (AudioType type in fallbackChain)
            {
                using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(fileUri, type))
                {
                    ((DownloadHandlerAudioClip)uwr.downloadHandler).streamAudio = true;
                    uwr.timeout = AI.Config.timeout;
                    UnityWebRequestAsyncOperation request = uwr.SendWebRequest();
                    while (!request.isDone) await Task.Yield();

#if UNITY_2020_1_OR_NEWER
                    if (uwr.result != UnityWebRequest.Result.Success)
#else
                    if (uwr.isNetworkError || uwr.isHttpError)
#endif
                    {
                        Debug.LogError($"Error fetching '{filePath} ({fileUri})': {uwr.error}");
                        return null;
                    }

                    DownloadHandlerAudioClip dlHandler = (DownloadHandlerAudioClip)uwr.downloadHandler;
                    dlHandler.streamAudio = false; // otherwise tracker files won't work
                    if (dlHandler.isDone)
                    {
                        // can fail if FMOD encounters incorrect file, will return zero-length then, error cannot be suppressed
                        AudioClip clip = dlHandler.audioClip;
                        if (clip == null || (clip.channels == 0 && clip.length == 0)) continue;

                        return clip;
                    }
                }
            }
            if (AI.Config.LogAudioParsing) Debug.LogError($"Unity could not load incompatible audio clip '{filePath} ({fileUri})'");

            return null;
        }

        public static async void LoadTextures(List<AssetInfo> infos, CancellationToken ct, Action<int, Texture2D> callback = null)
        {
            int chunkSize = AI.Config.previewChunkSize;

            for (int i = 0; i < infos.Count; i += chunkSize)
            {
                if (ct.IsCancellationRequested) break;

                List<Task> tasks = new List<Task>();

                int chunkEnd = Math.Min(i + chunkSize, infos.Count);
                for (int idx = i; idx < chunkEnd; idx++)
                {
                    int localIdx = idx; // capture value
                    AssetInfo info = infos[idx];

                    tasks.Add(ProcessAssetInfoAsync(info, localIdx, ct, callback));
                }
                await Task.WhenAll(tasks);
            }
        }

        private static async Task ProcessAssetInfoAsync(AssetInfo info, int idx, CancellationToken ct, Action<int, Texture2D> callback = null)
        {
            if (ct.IsCancellationRequested) return;

            if (info.ParentInfo != null)
            {
                await LoadPackageTexture(info.ParentInfo);
                info.PreviewTexture = info.ParentInfo.PreviewTexture;
            }
            else
            {
                await LoadPackageTexture(info);
            }
            callback?.Invoke(idx, info.PreviewTexture);
        }

        public static async Task LoadPackageTexture(AssetInfo info, bool useCache = true)
        {
            string file = info.ToAsset().GetPreviewFile(AI.GetPreviewFolder());
            if (string.IsNullOrEmpty(file)) return;

            Texture2D texture;
            if (useCache && PreviewCache.TryGetValue(file, out Texture2D pt) && pt != null)
            {
                texture = pt;
            }
            else
            {
                texture = await LoadLocalTexture(file, true);
                if (texture != null)
                {
                    PreviewCache[file] = texture;
                }
                else
                {
                    // texture could not be loaded, remove if it could never be loaded so far to auto-heal the state
                    if (PreviewCache.ContainsKey(file)) return;
                    if (AI.Config.LogMediaDownloads) Debug.LogWarning($"Could not load texture for {info.DisplayName} ({file}), removing from file system.");
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Could not delete texture file '{file}': {e.Message}");
                    }
                }
            }
            if (texture != null) info.PreviewTexture = texture;
        }

        public static void RemoveFromPreviewCache(string file)
        {
            if (PreviewCache.ContainsKey(file)) PreviewCache.Remove(file);
        }

        public static async Task<Texture2D> LoadLocalTexture(string file, bool useCache, int upscale = 0, bool upscaleIsMax = false)
        {
            file = IOUtils.ToShortPath(file);

            if (useCache && PreviewCache.TryGetValue(file, out Texture2D texture))
            {
                if (texture != null) return texture;

                PreviewCache.Remove(file); // entry became null, remove it
            }

            try
            {
                byte[] content = await Task.Run(() => File.ReadAllBytes(file));
                if (content == null || content.Length == 0)
                {
                    if (AI.Config.LogMediaDownloads) Debug.LogError($"Failed to read file data from '{file}'.");
                    if (content != null && content.Length == 0) File.Delete(file); // erroneous file, clean up right away
                    return null;
                }

                Texture2D result = new Texture2D(2, 2);
                if (!result.LoadImage(content))
                {
                    if (AI.Config.LogMediaDownloads) Debug.LogError($"Failed to load image from '{file}'. The data might be corrupted.");
                    return null;
                }

                result.hideFlags = HideFlags.HideAndDontSave;
                if (upscale > 0 && ((result.width < upscale && result.height < upscale) || upscaleIsMax))
                {
                    Texture2D original = result;
                    result = result.Resize(upscale);
                    Object.DestroyImmediate(original);
                }

                if (useCache) PreviewCache[file] = result;

                return result;
            }
            catch (Exception e)
            {
                if (AI.Config.LogMediaDownloads) Debug.LogError($"Unhandled error loading local texture '{file}': {e.Message}");
                return null;
            }
        }

        public static async Task<T> FetchAPIData<T>(string uri, string method = "GET", string postContent = null, string token = null, string etag = null, Action<string> eTagCallback = null, int retries = 1, Action<long> responseIssueCodeCallback = null, bool suppressErrors = false, string postType = "application/json")
        {
            Restart:
            using (UnityWebRequest uwr = method == "GET" ? UnityWebRequest.Get(uri) : new UnityWebRequest(uri, method))
            {
                if (!string.IsNullOrEmpty(token)) uwr.SetRequestHeader("Authorization", "Bearer " + token);
                if (!string.IsNullOrEmpty(etag)) uwr.SetRequestHeader("If-None-Match", etag);
                if (!string.IsNullOrEmpty(postContent))
                {
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(postContent);
                    uwr.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    uwr.uploadHandler.contentType = postType;
                    uwr.downloadHandler = new DownloadHandlerBuffer();
                }
                uwr.SetRequestHeader("Content-Type", postType);
                uwr.SetRequestHeader("User-Agent", $"UnityEditor/{Application.unityVersion} ({SystemInfo.operatingSystemFamily}; {SystemInfo.operatingSystem})");
                uwr.timeout = AI.Config.timeout;
                UnityWebRequestAsyncOperation request = uwr.SendWebRequest();
                while (!request.isDone) await Task.Yield();

#if UNITY_2020_1_OR_NEWER
                if (uwr.result == UnityWebRequest.Result.ConnectionError)
#else
                if (uwr.isNetworkError)
#endif
                {
                    if (retries > 0)
                    {
                        retries--;
                        goto Restart;
                    }
                    if (!suppressErrors) Debug.LogError($"Could not fetch API data from {uri} due to network issues: {uwr.error}");
                }
#if UNITY_2020_1_OR_NEWER
                else if (uwr.result == UnityWebRequest.Result.ProtocolError)
#else
                else if (uwr.isHttpError)
#endif
                {
                    responseIssueCodeCallback?.Invoke(uwr.responseCode);
                    if (uwr.responseCode == (int)HttpStatusCode.Unauthorized)
                    {
                        if (!suppressErrors) Debug.LogError($"Invalid or expired API Token when contacting {uri}");
                    }
                    else
                    {
                        if (!suppressErrors) Debug.LogError($"Error fetching API data from {uri} ({uwr.responseCode}): {uwr.downloadHandler.text}");
                    }
                }
                else
                {
                    if (typeof (T) == typeof (string))
                    {
                        return (T)Convert.ChangeType(uwr.downloadHandler.text, typeof (T));
                    }

                    string newEtag = uwr.GetResponseHeader("ETag");
                    if (!string.IsNullOrEmpty(newEtag) && newEtag != etag) eTagCallback?.Invoke(newEtag);

                    try
                    {
                        return JsonConvert.DeserializeObject<T>(uwr.downloadHandler.text);
                    }
                    catch (Exception e)
                    {
                        // can happen if deserializers in local project have been added/altered
                        Debug.LogError($"Error parsing API data from {uri}: {e.Message}");
                    }
                }
            }

            return default(T);
        }

        public static async Task LoadImageAsync(string imageUrl, string targetFile)
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imageUrl))
            {
                // Send the request and wait for the response without blocking the main thread
                uwr.timeout = AI.Config.timeout;
                UnityWebRequestAsyncOperation request = uwr.SendWebRequest();
                while (!request.isDone) await Task.Yield();

#if UNITY_2020_1_OR_NEWER
                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
#else
                if (uwr.isNetworkError || uwr.isHttpError)
#endif
                {
                    if (AI.Config.LogMediaDownloads) Debug.LogWarning($"Failed to download image from {imageUrl}: {uwr.error}");
                }
                else
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                    byte[] imageBytes = texture.EncodeToPNG();

                    Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
                    int retries = 3;
                    do
                    {
                        try
                        {
#if UNITY_2021_2_OR_NEWER
                            await File.WriteAllBytesAsync(targetFile, imageBytes);
                            break;
#else
                            File.WriteAllBytes(targetFile, imageBytes);
                            break;
#endif
                        }
                        catch (Exception e)
                        {
                            if (AI.Config.LogMediaDownloads) Debug.LogWarning($"Could not download image to {targetFile}, retrying: {e.Message}");

                            // can happen if file is locked (sharing violation)
                            retries--;
                            await Task.Delay(100);
                        }
                    } while (retries > 0);
                }
            }
        }

        // https://forum.unity.com/threads/handle-cannot-create-fmod-on-unitywebrequestmultimedia-getaudioclip.1139980/
        public static bool IsMp3File(string filePath)
        {
            byte[] mp3Header = {0xFF, 0xFB}; // Typical MP3 frame sync bits.
            byte[] id3Header = {0x49, 0x44, 0x33}; // 'ID3' in ASCII.
            byte[] bytes = new byte[3]; // Read the first three bytes of the file.

            using (FileStream file = File.OpenRead(filePath))
            {
                if (file.Length < 3)
                {
                    return false;
                }

                file.Read(bytes, 0, 3);
            }

            // Return true if we found an MP3 frame header or an ID3v2 tag.
            return bytes.SequenceEqual(mp3Header) || bytes.SequenceEqual(id3Header);
        }

        public static string GuessSafeName(string name, string replacement = "")
        {
            // remove special characters like Unity does when saving to disk
            // This will work in 99% of cases but sometimes items get renamed and
            // Unity will keep the old safe name so this needs to be synced with the 
            // download info API.
            string clean = name;

            // remove special characters
            clean = NoSpecialChars.Replace(clean, replacement);

            // remove duplicate spaces
            clean = Regex.Replace(clean, @"\s+", " ");

            return clean.Trim();
        }

        public static Dictionary<string, List<AssetInfo>> Guids2Files(List<string> guids, bool returnOriginIfNotFound = false, List<int> excludeIds = null)
        {
            Dictionary<string, List<AssetInfo>> result = new Dictionary<string, List<AssetInfo>>();
            Dictionary<string, AssetOrigin> origins = new Dictionary<string, AssetOrigin>();

            // Check if Unity can give us a definite origin (2023+), otherwise guids will hit multiple assets potentially
            foreach (string guid in guids)
            {
                result[guid] = new List<AssetInfo>(); // initialize

                AssetOrigin origin = AssetStore.GetAssetOrigin(guid);
                if (origin != null && origin.productId > 0) origins[guid] = origin;
            }

            List<AssetInfo> files = new List<AssetInfo>();
            if (origins.Count > 0)
            {
                string query = "with TempTable(ForeignId, Guid) as (";
                List<string> pairs = new List<string>();
                foreach (string guid in origins.Keys)
                {
                    pairs.Add($"select {origins[guid].productId}, '{guid}'");
                }
                query += string.Join(" union all ", pairs);
                query += ") select * from AssetFile inner join Asset on Asset.Id = AssetFile.AssetId ";
                query += "inner join TempTable on Asset.ForeignId = TempTable.ForeignId AND AssetFile.Guid = TempTable.Guid";

                if (excludeIds != null && excludeIds.Count > 0)
                {
                    query += $" where Asset.Id not in ({string.Join(",", excludeIds)})";
                    query += $" and Asset.ParentId not in ({string.Join(",", excludeIds)})"; // TODO: will misattribute children in deeper levels still
                }

                files.AddRange(DBAdapter.DB.Query<AssetInfo>($"{query}"));
            }
            List<string> noOrigin = guids.Except(origins.Keys).ToList();
            if (noOrigin.Count > 0)
            {
                string query = "select * from AssetFile inner join Asset on Asset.Id = AssetFile.AssetId where Guid in (";
                query += "'" + string.Join("','", noOrigin) + "')";

                if (excludeIds != null && excludeIds.Count > 0)
                {
                    query += $" and Asset.Id not in ({string.Join(",", excludeIds)})";
                    query += $" and Asset.ParentId not in ({string.Join(",", excludeIds)})"; // TODO: will misattribute children in deeper levels still
                }

                files.AddRange(DBAdapter.DB.Query<AssetInfo>($"{query}"));
            }

            // check for non-indexed assets
            List<string> nonIndexedGuids = guids.Except(files.Select(f => f.Guid)).ToList();
            if (nonIndexedGuids.Count > 0 && returnOriginIfNotFound)
            {
                foreach (string guid in nonIndexedGuids)
                {
                    AssetInfo ai = new AssetInfo();
                    ai.Guid = guid;
                    ai.CurrentState = Asset.State.Unknown;
                    if (origins.ContainsKey(guid))
                    {
                        ai.SafeName = origins[guid].packageName;
                        ai.ForeignId = origins[guid].productId;
                    }
                    files.Add(ai);
                }
            }

            // generate result
            files.ForEach(a =>
            {
                // add back origin information
                if (origins.TryGetValue(a.Guid, out AssetOrigin origin))
                {
                    a.Origin = origin;
                    a.DisplayName = origin.packageName;
                    a.Version = origin.packageVersion;
                    a.ProjectPath = origin.assetPath;
                    a.UploadId = origin.uploadId;
                }

                result[a.Guid].Add(a);
            });

            return result;
        }

        public static string ExtractGuidFromFile(string path)
        {
            string guid = null;
            try
            {
                using (StreamReader sr = new StreamReader(IOUtils.ToLongPath(path), Encoding.UTF8, true, 4096))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("guid:"))
                        {
                            guid = line;
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error reading guid from '{path}': {e.Message}");
                return null;
            }

            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogWarning($"Could not find guid in meta file: {path}");
                return null;
            }

            return guid.Substring(6);
        }

        public static bool IsOnURP()
        {
            RenderPipelineAsset rpa = GraphicsSettings.defaultRenderPipeline;
            if (rpa == null) return false;

            return rpa.GetType().Name.Contains("UniversalRenderPipelineAsset");
        }

        public static bool IsOnHDRP()
        {
            RenderPipelineAsset rpa = GraphicsSettings.defaultRenderPipeline;
            if (rpa == null) return false;

            return rpa.GetType().Name.Contains("HDRenderPipelineAsset");
        }

        public static string GetURPVersion()
        {
            PackageInfo packageInfo = PackageInfo.FindForAssetPath("Packages/com.unity.render-pipelines.universal");
            if (packageInfo == null) return null;

            return packageInfo.version.Split('.').First();
        }

        public static string GetHDRPVersion()
        {
            PackageInfo packageInfo = PackageInfo.FindForAssetPath("Packages/com.unity.render-pipelines.high-definition");
            if (packageInfo == null) return null;

            return packageInfo.version.Split('.').First();
        }

        public static bool IsUnityProject(string folder)
        {
            return Directory.Exists(Path.Combine(folder, "Assets"))
                && Directory.Exists(Path.Combine(folder, "Library"))
                && Directory.Exists(Path.Combine(folder, "Packages"))
                && Directory.Exists(Path.Combine(folder, "ProjectSettings"));
        }
    }
}