using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using AwARe.Data.Logic;
using AwARe.RoomScan.Polygons.Objects;
using System;

namespace AwARe
{
    public class PolygonSaveLoadManager : MonoBehaviour
    {
        private static PolygonSaveLoadManager instance;

        private string directoryPath;
        public string DirectoryPath
        {
            get { return directoryPath; }
        }
        public string GetDirectoryPath()
        {
            return directoryPath;
        }

        private void Start()
        {
            directoryPath = Application.persistentDataPath;
            // Create the directory if it doesn't exist
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            Debug.Log($"PolygonSaveLoadManager - Directory Path: {directoryPath}");
        }

        public void PerformSaveTest()
        {
            string testFilePath = Path.Combine(directoryPath, "TestFile.json");
            string testData = "{ \"test\": \"data\" }";

            try
            {
                File.WriteAllText(testFilePath, testData);
                Debug.Log("Test Write Success!");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error writing test data: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves the provided data object as JSON to a specified file.
        /// </summary>
        /// <param name="fileName">The name of the file to save the JSON data to.</param>
        /// <param name="data">The object containing the data to be serialized to JSON.</param>
        /// 
        public void SaveDataToJson<T>(string fileName, T data)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                Debug.LogError(" path is null or empty.");
                return;
            }

            string jsonFilePath = Path.Combine(directoryPath, fileName);

            try
            {
                string jsonData = JsonUtility.ToJson(data);
                Debug.Log("JSON data " + jsonData);
                File.WriteAllText(jsonFilePath, jsonData);
                Debug.Log("Write success!");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error writing to file: {ex.Message}");
            }
        }

        public T LoadDataFromJson<T>(string fileName)
        {
            Debug.Log($"PolygonSaveLoadManager - Directory Path: {directoryPath}");
            if (string.IsNullOrEmpty(directoryPath))
            {
                Debug.LogError("path is null or empty.");
                return default(T);
            }

            string jsonFilePath = Path.Combine(directoryPath, fileName );

            if (File.Exists(jsonFilePath))
            {
                string jsonData = File.ReadAllText(jsonFilePath);
                return JsonUtility.FromJson<T>(jsonData);
            }
            else
            {
                Debug.LogWarning("File not found: " + jsonFilePath);
                return default(T);
            }
        }
        
    }
}
