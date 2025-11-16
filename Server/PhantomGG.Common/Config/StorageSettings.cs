namespace PhantomGG.Common.Config;

public class StorageSettings
{
    public string Provider { get; set; } = "LocalFile";
    public string? AzureStorageConnectionString { get; set; }
    public string BlobContainerName { get; set; } = "images";
}
