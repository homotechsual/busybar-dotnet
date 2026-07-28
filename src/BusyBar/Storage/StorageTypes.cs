using System.Text.Json.Serialization;

namespace Busy.Bar;

/// <summary>
/// System.Text.Json's polymorphic deserialization requires the "type" discriminator to be the first
/// property in the JSON object, or it throws <see cref="NotSupportedException"/>. Confirmed against a
/// real BUSY Bar device (see <c>RealDeviceFixtureTests</c>) that "type" is always first for both
/// "file" and "dir" elements returned by GET /storage/list — treated as a confirmed assumption, not
/// a theoretical risk.
/// </summary>
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
