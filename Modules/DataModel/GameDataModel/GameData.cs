using SoR4_Studio.Modules.DataModel.GameDataModel.BeatThemAll;
using SoR4_Studio.Modules.DataModel.GameDataModel.BuiltIns;
using SoR4_Studio.Modules.DataModel.GameDataModel.FieldDescriber;
using SoR4_Studio.Modules.Utils.Protobuf.ProtoBinary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoR4_Studio.Modules.DataModel.GameDataModel;

internal class GameData : IDisposable
{
    public GameData(Chunks chunks)
    {
        _chunks = chunks;

        GenerateIDs();

        MetaGameConfigData = new(this);
        CharacterData = new(this);
        LevelData = new(this);
        DecorData = new(this);
        LocalizationData = new(this);
    }

    public int TotalChunkCount
    {
        get
        {
            return (from subChunks in Chunks.Values
                    from chunk in subChunks
                    select subChunks).Count();
        }
    }

    public Chunks? _chunks;
    public Chunks Chunks => _chunks ?? [];

    public ProtoField? this[FieldAddress address]
    {
        get
        {
            if (address.IsDirectKey)
            {
                return this[address.MainKey, 0]![address.Ints];
            }
            else // !address.IsDirectKey
            {
                return this[address.MainKey, address.SubKey!, address.Ints];
            }
        }
    }

    private ProtoField? this[string mainKey, string subKey, params int[] ints]
    {
        get
        {
            if (Chunks.TryGetValue(mainKey, out var subChunks)
                && subChunks.TryGetValue(subKey, out var chunk))
            {
                return chunk[ints];
            }
            else
            {
                return null;
            }
        }
    }

    private ProtoField? this[string mainKey, int index]
    {
        get
        {
            ProtoField? result = null;

            foreach (KeyValuePair<string, Chunk> subChunk in this[mainKey])
            {
                if (index < 0)
                {
                    break;
                }

                result = subChunk.Value.Root;

                index--;
            }

            return result;
        }
    }

    private IEnumerable<KeyValuePair<string, Chunk>> this[string mainKey]
    {
        get
        {
            if (!Chunks.TryGetValue(mainKey, out Dictionary<string, Chunk>? subChunks))
            {
                yield break;
            }

            foreach (KeyValuePair<string, Chunk> subChunk in subChunks)
            {
                yield return subChunk;
            }
        }
    }

    public MetaGameConfigData MetaGameConfigData { get; init; }
    public CharacterData CharacterData { get; init; }
    public LevelData LevelData { get; init; }
    public DecorData DecorData { get; init; }
    public LocalizationData LocalizationData { get; init; }

    public HashSet<string> CharacterIDs { get; init; } = [];
    public HashSet<string> AnimatedSpriteIDs { get; init; } = [];
    public HashSet<string> PickupIDs { get; init; } = [];
    public HashSet<string> LevelIDs { get; init; } = [];
    public HashSet<string> DestroyableIDs { get; init; } = [];
    public HashSet<string> FeedbackIDs { get; init; } = [];
    public HashSet<string> BtNodeIDs { get; init; } = [];
    public HashSet<string> DecorIDs { get; init; } = [];
    public HashSet<string> GuiNodeIDs { get; init; } = [];

    protected void GenerateIDs()
    {
        foreach (var chunk in this[MainKeys.CharacterData])
        {
            CharacterIDs.Add(chunk.Key);
        }
        foreach (var chunk in this[MainKeys.AnimatedSpriteData])
        {
            AnimatedSpriteIDs.Add(chunk.Key);
        }
        foreach (var chunk in this[MainKeys.PickupData])
        {
            PickupIDs.Add(chunk.Key);
        }
        foreach (var chunk in this[MainKeys.LevelData])
        {
            LevelIDs.Add(chunk.Key);
        }
        foreach (var chunk in this[MainKeys.DestroyableObjectData])
        {
            DestroyableIDs.Add(chunk.Key);
        }
        foreach (var chunk in this[MainKeys.FeedbackData])
        {
            FeedbackIDs.Add(chunk.Key);
        }
        foreach (var chunk in this[MainKeys.BtNodeData])
        {
            BtNodeIDs.Add(chunk.Key);
        }
        foreach (var chunk in this[MainKeys.DecorData])
        {
            DecorIDs.Add(chunk.Key);
        }
        foreach (var chunk in this[MainKeys.GuiNodeData])
        {
            GuiNodeIDs.Add(chunk.Key);
        }
    }

    #region IDisposable

    private bool _disposedValue = false;

    public void Dispose()
    {
        if (_disposedValue)
        {
            return;
        }

        foreach (var iChunk in from iPair in Chunks
                               from iChunk in iPair.Value
                               select iChunk.Value)
        {
            iChunk.Dispose();
        }

        _chunks = null;

        _disposedValue = true;

        GC.SuppressFinalize(this);
    }

    #endregion
}
