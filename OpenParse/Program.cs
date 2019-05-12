using System;
using System.Collections.Generic;
using System.IO;
using OpenParse.Common;
using OpenParse.Parsers;
using OpenParse.Records;

namespace OpenParse
{
    class Program
    {
        static void Main(string[] args)
        {
            // Read test.txt - SampleRecord
            ReadExample();

            // Write test2.txt - List<SampleRecord>
            WriteExample();

            // Write test3.txt - Queue<Row>
            WriteExample2();

            // Keep console open - Press ENTER to exit
            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }

        static void ReadExample()
        {
            var parser = new FixedWidthParser<SampleRecord>();
            parser.ReadFile("test.txt");

            foreach (var record in parser)
            {
                Console.WriteLine($"First Name: {record.FirstName, -12} Last Name: {record.LastName, -12} Email: {record.Email}");
            }
        }

        static void WriteExample()
        {
            var parser = new FixedWidthParser<SampleRecord>();
            var fs = new FileInfo("test2.txt").OpenWrite();

            var records = new List<SampleRecord>();

            var sample1 = new SampleRecord()
            {
                FirstName = "Daniel",
                LastName = "Davis",
                Email = "daniel@example.com"
            };

            var sample2 = new SampleRecord()
            {
                FirstName = "Some",
                LastName = "Guy"
            };

            records.Add(sample1);
            records.Add(sample2);

            parser.Write(fs, records);

            fs.Close();
        }

        // This needs to be setup differently later. We should just add the fields and assign the values later.
        static void WriteExample2()
        {
            var parser = new FixedWidthParser<GenericRecord>();
            var fs = new FileInfo("test3.txt").OpenWrite();

            Queue<Row> rows = new Queue<Row>();
            Row row = new Row()
            {
                Fields = new List<Field>()
                {        
                    new Field()
                    {
                        Name = "FirstName",
                        Value = "Some",
                        Options = new FieldOptions()
                        {
                            FieldWidth = 20
                        }
                    },

                    new Field()
                    {
                        Name = "Last Name",
                        Value = "Guy",
                        Options = new FieldOptions()
                        {
                            FieldWidth = 20
                        }
                    }
                }
            };

            Row row2 = new Row()
            {
                Fields = new List<Field>()
                {
                    new Field()
                    {
                        Name = "FirstName",
                        Value = "Another",
                        Options = new FieldOptions()
                        {
                            FieldWidth = 20
                        }
                    },

                    new Field()
                    {
                        Name = "Last Name",
                        Value = "Guy",
                        Options = new FieldOptions()
                        {
                            FieldWidth = 20
                        }
                    }
                }
            };

            rows.Enqueue(row);
            rows.Enqueue(row2);
            parser.Write(fs, rows);

            fs.Close();
        }
    }
}
