using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections.LowLevel.Unsafe.NotBurstCompatible;
using Unity.Serialization.Binary;
using Unity.Serialization.Json;
using UnityEngine;

public static class SaveSystem
{
    public const string fileExtension = "data";

    public static string currentSaveName;

    public static event Action OnUpdateSaveContent;
    public static event Action OnLoadSaveContent;
    public static SaveContent saveContent = new();

    public static void Save(string fileName)
    {
        Debug.Log($"save: {Application.persistentDataPath}/{fileName}.{fileExtension}");
        OnUpdateSaveContent?.Invoke();
        BinarySave(saveContent, fileName);
#if DEBUG
        JsonSave(saveContent, fileName);
#endif
    }

    public static void Load(string fileName)
    {
        currentSaveName = fileName;

#if DEBUG
        if (BinaryLoad(fileName))
        {
            Debug.Log("loaded binary save");
        }
        else
        {
            if (JsonLoad(fileName))
            {
                Debug.Log("loaded json save");
            }
            else
            {
                Debug.Log("No valid save file found");
            }
        }
#else
        BinaryLoad(fileName);
#endif
        OnLoadSaveContent?.Invoke();
    }

    public static void DeleteSave(string fileName)
    {
        File.Delete($"{Application.persistentDataPath}/{fileName}.{fileExtension}");
    }

    private static readonly JsonSerializationParameters JsonSerializationParameters = new()
    {
        SerializedType = typeof(SaveContent),
        RequiresThreadSafety = true,
        UserDefinedAdapters = new List<IJsonAdapter>
        {
            new QuestLocationJsonAdapter(),
        }
    };

    private static void JsonSave(object data, string fileName)
    {
        string json = JsonSerialization.ToJson(data, JsonSerializationParameters);
        File.WriteAllText($"{Application.persistentDataPath}/{fileName}.json", json);
    }

    private static bool JsonLoad(string fileName)
    {
        if (File.Exists($"{Application.persistentDataPath}/{fileName}.json"))
        {
            try
            {
                string file = File.ReadAllText($"{Application.persistentDataPath}/{fileName}.json");
                saveContent = JsonSerialization.FromJson<SaveContent>(file, JsonSerializationParameters);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }        
        else
        {
            saveContent = new();
        }

        return false;
    }

    private static readonly BinarySerializationParameters BinarySerializationParameters = new()
    {
        SerializedType = typeof(SaveContent),
        RequiresThreadSafety = true,
        UserDefinedAdapters = new List<IBinaryAdapter>
        {
            new QuestLocationBinaryAdapter(),
            // new SerializableScriptableObjectJsonAdapter(),
            // new SerializableObjectSus(),
        }
    };

    private static unsafe void BinarySave(object data, string fileName)
    {
        UnsafeAppendBuffer stream = new(16, 8, Allocator.Temp);
        try
        {
            BinarySerialization.ToBinary(&stream, data, BinarySerializationParameters);
            byte[] bytes = stream.ToBytesNBC();
            File.WriteAllBytes($"{Application.persistentDataPath}/{fileName}.{fileExtension}", bytes);
        }
        finally
        {
            stream.Dispose();
        }
    }

    private static bool BinaryLoad(string fileName)
    {
        string path = $"{Application.persistentDataPath}/{fileName}.{fileExtension}";
        if (File.Exists(path))
        {
            byte[] data = File.ReadAllBytes(path);
            UnsafeAppendBuffer buffer = new(data.Length, 8, Allocator.Temp);
            try
            {
                unsafe
                {
                    fixed (byte* dataPtr = data)
                    {
                        buffer.Add(dataPtr, data.Length);
                    }

                    var reader = buffer.AsReader();
                    saveContent = BinarySerialization.FromBinary<SaveContent>(&reader, BinarySerializationParameters);
                    return true;
                }
            }
            finally
            {
                buffer.Dispose();
            }
        }
        else
        {
            saveContent = new();
        }

        return false;
    }
}