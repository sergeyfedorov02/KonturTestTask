using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;

namespace KonturTestTask.Helpers
{
    internal class XmlHelper
    {
        public static void TransformXml(XmlReader inputXmlReader, XmlWriter outputXmlWriter)
        {
            // создание и настройка XSLT преобразователя
            var xslt = new XslCompiledTransform();

            // загрузка XSLT схемы
            using var xsltReader = ResourceHelper.ReadTransformContent();
            xslt.Load(xsltReader);

            // выполнение преобразования
            xslt.Transform(inputXmlReader, outputXmlWriter);           
        }
    }
}
