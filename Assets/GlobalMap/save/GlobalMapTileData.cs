using System;

[Serializable]
public struct GlobalMapTileData
{
    public GlobalMapTileModifier modifier;
    public int tileVariantId;
    // public Location location;

    public bool Equals(GlobalMapTileData other)
    {
        return modifier == other.modifier && tileVariantId == other.tileVariantId;
    }

    public override bool Equals(object obj)
    {
        return obj is GlobalMapTileData other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)modifier, tileVariantId);
    }

    public static bool operator ==(GlobalMapTileData left, GlobalMapTileData right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GlobalMapTileData left, GlobalMapTileData right)
    {
        return !left.Equals(right);
    }
}
