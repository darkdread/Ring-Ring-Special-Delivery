using System.Xml;
using System.Xml.Serialization;

public class QuestData
{
    [XmlAttribute("name")]
    public string Name;

    [XmlArray("Dialogues")]
    [XmlArrayItem("Dialogue")]
    public string[] Dialogue;

    public int AwardBronze, AwardSilver, AwardGold;
}