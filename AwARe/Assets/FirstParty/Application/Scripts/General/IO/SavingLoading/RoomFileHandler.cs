// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.IO;
using AwARe.Data.Objects;
using UnityEngine;

namespace AwARe
{
    /// <summary>
    /// Handles the loading and parsing of a local JSON file to a <see cref="List{T}"/> of <see cref="Room"/> and vice versa.
    /// </summary>
    public class RoomFileHandler
    {
        readonly string filePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomFileHandler"/> class.
        /// </summary>
        public RoomFileHandler()
        {
            filePath = Application.persistentDataPath;
            // Create the directory if it doesn't exist
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
        }

        /// <summary>
        /// Saves the provided data object as JSON to a specified file.
        /// </summary>
        /// <param name="fileName">The name of the file to save the JSON data to.</param>
        /// <param name="data">The object containing the data to be serialized to JSON.</param>
        public void SaveDataToJson<T>(string fileName, T data)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogError(" path is null or empty.");
                return;
            }
            string jsonFilePath = Path.Combine(filePath, fileName);
            try
            {
                string jsonData = JsonUtility.ToJson(data);
                File.WriteAllText(jsonFilePath, jsonData);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error writing to file: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves a room list to a JSON file.
        /// </summary>
        /// <param name="fileName">The name of the file to save the JSON data to.</param>
        /// <param name="roomList">The room list to be serialized and saved.</param>
        public void SaveRoomList(string fileName, RoomListSerialization roomList)
        {
            SaveDataToJson(fileName, roomList);
        }

        /// <summary>
        /// Loads a room list from a JSON file.
        /// </summary>
        /// <param name="fileName">The name of the JSON file to load data from.</param>
        /// <returns>The deserialized room list.</returns>
        public RoomListSerialization LoadRooms(string fileName) =>
            LoadDataFromJson<RoomListSerialization>(fileName);

        /// <summary>
        /// Loads data of type T from a JSON file with the specified fileName.
        /// </summary>
        /// <typeparam name="T">Type of data to load.</typeparam>
        /// <param name="fileName">The name of the JSON file to load data from.</param>
        public T LoadDataFromJson<T>(string fileName)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogError("File path is null or empty.");
                return default;
            }

            string jsonFilePath = Path.Combine(filePath, fileName);

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
                    return default;
                }
            }
            else
            {
                Debug.LogWarning("File not found: " + jsonFilePath);
                return default;
            }
        }


    }
}