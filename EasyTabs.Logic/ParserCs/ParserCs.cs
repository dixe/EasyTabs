using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyTabs.Logic.ParserCs
{
    public class ParserCs
    {

        public ParserResult Parse(string input)
        {
            var lines = input.Split('\n').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();

            var version = lines[0];

            switch (version.ToLower())
            {
                case "v1": return ParseV1(lines.Skip(1));                                   
                case "v2": return ParseV2(lines.Skip(1));
                default:
                    {
                        Console.WriteLine("No version found returning empy parse");
                        break;
                    }
            }
            return new ParserResult();

        }
        private ParserResult ParseV2(IEnumerable<string> lines)
        {
            var currentMoments = new List<Moment>();

            var label = "";
            var parserResult = new ParserResult();
            foreach (var line in lines)
            {
                if (line.StartsWith("["))
                {
                    if (!string.IsNullOrEmpty(label) || currentMoments.Any())
                    {
                        parserResult.Parts.Add((label, currentMoments));
                        label = "";
                        currentMoments = new List<Moment>();
                    }

                    label = line.Replace("[", "").Replace("]", "").Trim();

                    continue;
                }

                currentMoments.AddRange(ParseLine(line));
            }

            if (!string.IsNullOrEmpty(label) || currentMoments.Any())
            {
                parserResult.Parts.Add((label, currentMoments));
            }

            return parserResult;
        }



        private ParserResult ParseV1(IEnumerable<string> lines)
        {
            var currentMoments = new List<Moment>();

            var label = "";
            var parserResult = new ParserResult();
            foreach (var line in lines)
            {
                if (line.StartsWith("["))
                {
                    if(!string.IsNullOrEmpty(label) || currentMoments.Any())
                    {
                        parserResult.Parts.Add((label, currentMoments));
                        label = "";
                        currentMoments = new List<Moment>();
                    }

                    label = line.Replace("[", "").Replace("]", "").Trim();

                    continue;
                }

                currentMoments.AddRange(ParseLine(line));
            }

            if (!string.IsNullOrEmpty(label) || currentMoments.Any())
            {
                parserResult.Parts.Add((label, currentMoments));               
            }

            return parserResult;
        }



        private List<Moment> ParseLine(string line)
        {
            var tokens = line.Split().Where(x => !string.IsNullOrEmpty(x)).ToArray() ;
            
            var moments = new List<Moment>();

            while (tokens.Any())
            {

                List<string> stringLines;
                (tokens, stringLines) = ParseStringLines(tokens);


                List<Moment> lineMoments;
                (tokens, lineMoments) = ParseFrets(stringLines, tokens);

                moments.AddRange(lineMoments);
            }

            return moments;
        }

        private (string[] tokens, List<string> stringLines) ParseStringLines(string[] tokens)
        {
            return (tokens.Skip(1).ToArray(), tokens[0].ToList().Select(x=>x.ToString()).ToList());
        }

        private (string[] tokens, List<Moment> moments) ParseFrets(List<string> stringLines, string[] tokens)
        {


            // take stringlines tokens and parse as int, repeat until tokens does not start with an int or a -
            var idx = 0;
            var frets = new List<int?>();
            var success = false;
            while (idx < tokens.Count() && ((success = int.TryParse(tokens[idx], out var fret)) || tokens[idx] == "-"))
            {
                frets.Add(success ? (int?) fret : null);
                idx += 1;
            }

            if (frets.Count % stringLines.Count != 0)
            {
                // Error there was a miss match like 'DA 1 1 2' we are missing a note value for A
            }


            var offSet = 0;
            var result = new List<Moment>();


            while (offSet < frets.Count)
            {
                // take the same number of frets as there are notess
                var noteFrets = frets.Skip(offSet).Take(stringLines.Count).ToList();

                offSet += stringLines.Count;

                result.Add(new Moment(stringLines, noteFrets.Where(x=>x.HasValue).Select(y=>y.Value).ToList()));
            }


            return (tokens.Skip(idx).ToArray(), result);
        }


    }
}
