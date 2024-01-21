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
using Newtonsoft.Json;
using AwARe.Data.Logic;
using AwARe.RoomScan.Polygons.Objects;
using System;

namespace AwARe
{
    /// <summary>
    /// Class <c>PolygonSaveLoadManager</c> is responsible for managing the storage of polygons on the disc. (Done and loading).
    /// </summary>
    public class SaveLoadManager : MonoBehaviour
    {
        public string directoryPath;
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
                Debug.LogError("Path is null or empty.");
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

        /// <summary>
        /// Saves a list of rooms to a JSON file.
        /// </summary>
        /// <param name="fileName">The name of the file to save the JSON data to.</param>
        /// <param name="rooms">The list of rooms to be serialized and saved.</param>
        public void SaveRooms(string fileName, List<RoomSerialization> rooms)
        {
            SaveDataToJson(fileName, rooms);
        }

        // <summary>
        /// Loads a list of rooms from a JSON file.
        /// </summary>
        /// <param name="fileName">The name of the JSON file to load data from.</param>
        /// <returns>The list of deserialized rooms.</returns>

        public List<RoomSerialization> LoadRooms(string fileName)
        {
            return LoadDataFromJson<List<RoomSerialization>>(fileName) ?? new List<RoomSerialization>();
        }


        /// <summary>
        /// Loads data of type T from a JSON file with the specified fileName using the save load manager.
        /// </summary>
        /// <typeparam name="T">Type of data to load.</typeparam>
        /// <param name="fileName">The name of the JSON file to load data from.</param>
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
                    return JsonUtility.FromJson<T>(jsonData);
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
