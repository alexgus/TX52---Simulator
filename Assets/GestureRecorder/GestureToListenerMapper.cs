using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;
using System.Xml;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
public class GestureToListenerMapper
{

    private Dictionary<String, String> gestureToListenerMap;
    private Dictionary<String, int> actionsNameToIndex;
    public GestureToListenerMapper(Manager.ActionCouple[] actions, string pathToMappingFile)
    {
        gestureToListenerMap = new Dictionary<String,String>();
        actionsNameToIndex = new Dictionary<string, int>();
        for (int i = 0; i < actions.Length; i++)
        {
            actionsNameToIndex.Add(actions[i].name, i);
        }
            this.ParseXML(pathToMappingFile);
    }

    public void ParseXML(string pathToXML)
    {
        XmlDocument doc = new XmlDocument();
        StringReader sReader = new StringReader(new StreamReader(pathToXML).ReadToEnd());
        doc.LoadXml(sReader.ReadToEnd());
        XmlNodeList mappingList = doc.DocumentElement.SelectNodes("/gestureToListener/mapping");
        Debug.Log("Mapping number of nodes :" + mappingList.Count);
        foreach (XmlNode node in mappingList)
        {
           String gestureName= node.SelectSingleNode("gesture").InnerText;
           String ListenerName = node.SelectSingleNode("listener").InnerText;
           Debug.Log("gesture '" + gestureName + "' is mapped to listener '" + ListenerName + "'");
           gestureToListenerMap.Add(gestureName, ListenerName);
        }
    }

    public int GetActionIndexFromGestureName(string gestureName)
    {
        string cleanName = this.CleanName(gestureName);
        if (gestureToListenerMap.ContainsKey(cleanName))
        {
            String listenerName = gestureToListenerMap[cleanName];

            if (actionsNameToIndex.ContainsKey(listenerName))
            {
                return actionsNameToIndex[listenerName];
            }
        }
        throw new Exception("Gesture is not mapped in file");
    }

    private String CleanName(String name)
    {
        String cleanName= name.Substring(name.LastIndexOf("/")+1);
        Debug.Log(cleanName);
        return cleanName;
        
    }
}
