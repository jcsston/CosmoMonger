namespace CosmoMonger.Models.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    public class LoggingTextWriter : TextWriter
    {
        private LogEntry _entryTemplate = new LogEntry();

        public LoggingTextWriter()
        {
        }

        public LoggingTextWriter(ICollection<string> categories,
          int priority,
          int eventId,
          TraceEventType severity,
          string title,
          IDictionary<string, object> properties)
        {
            _entryTemplate = new LogEntry("EMPTY", categories, priority, eventId,
                severity, title, properties);
        }

        public LoggingTextWriter(string category,
          int priority,
          int eventId,
          TraceEventType severity,
          string title,
          IDictionary<string, object> properties)
        {
            _entryTemplate = new LogEntry("EMPTY", category, priority, eventId,
                severity, title, properties);
        }

        public LoggingTextWriter(LogEntry entryTemplate)
        {
            if (entryTemplate == null)
                throw new ArgumentNullException("entryTemplate");

            _entryTemplate = entryTemplate;
        }

        public override void Write(string value)
        {
            value = Regex.Replace(value, "[ \t\r\n \t]+", " ",
                RegexOptions.Compiled);

            LogEntry entry = new LogEntry();
            entry.Categories = _entryTemplate.Categories;
            entry.Priority = _entryTemplate.Priority;
            entry.EventId = _entryTemplate.EventId;
            entry.Severity = _entryTemplate.Severity;
            entry.Title = _entryTemplate.Title;
            entry.ExtendedProperties = _entryTemplate.ExtendedProperties;

            entry.Message = value;

            Logger.Write(entry);
        }

        public override void WriteLine(string value)
        {
            this.Write(value);
        }

        public override System.Text.Encoding Encoding
        {
            get { return System.Text.Encoding.Unicode; }
        }
    }
}
