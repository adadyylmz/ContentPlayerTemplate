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

    private string downloadFolderPath;

    public TMP_Dropdown filesDropdown;
    public Button listDownloadedFilesButton;

    void Start()
    {
        AmazonS3Config s3Config = new AmazonS3Config
        {
            RegionEndpoint = Amazon.RegionEndpoint.EUCentral1,
            Timeout = TimeSpan.FromMinutes(10),
            ReadWriteTimeout = TimeSpan.FromMinutes(10)
        };

        s3Client = new AmazonS3Client("","", s3Config);

        downloadFolderPath = Path.Combine(Application.dataPath, "DownloadedFiles");

        if (!Directory.Exists(downloadFolderPath))
        {
            Directory.CreateDirectory(downloadFolderPath);
        }

        listDownloadedFilesButton.onClick.AddListener(ListDownloadedFiles);
    }

    // Fetches file names from the S3 bucket and downloads them
    [Button]
    public async void FetchAndDownloadFiles()
    {
        List<string> fileNames = await GetFileNamesFromBucket(bucketName);

        foreach (var fileName in fileNames)
        {
            Debug.Log($"File: {fileName}");
        }

        foreach (var fileName in fileNames)
        {
            await DownloadFileFromS3(bucketName, fileName);
        }
    }

    // Retrieves a list of file names from the specified S3 bucket
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

    // Downloads a specific file from the S3 bucket to the local download folder
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

    // Lists all downloaded files in the specified folder and populates the dropdown UI with them
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
