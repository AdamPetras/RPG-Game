using System.Collections.Generic;
using System.Xml;
using Assets.Scripts.InventoryFolder;
using UnityEngine;

namespace Assets.Scripts.Extension
{
    public class XmlDatabaseReader
    {
        private TextAsset _textAsset;
        private Dictionary<string, string> _dbDictionary;
        public XmlDatabaseReader(TextAsset textAsset)
        {
            _textAsset = textAsset;           
        }
        public void ReadItemsFromDatabase(string firstElement, List<Dictionary<string, string>> _dbDictionaryList, params string[] otherElements)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(_textAsset.text);
            XmlNodeList itemList = xml.GetElementsByTagName(firstElement);
            foreach (XmlNode itemInfo in itemList)
            {
                XmlNodeList itemContentList = itemInfo.ChildNodes;
                _dbDictionary = new Dictionary<string, string>();   //ID : číslo
                foreach (XmlNode content in itemContentList)
                {
                    Comparing(content,_dbDictionaryList,otherElements);
                }
                _dbDictionaryList.Add(_dbDictionary);
            }
        }

        private void Comparing(XmlNode content , List<Dictionary<string, string>> _dbDictionaryList, params string[] otherElements)
        {
            foreach (string element in otherElements)
            {
                if (content.Name == element)
                {
                    _dbDictionary.Add(element, content.InnerText);
                }
            }           
        }
    }
}
