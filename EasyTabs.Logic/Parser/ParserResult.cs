using System;
using System.Collections.Generic;
using System.Text;

namespace EasyTabs.Logic.Parser
{
    public class ParserResult
    {
        public ParserResult()
        {
            Parts = new List<(string label, List<Moment> moments)>();
            TuningDict = GetEStandard();
        }


        public List<(string label, List<Moment> moments)> Parts { get; set; }


        public Dictionary<string,int> TuningDict { get; set; }

        


        //ToDO remove to general tuning lib class ect.
        private static Dictionary<string,int> GetEStandard()
        {
            var dict = new Dictionary<string, int>();

            dict.Add("E", 6);
            dict.Add("A",5);
            dict.Add("D", 4);
            dict.Add("G", 3);
            dict.Add("B", 2);
            dict.Add("e", 1);


            return dict;
            
        }

    }
}
