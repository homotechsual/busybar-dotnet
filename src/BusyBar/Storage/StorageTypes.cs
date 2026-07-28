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
    /// <summary>File or directory name.</summary>
    public required string Name { get; init; }
}

/// <summary>A file entry within a directory listing.</summary>
public sealed record StorageFileElement : StorageListElement
{
    /// <summary>File size, in bytes.</summary>
    public required long Size { get; init; }
}

/// <summary>A directory entry within a directory listing.</summary>
public sealed record StorageDirElement : StorageListElement;

/// <summary>The contents of a directory on internal storage.</summary>
/// <param name="List">The directory's entries.</param>
public sealed record StorageList(IReadOnlyList<StorageListElement> List);

/// <summary>Internal storage usage.</summary>
/// <param name="UsedBytes">Bytes currently in use.</param>
/// <param name="FreeBytes">Bytes currently free.</param>
/// <param name="TotalBytes">Total size of the storage partition, in bytes.</param>
public sealed record StorageStatus(long UsedBytes, long FreeBytes, long TotalBytes);

/// <summary>Target path for a file upload.</summary>
/// <param name="Path">Destination path, under <c>/ext</c>, for the uploaded file.</param>
public sealed record StorageWriteParams(string Path);

/// <summary>Source path for a file download.</summary>
/// <param name="Path">Path, under <c>/ext</c>, of the file to download.</param>
public sealed record StorageReadParams(string Path);

/// <summary>Directory path to list.</summary>
/// <param name="Path">Path, under <c>/ext</c>, of the directory to list.</param>
public sealed record StorageListParams(string Path);

/// <summary>Target path for a file removal.</summary>
/// <param name="Path">Path, under <c>/ext</c>, of the file to remove.</param>
public sealed record StorageRemoveParams(string Path);

/// <summary>Target path for a new directory.</summary>
/// <param name="Path">Path, under <c>/ext</c>, of the directory to create.</param>
public sealed record StorageMkdirParams(string Path);

/// <summary>Source and destination paths for a rename/move operation.</summary>
/// <param name="Path">Current path, under <c>/ext</c>, of the file to move.</param>
/// <param name="NewPath">New path, under <c>/ext</c>, to move the file to.</param>
public sealed record StorageRenameParams(string Path, string NewPath);
