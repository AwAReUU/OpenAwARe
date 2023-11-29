// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

/*
 * TODO: rewrite comment and naming
 * Inspired by https://github.com/tertle/com.bovinelabs.analyzers/blob/master/BovineLabs.Analyzers/ProjectFilesGeneration.cs
 */

#if UNITY_EDITOR

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UnityEditor;

namespace AwARe.Packages
{
    [InitializeOnLoad]
    public class ProjectFilesGeneration : AssetPostprocessor
    {
        static ProjectFilesGeneration() { }

        private static string OnGeneratedCSProject(string path, string contents)
        {
            XDocument xml = XDocument.Parse(contents);

            UpgradeProjectFile(xml);

            // Write to the csproj file:
            using Utf8StringWriter str = new();
            xml.Save(str);
            return str.ToString();
        }

        /// <summary>
        ///   Add All extra files to assembly.
        /// </summary>
        /// <param name="doc"></param>
        private static void UpgradeProjectFile(XDocument doc)
        {
            XElement projectContentElement = doc.Root;
            if (projectContentElement == null)
                return;

            XNamespace xmlns = projectContentElement.Name.NamespaceName; // do not use var
            SetJSON(projectContentElement, xmlns);
        }

        /// <summary>
        ///  Add stylecop.json to csproj.
        /// </summary>
        private static void SetJSON(XElement projectContentElement, XNamespace xmlns)
        {
            var file = @"Assets\Packages\StyleCop\stylecop.json";

            var itemGroup = new XElement(xmlns + "ItemGroup");
            var reference = new XElement(xmlns + "AdditionalFiles");
            reference.Add(new XAttribute("Include", file));
            itemGroup.Add(reference);
            projectContentElement.Add(itemGroup);
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}

#endif