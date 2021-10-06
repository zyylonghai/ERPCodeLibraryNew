using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Library.Core
{
    public class SerializerUtils
    {
        /// <summary>序列化并保存为xml文档</summary>
        /// <param name="msg"></param>
        /// <param name="filepath"></param>
        public static void xmlserialzaition<T>(T entity, string filepath)
        {
            using (FileStream stream = new FileStream(filepath, FileMode.Create, FileAccess.Write))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, entity);
                //Console.WriteLine("对象已经被序列化。" + msg.ToString());
            }
        }

        /// <summary>
        /// xml反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlstr"></param>
        /// <returns></returns>
        public static T XMLDeSerialize<T>(string xmlstr)
        {
            T rejust = Activator.CreateInstance<T>();
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                StringReader xmlrd = new StringReader(xmlstr);
                rejust = (T)serializer.Deserialize(xmlrd);
            }
            catch (Exception e)
            {

            }
            return rejust;
        }
        /// <summary>
        /// 序列化，转成xml字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string XMLSerialize<T>(T entity)
        {
            StringBuilder buffer = new StringBuilder();

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextWriter writer = new StringWriter(buffer))
            {
                serializer.Serialize(writer, entity);
            }

            return buffer.ToString();

        }

    }
}
