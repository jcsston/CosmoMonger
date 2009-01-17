namespace Tjoc.Web.Validator
{
    using System;
    using System.Collections.ObjectModel;
    using System.Text;
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;

    public class XhtmlValidator
    {
        private Collection<ValidationRecord> _records = new Collection<ValidationRecord>();
        private string _document;

        public XhtmlValidator(string input)
        {
            _document = input;
        }

        public Collection<ValidationRecord> Validate()
        {
            XmlReaderSettings xrs = new XmlReaderSettings();
            xrs.ProhibitDtd = false;
            xrs.ValidationType = ValidationType.DTD;
            xrs.ValidationEventHandler += new ValidationEventHandler(xrs_ValidationEventHandler);
            // TODO - need caching resolver!
            xrs.XmlResolver = new CachingXmlResolver();
            using (StringReader sr = new StringReader(_document))
            using (XmlReader xr = XmlReader.Create(sr, xrs))
            {
                try
                {
                    while (xr.Read());
                }
                catch (XmlException xmlExc)
                {
                    _records.Add(new ValidationRecord(xmlExc));
                }
            }

            return _records;
        }

        private void xrs_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            _records.Add(new ValidationRecord(e));
        }
    }
}
