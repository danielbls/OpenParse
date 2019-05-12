using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using OpenParse.Common;
using OpenParse.Records;

namespace OpenParse.Parsers
{
    class FixedWidthParser<T> where T : new()
    {
        private Queue<T> FieldList { get; set; }

        public FixedWidthParser()
        {
            FieldList = new Queue<T>();
        }

        public void ReadFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                int position = 0;
                T t = new T();
                string value = string.Empty;

                foreach (PropertyInfo prop in t.GetType().GetProperties())
                {
                    FieldOptions options = new FieldOptions();

                    var attributes = prop.GetCustomAttributes();

                    foreach (var attribute in attributes)
                    {
                        if (attribute is FieldOptions)
                        {
                            var attrib = attribute as FieldOptions;
                            options.FieldWidth = attrib.FieldWidth;
                            options.TrimType = attrib.TrimType;
                        }
                    }

                    // Index within bounds
                    if (line.Length >= position + options.FieldWidth)
                    {
                        value = Trim(options.TrimType, line.Substring(position, options.FieldWidth));
                    }

                    // Index out of bounds, get as much as possible
                    else if (line.Length >= position)
                    {
                        value = Trim(options.TrimType, line.Substring(position, line.Length - position));
                    }
                    else
                    {
                        throw new Exception($"Not enough characters in line. Expected length >= {position}. Line: {line}");
                    }

                    t.GetType().GetProperty(prop.Name).SetValue(t, value);
                    position += options.FieldWidth;
                }

                FieldList.Enqueue(t);
            }
        }

        public string Trim(TrimType trimType, string value)
        {
            switch (trimType)
            {
                case TrimType.Left:
                    value = value.TrimStart();
                    break;
                case TrimType.Right:
                    value = value.TrimEnd();
                    break;
                case TrimType.Both:
                    value = value.Trim();
                    break;
            }

            return value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            while (FieldList.Count > 0)
            {
                yield return FieldList.Dequeue();
            }
        }

        public void Write(Stream stream, Queue<Row> rows)
        {
            while(rows.Count > 0)
            {
                var fields = rows.Dequeue().Fields;

                foreach(Field field in fields)
                {
                    WriteField(stream, field.Value, field.Options);
                }

                WriteField(stream, Environment.NewLine, new FieldOptions() { FieldWidth = 1 });
            }

            stream.Flush();
        }

        public void Write(Stream stream, List<T> list)
        {
            foreach(var t in list)
            {
                foreach(PropertyInfo prop in t.GetType().GetProperties())
                {
                    var attributes = prop.GetCustomAttributes();
                    FieldOptions options = new FieldOptions();

                    foreach (var attribute in attributes)
                    {
                        if (attribute is FieldOptions)
                        {
                            options = attribute as FieldOptions;
                        }
                    }

                    var obj = prop.GetValue(t) ?? string.Empty;
                    WriteField(stream, obj.ToString(), options);
                }

                WriteField(stream, Environment.NewLine, new FieldOptions() { FieldWidth = 1 });
            }

            stream.Flush();
        }

        public void WriteField(Stream stream, string value, FieldOptions options)
        {
            string val = value.PadRight(options.FieldWidth, ' ');
            byte[] buffer = Encoding.Default.GetBytes(val);
            stream.Write(buffer, 0, val.Length);
        }
    }
}