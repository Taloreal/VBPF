using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace KMI.VBPF1Lib
{
    public delegate TResult func<p1, TResult>(p1 param);

    [Serializable]
    public class StrTyp_Key
    {
        public string name;

        [XmlIgnore] private string _TypeName;
        [XmlIgnore] private Type _type;

        [XmlIgnore] public Type type {
            get { return _type; }
            set {
                _type = value;
                _TypeName = value.AssemblyQualifiedName;
            }
        }

        public string TypeName {
            get { return _TypeName; }
            set {
                _type = Type.GetType(value);
                _TypeName = value;
            }
        }

        public StrTyp_Key(string STKCode) {
            string[] parts = new string[] {
                STKCode.Substring(0, STKCode.IndexOf(" :1a3b5c7d9: ")),
                STKCode.Substring(STKCode.IndexOf(" :1a3b5c7d9: ") + 12)
            };
            name = parts[0];
            type = Type.GetType(parts[1]);
        }

        public StrTyp_Key(string called, Type ofKind)
        {
            name = called;
            type = ofKind;
        }

        public static string Get_STKCode(string called, Type ofKind)
        {
            return called + " :1a3b5c7d9: " + ofKind.AssemblyQualifiedName;
        }
    }

    [Serializable]
    public class Settings
    {
        private static bool Busy = false;
        public static bool Close = false;
        private static bool isChanged = false;
        public static SerializableDictionary<string, string> settings =
            new SerializableDictionary<string, string>();
        public static Dictionary<Type, func<string, object>> Conversions =
            new Dictionary<Type, func<string, object>>();

        public static void AutoSave()
        {
            DateTime Alpha = DateTime.Now;
            while (true)
            {
                while (DateTime.Now < Alpha.AddSeconds(5)) { }
                while (Busy) { }
                if (isChanged) { SaveSettings(); isChanged = false; }
                if (Close) { return; }
                Alpha = DateTime.Now;
            }
        }

        public static void SetUp(bool LoadValues)
        {
            Busy = true;
            Conversions.Add(typeof(string), (x) => x);
            Conversions.Add(typeof(int), (x) => Convert.ToInt32(x));
            Conversions.Add(typeof(float), (x) => Convert.ToSingle(x));
            Conversions.Add(typeof(bool), (x) => Convert.ToBoolean(x));
            Conversions.Add(typeof(double), (x) => Convert.ToDouble(x));
            Conversions.Add(typeof(DateTime), (x) => Convert.ToDateTime(x));
            if (LoadValues)
                LoadSettings();
            new Thread(AutoSave).Start();
            Busy = false;
        }

        public static void SetValue<t1>(string keyname, t1 value)
        {
            if (keyname == null || !Conversions.ContainsKey(typeof(t1)))
                return;
            Busy = true;
            RemoveOld(keyname);
            settings.Add(StrTyp_Key.Get_STKCode(keyname, typeof(t1)), value.ToString());
            isChanged = true;
            Busy = false;
        }

        public static void RemoveOld(string Key)
        {
            settings.Remove(StrTyp_Key.Get_STKCode(Key, typeof(string)));
            settings.Remove(StrTyp_Key.Get_STKCode(Key, typeof(int)));
            settings.Remove(StrTyp_Key.Get_STKCode(Key, typeof(float)));
            settings.Remove(StrTyp_Key.Get_STKCode(Key, typeof(bool)));
            settings.Remove(StrTyp_Key.Get_STKCode(Key, typeof(double)));
            settings.Remove(StrTyp_Key.Get_STKCode(Key, typeof(DateTime)));
        }

        public static bool isGoodKey(string keyname, Type type, out string KEY)
        {
            KEY = StrTyp_Key.Get_STKCode(keyname, type);
            return Conversions.ContainsKey(type)
                && settings.ContainsKey(KEY);
        }

        public static bool GetValue<t1>(string keyname, out t1 Result)
        {
            Busy = true;
            string KEY;
            Result = default(t1);
            if (!isGoodKey(keyname, typeof(t1), out KEY)) { Busy = false; return false; }
            try
            {
                Result = (t1)Conversions[new StrTyp_Key(KEY).type](settings[KEY]);
                Busy = false;
                return true;
            }
            catch { Busy = false; return false; }
        }

        public static bool SaveSettings()
        {
            try
            {
                TextWriter writer = new StreamWriter(File.Open(@"temp.vbpf", FileMode.OpenOrCreate));
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary<string, string>));
                    serializer.Serialize(writer, settings);
                    writer.Close();
                    File.Delete("Settings.vbpf");
                    File.Move("temp.vbpf", "Settings.vbpf");
                    return true;
                }
                catch (Exception E) { writer.Close(); return false; }
            }
            catch (Exception E) { return false; }
        }

        public static bool LoadSettings()
        {
            Busy = true;
            try
            {
                TextReader reader = new StreamReader(File.Open(@"Settings.vbpf", FileMode.Open));
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary<string, string>));
                    settings = (SerializableDictionary<string, string>)serializer.Deserialize(reader);
                    reader.Close();
                    Busy = false;
                    return true;
                }
                catch (Exception E) { reader.Close(); Busy = false; return false; }
            }
            catch (Exception E) { Busy = false; return false; }
        }
    }

    [XmlRoot("dictionary")]
    public class SerializableDictionary<TKey, TValue>
    : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region IXmlSerializable Members
        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");

                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();

                this.Add(key, value);

                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");

                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();

                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }
        #endregion
    }
}
