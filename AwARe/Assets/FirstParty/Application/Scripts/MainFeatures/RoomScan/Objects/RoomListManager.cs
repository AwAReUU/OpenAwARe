// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using UnityEngine;

namespace AwARe.RoomScan.Objects
{
    /// <summary>
    /// Contains the Room and handles the different states within the Polygon scanning.
    /// </summary>
    public class RoomListManager : MonoBehaviour
    {
        private RoomListSerialization roomListSerialization;
        public List<Data.Logic.Room> Rooms { get; private set; }

        private SaveLoadManager saveLoadManager;
        [SerializeField] private ScreenshotManager screenshotManager;

        private void Awake()
        {
            saveLoadManager = new();
        }

        public void SaveRoom(Data.Logic.Room room, List<Vector3> anchors, List<Texture2D> screenshots)
        {
            if(Rooms.Contains(room))
                UpdateRoom(room, anchors, screenshots);
            else
                AddRoom(room, anchors, screenshots);
        }

        public void DeleteRoom(Data.Logic.Room room, List<Texture2D> screenshots)
        {
            for (var i = 0; i < screenshots.Count; i++)
                screenshotManager.DeleteScreenshot(room, i);

            var idx = Rooms.FindIndex(r => r == room);
            Rooms.RemoveAt(idx);
            roomListSerialization.Rooms.RemoveAt(idx);
            saveLoadManager.SaveRoomList("rooms", roomListSerialization);
        }

        public void UpdateRoom(Data.Logic.Room room, List<Vector3> anchors, List<Texture2D> screenshots)
        {
            for (var i = 0; i < screenshots.Count; i++)
                screenshotManager.SaveScreenshot(screenshots[i], room, i);

            var idx = Rooms.FindIndex(r => r == room);
            Rooms[idx] = room;
            roomListSerialization.Rooms[idx] = new(room, anchors);
            saveLoadManager.SaveRoomList("rooms", roomListSerialization);
        }

        public void AddRoom(Data.Logic.Room room, List<Vector3> anchors, List<Texture2D> screenshots)
        {
            for(var i = 0; i < screenshots.Count; i++)
                screenshotManager.SaveScreenshot(screenshots[i], room, i);

            Rooms.Add(room);
            roomListSerialization.Rooms.Add(new(room, anchors));
            saveLoadManager.SaveRoomList("rooms", roomListSerialization);
        }

        /// <summary>
        /// Load in a list of rooms from roomListSerialization rooms.
        /// </summary>
        /// <returns>The list of rooms.</returns>
        public Data.Logic.Room LoadRoom(int idx, List<Vector3> anchors)
        {
            return roomListSerialization.Rooms?[idx].ToRoom(anchors);
        }

        /// <summary>
        /// Load in a list of rooms from roomListSerialization rooms.
        /// </summary>
        /// <returns>The list of rooms.</returns>
        public RoomListSerialization LoadList(List<Vector3> anchors)
        {
            roomListSerialization = saveLoadManager.LoadRooms("rooms") ?? new();
            return roomListSerialization;
        }
    }
}