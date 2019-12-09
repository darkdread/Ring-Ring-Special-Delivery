using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("QuestCollection")]
public class QuestDataContainer
{
    [XmlArray("Quests")]
    [XmlArrayItem("QuestData")]
    public List<QuestData> QuestsData = new List<QuestData>();

    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(QuestDataContainer));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

    public static QuestDataContainer Load(string path)
    {
        var serializer = new XmlSerializer(typeof(QuestDataContainer));
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return serializer.Deserialize(stream) as QuestDataContainer;
        }
    }

    //Loads the xml directly from the given string. Useful in combination with www.text.
    public static QuestDataContainer LoadFromText(string text)
    {
        var serializer = new XmlSerializer(typeof(QuestDataContainer));
        return serializer.Deserialize(new StringReader(text)) as QuestDataContainer;
    }
}