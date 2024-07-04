using UnityEngine;
using System.Collections.Generic;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using NaughtyAttributes;
using System;
using System.Threading.Tasks;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class DownloadManager : MonoBehaviour
{
    public string bucketName = "cpwizard.bucket";
    private IAmazonS3 s3Client;

    // Define the path where you want to store the downloaded files within your Unity project
    private string downloadFolderPath;

    public TMP_Dropdown filesDropdown; // Reference to the Dropdown element for listing files
    public Button listDownloadedFilesButton; // Reference to the List Downloaded Files button

    void Start()
    {
        // Configure S3 client
        AmazonS3Config s3Config = new AmazonS3Config
        {
            RegionEndpoint = Amazon.RegionEndpoint.EUCentral1,
            Timeout = TimeSpan.FromMinutes(10),
            ReadWriteTimeout = TimeSpan.FromMinutes(10)
        };

        s3Client = new AmazonS3Client("YOUR ACCESS KEY HERE", s3Config);

        // Set the download folder path to a directory within your Assets folder
        downloadFolderPath = Path.Combine(Application.dataPath, "DownloadedFiles");

        // Ensure the download directory exists
        if (!Directory.Exists(downloadFolderPath))
        {
            Directory.CreateDirectory(downloadFolderPath);
        }

        // Add listener for the List Downloaded Files button
        listDownloadedFilesButton.onClick.AddListener(ListDownloadedFiles);
    }

    [Button]
    public async void FetchAndDownloadFiles()
    {
        List<string> fileNames = await GetFileNamesFromBucket(bucketName);

        // Log the file names
        foreach (var fileName in fileNames)
        {
            Debug.Log($"File: {fileName}");
        }

        // Download each file
        foreach (var fileName in fileNames)
        {
            await DownloadFileFromS3(bucketName, fileName);
        }
    }

    private async Task<List<string>> GetFileNamesFromBucket(string bucketName)
    {
        List<string> fileNames = new List<string>();

        try
        {
            ListObjectsV2Request request = new ListObjectsV2Request
            {
                BucketName = bucketName
            };

            ListObjectsV2Response response;
            do
            {
                response = await s3Client.ListObjectsV2Async(request);

                foreach (S3Object entry in response.S3Objects)
                {
                    fileNames.Add(entry.Key);
                }

                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);
        }
        catch (AmazonS3Exception e)
        {
            Debug.LogError($"Error fetching file names: {e.Message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Unexpected error: {e.Message}");
        }

        return fileNames;
    }

    private async Task DownloadFileFromS3(string bucketName, string fileName)
    {
        try
        {
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = fileName
            };

            GetObjectResponse response = await s3Client.GetObjectAsync(request);

            // Save the downloaded file to the specified directory within your Unity project
            string filePath = Path.Combine(downloadFolderPath, fileName);
            using (var fileStream = File.Create(filePath))
            {
                await response.ResponseStream.CopyToAsync(fileStream);
            }

            Debug.Log($"File downloaded successfully: {filePath}");
        }
        catch (AmazonS3Exception e)
        {
            Debug.LogError($"Error downloading file {fileName}: {e.Message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Unexpected error: {e.Message}");
        }
    }

    private void ListDownloadedFiles()
    {
        List<string> fileNames = new List<string>();

        try
        {
            string[] files = Directory.GetFiles(downloadFolderPath);
            foreach (string filePath in files)
            {
                fileNames.Add(Path.GetFileName(filePath));
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error listing downloaded files: {e.Message}");
        }

        filesDropdown.ClearOptions();
        filesDropdown.AddOptions(fileNames);
    }
}
