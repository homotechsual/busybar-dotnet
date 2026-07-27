using System.Text.Json.Serialization;

namespace Busy.Bar;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(StorageFileElement), "file")]
[JsonDerivedType(typeof(StorageDirElement), "dir")]
public abstract record StorageListElement
{
    public required string Name { get; init; }
}

public sealed record StorageFileElement : StorageListElement
{
    public required long Size { get; init; }
}

public sealed record StorageDirElement : StorageListElement;

public sealed record StorageList(IReadOnlyList<StorageListElement> List);

public sealed record StorageStatus(long UsedBytes, long FreeBytes, long TotalBytes);

public sealed record StorageWriteParams(string Path);

public sealed record StorageReadParams(string Path);

public sealed record StorageListParams(string Path);

public sealed record StorageRemoveParams(string Path);

public sealed record StorageMkdirParams(string Path);

public sealed record StorageRenameParams(string Path, string NewPath);
