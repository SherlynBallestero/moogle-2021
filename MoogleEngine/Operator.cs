using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoogleEngine
{
    public class Operator
    {
        public HashSet<string> MustExistWords = new HashSet<string>();
        public HashSet<string> MustNotExistWords = new HashSet<string>();
        public HashSet<Tuple<string,string>> PairWords = new HashSet<Tuple<string,string>>();
        public Dictionary<string,int> ImportanceWords = new Dictionary<string,int>();

        public static List<Tuple<string,int>> GetWordsAndPositionsFromString(string cad)
        {
            List<Tuple<string,int>> vect = new List<Tuple<string,int>>();

            int last = -1;
            StringBuilder word = new StringBuilder();

            for(int i = 0 ; i < cad.Length ; i++)
            {
                if(Char.IsLetterOrDigit(cad[i]))
                {
                    if(word.Length == 0)last = i;
                    word.Append(cad[i]);
                }
                else
                {
                    if(word.Length > 0)
                    {
                        vect.Add(new Tuple<string,int>(word.ToString().ToLower(), last));
                        last = -1;
                        word.Clear();
                    }
                }
            }

            if(word.Length > 0)
            {
                vect.Add(new Tuple<string,int>(word.ToString().ToLower(), last));
                last = -1;
                word.Clear();
            }

            return vect;
        }

        public bool CompareWords(string s1, string s2)
        {
            for(int i = 0 ; i < Math.Min(s1.Length, s2.Length) ; i++)
            {
                if(s1[i] < s2[i])return true;
                if(s1[i] > s2[i])return false;
            }

            if(s1.Length < s2.Length)return true;

            return false;
        }

        public List<int> GetCharPositions(string cad, char c)
        {
            List<int> Pos = new List<int>();
            for(int i = 0 ; i < cad.Length ; i++)
            {
                if(cad[i] == c)
                {
                    Pos.Add(i);
                }
            }
            return Pos;
        }

        public int FindFistRight(List<Tuple<string,int>> vect, int pos)
        {
            for(int i = 0 ; i < vect.Count ; i++)
            {
                if(pos < vect[i].Item2)
                {
                    return i;
                }
            }
            return -1;
        }

        public int FindFistLeft(List<Tuple<string,int>> vect, int pos)
        {
            for(int i = vect.Count-1 ; i >= 0 ; i--)
            {
                if(pos > vect[i].Item2)
                {
                    return i;
                }
            }
            return -1;
        }

        public Operator(string cad)
        {
            List<Tuple<string,int>> vect = GetWordsAndPositionsFromString(cad);

            foreach(int pos in GetCharPositions(cad,'^'))
            {
                int R = FindFistRight(vect, pos);

                if(R != -1)
                {
                    if(!MustExistWords.Contains(vect[R].Item1))
                    {
                        MustExistWords.Add(vect[R].Item1);
                    }
                }
            }

            foreach(int pos in GetCharPositions(cad,'!'))
            {
                int R = FindFistRight(vect, pos);

                if(R != -1)
                {
                    if(!MustNotExistWords.Contains(vect[R].Item1))
                    {
                        MustNotExistWords.Add(vect[R].Item1);
                    }
                }
            }

            foreach(var word in vect)
            {
                if(!ImportanceWords.ContainsKey(word.Item1))
                {
                    ImportanceWords.Add(word.Item1, 0);
                }
            }

            foreach(int pos in GetCharPositions(cad,'*'))
            {
                int R = FindFistRight(vect, pos);

                if(R != -1)
                {
                    ImportanceWords[vect[R].Item1]++;
                }
            }

            foreach(int pos in GetCharPositions(cad,'~'))
            {
                int L = FindFistLeft(vect, pos);
                int R = FindFistRight(vect, pos);

                if(L != -1 && R != -1)
                {
                    string s1 = vect[L].Item1;
                    string s2 = vect[R].Item1;

                    if(s1 == s2)
                    {
                        continue;
                    }

                    if(CompareWords(s2,s1))
                    {
                        (s1,s2) = (s2,s1);
                    }

                    if(!PairWords.Contains(new Tuple<string, string>(s1,s2)))
                    {
                        PairWords.Add(new Tuple<string, string>(s1,s2));
                    }
                }
            }
        }
    }
}