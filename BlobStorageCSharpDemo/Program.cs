using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BlobStorageCSharpDemo
{
    public static class Program
    {
        const string FolderPath = @"C:\blobstoragecsharp";
        private const string connstring = "<copy your storage account connection string here>";

        public static void Main()
        {
            Console.WriteLine("Azure Blob storage Demo - about to upload images");
            Console.WriteLine();
            ProcessAsync().GetAwaiter().GetResult();

            Console.WriteLine("Press any key to exit the application.");
            Console.ReadLine();
        }

        private static async Task ProcessAsync()
        {
            string storageConnectionString = connstring;

            if (CloudStorageAccount.TryParse(storageConnectionString, out var storageAccount))
            {
                try
                {
                    var blobClient = storageAccount.CreateCloudBlobClient();

                    var cloudBlobContainer = blobClient.GetContainerReference("xmasimages-container");
                    await cloudBlobContainer.CreateIfNotExistsAsync();

                    var permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);

                    foreach (var filePath in Directory.GetFiles(FolderPath, "*.*", SearchOption.AllDirectories))
                    {
                        var blob = cloudBlobContainer.GetBlockBlobReference(filePath);
                        await blob.UploadFromFileAsync(filePath);

                        Console.WriteLine("Uploaded {0}", filePath);
                    }

                    Console.WriteLine("Completed uploading images");
                }
                catch (StorageException ex)
                {
                    Console.WriteLine("Error returned from the service: {0}", ex.Message);
                }
            }
        }
    }
}
