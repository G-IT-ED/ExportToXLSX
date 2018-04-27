using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using RetroToFileExporter.Core.Interfaces;
using RetroToFileExporter.Core.Objects;

namespace RetroToFileExporter.Core.Models
{
    [Serializable]
    [XmlRoot(
            Namespace = "rsdu.ema",
            ElementName = "RetroExporter",
            DataType = "string",
            IsNullable = false)
    ]
    public class Action : IAction
    {
        public Action ()
        {
            
        }
        
        [XmlElement("NameTrigger")]
        public string NameTrigger { get; set; }
        [XmlElement("Info")]
        public bool Info { get; set; }
        [XmlElement("ObjectField")]
        public bool ObjectField { get; set; }
        [XmlElement("TypeParam")]
        public bool TypeParam { get; set; }
        [XmlElement("IdParam")]
        public bool IdParam { get; set; }
        [XmlElement("IdTableParam")]
        public bool IdTableParam { get; set; }
        [XmlElement("NameArchTable")]
        public bool NameArchTable { get; set; }
        [XmlElement("StateColumnHeader")]
        public bool StateColumnHeader { get; set; }
        [XmlElement("FilesFolder")]
        public string FilesFolder { get; set; }
        [XmlElement("FileName")]
        public string FileName { get; set; }
        [XmlElement("TagTime")]
        public string TagTime { get; set; }
        [XmlElement("IsHighLevelReliability")]
        public bool IsHighLevelReliability { get; set; }
        [XmlElement("VersionExcel")]
        public string VersionExcel { get; set; }
        [XmlElement("GuidAction")]
        public Guid GuidAction { get; set; }
        [XmlElement("KadrId")]
        public int KadrId { get; set; }
        [XmlElement("QueryConditions")]
        public List<QueryCondition> QueryConditions { get; set; }
        [XmlElement("RunTimeOffset")]
        public int RunTimeOffset { get; set; }

        public string ToXml()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Action));
            StringWriter sw = new StringWriter();
            serializer.Serialize(sw, this);
            return sw.ToString();
        }

        public Action(Guid guidAction, string name, string filesFolder, string fileName, string tagTime,
            string versionExcel, bool info, bool objectField, bool typeParam, bool idParam, bool idTableParam,
            bool nameArchTable, bool stateColumnHeader, bool isHighLevelReliability, int kadr, List<QueryCondition> queryConditions, int runTimeOffset)
        {
            GuidAction = guidAction;
            NameTrigger = name;
            FilesFolder = filesFolder;
            FileName = fileName;
            TagTime = tagTime;
            VersionExcel = versionExcel;
            Info = info;
            ObjectField = objectField;
            TypeParam = typeParam;
            IdParam = idParam;
            IdTableParam = idTableParam;
            NameArchTable = nameArchTable;
            StateColumnHeader = stateColumnHeader;
            IsHighLevelReliability = isHighLevelReliability;
            KadrId = kadr;
            QueryConditions = queryConditions;
            RunTimeOffset = runTimeOffset;
        }

        public override string ToString()
        {
            return NameTrigger;
        }
        
        public override bool Equals(object obj)
        {
            try
            {
                var action = (Action)obj;
                return action != null && GuidAction == action.GuidAction;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return KadrId + FileName.Length;
        }
    }
}