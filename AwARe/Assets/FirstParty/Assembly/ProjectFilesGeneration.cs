// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

/*
 * Inspired by https://github.com/tertle/com.bovinelabs.analyzers/blob/master/BovineLabs.Analyzers/ProjectFilesGeneration.cs
 */

#if UNITY_EDITOR

using System.IO;
using System.Text;
using System.Xml.Linq;
using UnityEditor;

namespace AwARe.Packages
{
    /// <summary>
    /// Responsible for adding additional files and other elements to the project file each time Unity updates it. <br/>
    /// For example, the StyleCop settings file is added by this class.
    /// </summary>
    [InitializeOnLoad]
    public class ProjectFilesGeneration : AssetPostprocessor
    {
        static ProjectFilesGeneration() { }

        /// <summary>
        /// Runs just after new CSProject file is generated. <br/>
        /// Adds additional assemblies to the new CSProject file specified in <c>AdditionalAssembly.xml</c>".
        /// </summary>
        /// <param name="path">Filepath of the csproj file.</param>
        /// <param name="contents">Content of the csproj file.</param>
        /// <returns>The content written to the Writer used.</returns>
        private static string OnGeneratedCSProject(string path, string contents)
        {
            // Set file path to additional assembly file
            const string FILEPATH_PLUS = @"Assets\FirstParty\Assembly\AdditionalAssembly.xml";
            string rootFolder = Directory.GetCurrentDirectory();
            string path_plus = Path.Combine(rootFolder, FILEPATH_PLUS);

            // Load and parse csproj file (contents) and additional assembly file
            XDocument doc = XDocument.Parse(contents);
            XDocument doc_plus = XDocument.Load(path_plus);
            
            // Upgrade the csproj content with the additional assembly content
            UpgradeProjectFile(doc, doc_plus, FILEPATH_PLUS);

            // Write new csproj content to the csproj file:
            using Utf8StringWriter stringWriter = new();
            doc.Save(stringWriter);
            return stringWriter.ToString();
        }

        /// <summary>
        /// Add all additional elements of the additional assembly file to the csproj project element.
        /// </summary>
        /// <param name="doc">Content of the csproj file.</param>
        /// <param name="doc_plus">Content of the additional assembly file.</param>
        /// <param name="path_plus">File path of the additional assembly file.</param>
        private static void UpgradeProjectFile(XDocument doc, XDocument doc_plus, string path_plus)
        {
            // Cop out if project element of additional elements group is missing
            XElement content = doc.Root;
            XElement content_plus = doc_plus.Root;
            if (content == null || content_plus == null)
                return;

            // Add comment before additional elements
            string comment = "Additional elements loaded from: " + path_plus;
            content.Add(new XComment(comment));

            // Add each additional element to the project
            foreach (XElement element in content_plus.Elements())
                content.Add(element);
        }

        /// <summary>
        /// Helper class for overwriting csproj file.
        /// </summary>
        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}

#endif