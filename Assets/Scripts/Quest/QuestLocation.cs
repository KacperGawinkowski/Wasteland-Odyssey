using System;
using System.Linq;
using Unity.Serialization.Binary;
using Unity.Serialization.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = nameof(QuestLocation), menuName = "Quests/" + nameof(QuestLocation))]
public class QuestLocation : ScriptableObject
{
    // public string locationSceneName;

    public QuestLocationController[] questLocationVariants;

    public string locationName;
    public TileBase locationTile;
    public TileBase selectedLocationTile;

    public int minEnemiesCount = 1;
    public int maxEnemiesCount = 1;

    private static QuestLocation[] s_QuestLocations;

    public static QuestLocation[] QuestLocations
    {
        get
        {
            if (s_QuestLocations == null)
            {
                s_QuestLocations = Resources.LoadAll<QuestLocation>("QuestLocations/Random").ToArray();
            }

            return s_QuestLocations;
        }
    }

    public static QuestLocation LastQuestLocation => Resources.Load<QuestLocation>("QuestLocations/Story/LastQuest");

    public static QuestLocation GetQuestLocationById(string name)
    {
        return Resources.Load<QuestLocation>($"QuestLocations/Random/{name}") ?? Resources.Load<QuestLocation>($"QuestLocations/Story/{name}");
    }
}

public class QuestLocationJsonAdapter : IJsonAdapter<QuestLocation>
{
    public void Serialize(in JsonSerializationContext<QuestLocation> context, QuestLocation value)
    {
        if (value != null)
        {
            context.Writer.WriteValue(value.name);
        }
        else
        {
            context.Writer.WriteNull();
        }
    }

    public QuestLocation Deserialize(in JsonDeserializationContext<QuestLocation> context)
    {
        if (context.SerializedValue.IsNull())
        {
            return null;
        }

        return QuestLocation.GetQuestLocationById(context.SerializedValue.ToString());
    }
}

public class QuestLocationBinaryAdapter : IBinaryAdapter<QuestLocation>
{
    public unsafe void Serialize(in BinarySerializationContext<QuestLocation> context, QuestLocation value)
    {
        if (value != null)
        {
            char[] chars = value.name.ToCharArray();
            fixed (char* dataPtr = chars)
            {
                context.Writer->AddArray<char>(dataPtr, chars.Length);
            }
        }
        else
        {
            char[] chars = Array.Empty<char>();
            fixed (char* dataPtr = chars)
            {
                context.Writer->AddArray<char>(dataPtr, chars.Length);
            }
        }
    }

    public unsafe QuestLocation Deserialize(in BinaryDeserializationContext<QuestLocation> context)
    {
        int length;
        char* chars = (char*)context.Reader->ReadNextArray<char>(out length);

        string s = new(chars, 0, length);
        Debug.Log($"{length} - {s}");

        if (length > 0)
        {
            return QuestLocation.GetQuestLocationById(s);
        }

        return null;
    }
}