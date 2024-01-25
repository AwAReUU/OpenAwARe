// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \* 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using AwARe.Data.Logic;
using AwARe.RoomScan.Polygons.Objects;
using System;

using Newtonsoft.Json;

namespace AwARe
{
    /// <summary>
    /// Class <c>PolygonSaveLoadManager</c> is responsible for managing the storage of polygons on the disc. (Done and loading).
    /// </summary>
    public class SaveLoadManager : MonoBehaviour
    {
        private string directoryPath;
        public string DirectoryPath =>
            directoryPath;

        private void Start()
        {
            directoryPath = Application.persistentDataPath;
            // Create the directory if it doesn't exist
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
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
                string jsonData = JsonConvert.SerializeObject(data);
                File.WriteAllText(jsonFilePath, jsonData);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error writing to file: {ex.Message}");
            }
        }

        public T LoadDataFromJson<T>(string fileName)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                Debug.LogError("path is null or empty.");
                return default(T);
            }

            string jsonFilePath = Path.Combine(directoryPath, fileName);

            if (File.Exists(jsonFilePath))
            {
                try
                {
                    string jsonData = File.ReadAllText(jsonFilePath);
                    return JsonConvert.DeserializeObject<T>(jsonData);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error reading file {fileName}: {ex.Message}");
                    return default(T);
                }
            }
            else
            {
                Debug.LogWarning("File not found: " + jsonFilePath);
                return default(T);
            }
        }

    }
}
