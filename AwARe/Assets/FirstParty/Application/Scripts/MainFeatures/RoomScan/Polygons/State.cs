// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

namespace AwARe.RoomScan.Polygons
{
    /// <summary>
    /// The different states within the Polygon scanning process.
    /// </summary>
    public enum State
    {
        Default,
        Scanning,
        SettingHeight,
        Saving
    }
}