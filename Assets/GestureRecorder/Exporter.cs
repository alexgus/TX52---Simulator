using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;
using System.Xml;
using System.Collections.Generic;

public class Exporter{

    public static void export(DynGesture ges)
    {
        XElement listMC = new XElement("listMC");

        foreach(GestureAnalyzer.MovementCheckpoint mc in ges.Movement.checkpoints){
            XElement listJM = new XElement("listJM");

            foreach (GestureAnalyzer.JointMovement jm in mc.jointMovements){
                listJM.Add(new XElement("jointMouvement",
                    new XElement("id", jm.joint),
                    new XElement("percent", jm.percent),
                    new XElement("initialPosition",
                        new XElement("w", jm.initialPosition.w),
                        new XElement("x", jm.initialPosition.x),
                        new XElement("y", jm.initialPosition.y),
                        new XElement("z", jm.initialPosition.z)
                        ),
                    new XElement("finalPosition", 
                        new XElement("w", jm.finalPosition.w),
                        new XElement("x", jm.finalPosition.x),
                        new XElement("y", jm.finalPosition.y),
                        new XElement("z", jm.finalPosition.z)
                        ),
                    new XElement("initialAbsolutePosition",
                        new XElement("w", jm.initialAbsolutePosition.w),
                        new XElement("x", jm.initialAbsolutePosition.x),
                        new XElement("y", jm.initialAbsolutePosition.y),
                        new XElement("z", jm.initialAbsolutePosition.z)
                        ),
                    new XElement("finalAbsolutePosition", 
                        new XElement("w", jm.finalPosition.w),
                        new XElement("x", jm.finalPosition.x),
                        new XElement("y", jm.finalPosition.y),
                        new XElement("z", jm.finalPosition.z)
                        ))
                );
            }


            listMC.Add(new XElement("mouvementCheckPoint",
                    new XElement("duration",mc.duration),
                    listJM
                )
            );
        }

        XElement root = 
            new XElement("dyngesture",
                new XElement("name",ges.Name),
                new XElement("isRepetitive", ges.IsRepetitive),
                new XElement("ignoreOtherJoints",ges.IgnoreOtherJoints),
                new XElement("mouvement",
                      new XElement("duration",ges.Movement.duration),
                      listMC
                )
            );

        root.Save("./Assets/RecordedGesture/"+ges.Name+".xml");
    }

    public static DynGesture import(string filename)
    {
        DynGesture ges = new DynGesture();

        XmlDocument doc = new XmlDocument();
        System.IO.StringReader stringReader = new System.IO.StringReader(new System.IO.StreamReader(filename).ReadToEnd());
        doc.LoadXml(stringReader.ReadToEnd()); 
        
        ges.Name = doc.DocumentElement.SelectSingleNode("/dyngesture/name").InnerText;
        ges.IgnoreOtherJoints = bool.Parse(doc.DocumentElement.SelectSingleNode("/dyngesture/ignoreOtherJoints").InnerText);
        ges.IsRepetitive = bool.Parse(doc.DocumentElement.SelectSingleNode("/dyngesture/isRepetitive").InnerText);


        XmlNodeList listMC = doc.DocumentElement.SelectNodes("/dyngesture/mouvement/listMC/mouvementCheckPoint");
        List<GestureAnalyzer.MovementCheckpoint> lMoveCheckpoint = new List<GestureAnalyzer.MovementCheckpoint>();
        Debug.Log("listMC size " + listMC.Count);
        foreach (XmlNode mc in listMC)
        {
            float duration = float.Parse(mc.SelectSingleNode("duration").InnerText);
            XmlNodeList listJM = mc.LastChild.SelectNodes("jointMouvement");

            List<GestureAnalyzer.JointMovement> jmList = new List<GestureAnalyzer.JointMovement>();
            Debug.Log("listJM size " + listJM.Count);
            foreach (XmlNode n in listJM)
            {
                GestureAnalyzer.JointMovement jm = new GestureAnalyzer.JointMovement();
                jm.joint = int.Parse(n.SelectSingleNode("id").InnerText);
                jm.percent = float.Parse(n.SelectSingleNode("percent").InnerText);
                jm.initialPosition = Exporter.parseVector4Node(n.SelectSingleNode("initialPosition"));
                jm.initialAbsolutePosition = Exporter.parseVector4Node(n.SelectSingleNode("initialAbsolutePosition"));
                jm.finalPosition = Exporter.parseVector4Node(n.SelectSingleNode("finalPosition"));
                jm.finalAbsolutePosition = Exporter.parseVector4Node(n.SelectSingleNode("finalAbsolutePosition"));
                jmList.Add(jm);
            }

            if (jmList.Count > 0)
            {
                GestureAnalyzer.MovementCheckpoint moveCheckPoint = new GestureAnalyzer.MovementCheckpoint();
                moveCheckPoint.duration = duration;
                moveCheckPoint.jointMovements = jmList;
                lMoveCheckpoint.Add(moveCheckPoint);
            }
        }

        GestureAnalyzer.Movement m = new GestureAnalyzer.Movement();
        m.duration = float.Parse(doc.DocumentElement.SelectSingleNode("/dyngesture/mouvement/duration").InnerText);
        m.checkpoints = lMoveCheckpoint;
        ges.Movement = m;
                
        return ges;
    }

    public static Vector4 parseVector4Node(XmlNode node)
    {
        
        Vector4 vec = new Vector4();
        vec.w = float.Parse(node.SelectSingleNode("w").InnerText);
        vec.x = float.Parse(node.SelectSingleNode("x").InnerText);
        vec.y = float.Parse(node.SelectSingleNode("y").InnerText);
        vec.z = float.Parse(node.SelectSingleNode("z").InnerText);
        return vec;
    }
}
