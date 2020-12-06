using EasyTabs.Logic.ParserCs;
using System.Linq;
using System.Collections.Generic;
using System;
using System.IO;
using EasyTabs.Parser;

namespace EasyTabs
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = null;

            if (args.Length >= 1)
            {
                
                if (!args[0].EndsWith(".et"))
                {
                    Console.WriteLine($"Argument {args[0]} does not have .et ending as expected exiting ");
                    return;
                }

                fileName = args[0];
            }

            if (args.Contains("-fs"))
            {
                var content = TestDataV1;
                if(fileName != null)
                {
                    content = LoadFromFile(fileName);
                }
                FsParser(content);
                return;
            }

            var parser = new ParserCs();

            if (args.Length == 1)
            {

                ParseFromFile(fileName);

                return;
            }

            var parseRes = parser.Parse(TestDataV1);

            var outString = WriteResult(parseRes);


            Console.WriteLine(outString);

        }

        static void FsParser(string content)
        {
            var res = ParserFs.ParseEasyTabs(content);
            ParserFs.TestParse(content);
        }


        static void ParseFromFile(string fileName)
        {
            var content = LoadFromFile(fileName);
            var parser = new ParserCs();

            var parseRes = parser.Parse(content);

            var outFile = fileName.Replace(".et", ".txt");

            var textOut = WriteResult(parseRes);
            Console.Write(textOut);
            File.WriteAllText(outFile, textOut);


        }

        static string LoadFromFile(string fileName)
        {
            var content = File.ReadAllText(fileName);

            return content;
            
         
        }

        static string WriteResult(ParserResult result)
        {
            var outText = "";
            foreach( var part in result.Parts)
            {
                var length = part.moments.Count * 3;               

                outText += WriteMoments(part.label, part.moments, result.TuningDict);

                outText += "\n\n";
                    
            }

            return outText;

        }

        static string WriteMoments(string label, List<Moment> moments, Dictionary<string, int> tuningDict)
        {

            var outputs = new Dictionary<string, string>();

            foreach (var key in tuningDict.Keys)
            {
                outputs.Add(key, "");
            }

            var outputKeys = outputs.Keys.Select(x => x.ToString()).ToList();

            foreach (var moment in moments)
            {
                foreach (var key in outputKeys)
                {
                    var note = moment.Notes.FirstOrDefault(x => x.StringLine == key);

                    var output = "---";
                    if (note != null)
                    {
                        if (note.Fret < 10)
                        {
                            output = $"{note.Fret}--";
                        }
                        else
                        {
                            output = $"{note.Fret}-";
                        }

                    }


                    outputs[key] += output;
                }

            }
            
            var lineLength = 80;
            var lineOffSet = 0;
            var order = tuningDict.OrderBy(x => x.Value).ToList();

            var outResult = "";
            var totalLength = moments.Count * 3;
            while (lineOffSet < totalLength)
            {
                foreach (var kv in order)
                {
                    outResult += $"{kv.Key} {outputs[kv.Key].Substring(lineOffSet , Math.Min(lineLength, totalLength - lineOffSet))}\n";
                }
                lineOffSet += lineLength;
                outResult += "\n";
            }

            return outResult;

        }

        static string TestDataV1 =
            @"v1
D 0 G 2 B 3 E 1 B 3 GA 2 2

ADG 8 10 10 7 9 9 E 0 0 0 0 0 0 - - 0 0 0 0 0 0 - - 0 0 0 0 0 0 0 0";

        static string TestDataV2 =
            @"v2


[Verse 1]
EA 0 3 0 2 E 3 2 1 0 3 2 1 0 (3)

EA 0 3 0 2 E 3 3 6 6 6 6 3 3

ADG 8 10 10 7 9 9 E 0 0 0 0 0 0 0 0 0 0 0 0  0 0 0 0 0 0 0 0

ADG 8 10 10 7 9 9 E 0 0 0 - 0 0 0 - 0 0 0 - 0 0 0 - 0 0 0 - 0 0 0


EA 2 3 2 3 4 3 (2)

[Chorus]
EA 0 2 D 9 9 7 9 10 9 10 12 10 9 10 9 6 (2)

D 12 7 12 7 12 7 12 7 12 7 12 7 12 7

D 10 7 10 7 10 7 10 7 10 7 10 7 10 7";
    }
}
