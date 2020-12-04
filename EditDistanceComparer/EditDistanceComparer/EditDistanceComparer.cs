using System;
using System.Collections.Generic;
using System.Text;

namespace EditDistanceComparerDemo
{
    public class EditDistanceComparer
    {
        private readonly string _pattern;
        private readonly double _mt;
        WordCompareType _compareType;
        public enum WordCompareType { None, Asymmetrical, Symmetrical }

        public EditDistanceComparer(string pattern, double mismtachThreshold, WordCompareType compareType = WordCompareType.Asymmetrical)
        {
            _pattern = pattern;

            if (mismtachThreshold < 0)
            {
                _mt = 0;        // exact word matches only 
            }
            else if (mismtachThreshold > 1)
            {
                _mt = 1;        // 100% mismatch allowed (everything will match)
            }
            else
            {
                _mt = mismtachThreshold;
            }

            _compareType = compareType;
        }

        public static double EditDistance(string s, string t, double limit)
        {
            int x, y, width, height, cost;
            int[] area;
            double distance;

            distance = -1;
            if (s.Length != t.Length)
                limit = 0;

            width = s.Length;
            height = t.Length;

            if ((width > 0) && (height > 0))
            {
                area = new int[(width + 1) * (height + 1)];
                width++; height++;

                // initialize matrix
                for (x = 0; x < width; x++)
                    area[x] = x * 10;
                for (y = 0; y < height; y++)
                    area[y * width] = y * 10;

                // calc distance
                for (x = 1; x < width; x++)
                {
                    for (y = 1; y < height; y++)
                    {
                        if (s[x - 1] == t[y - 1])
                            cost = 0;
                        else if ((((y < height - 1) && (s[x - 1] == t[y])) && ((x < width - 1) && (s[x] == t[y - 1]))) || (((x > 1) && (s[x - 2] == t[y - 1])) && ((y > 1) && (s[x - 1] == t[y - 2]))))
                            cost = 3;
                        else
                            cost = 10;
                        area[(y * width) + x] = Minimum(area[((y - 1) * width) + x] + 10, area[((y * width) + x) - 1] + 10, area[(((y - 1) * width) + x) - 1] + cost);

                        if ((limit > 0) && ((y == x) || ((x > height - 1) && (y == height - 1)) || (y > width - 1) && (x == width - 1)) && ((double)area[(y * width) + x] / 10 > limit))
                        {
                            distance = (double)area[(y * width) + x] / 10;

                            return distance;
                        }
                    }
                }

                distance = (double)area[(width * height) - 1] / 10;
            }

            static int Minimum(int a, int b, int c)
            {
                int min = a;

                if (b < min)
                {
                    min = b;
                }

                if (c < min)
                {
                    min = c;
                }

                return min;
            }

            return distance;
        }

        public bool Matches(string value)
        {
            switch (_compareType)
            {
                case WordCompareType.Asymmetrical:
                    break;

                default: throw new NotSupportedException();
            }

            if (_pattern == null || value == null)
            {
                return false;
            }

            var sourceWs = _pattern.Split(new char[] { ' ', ',', '.' }, StringSplitOptions.RemoveEmptyEntries);
            var targetWs = value.Split(new char[] { ' ', ',', '.' }, StringSplitOptions.RemoveEmptyEntries);
            bool matchEd;

            if (sourceWs.Length <= targetWs.Length)
            {
                matchEd = AsymmentricWordCompare(sourceWs, targetWs);
            }
            else
            {
                matchEd = AsymmentricWordCompare(targetWs, sourceWs);
            }

            return matchEd;
        }

        private bool AsymmentricWordCompare(string[] a, string[] b)
        {
            if (a.Length <= 0)
            {
                return false;
            }

            int hits = 0;
            foreach (var s1 in a)
            {
                foreach (var s2 in b)
                {
                    if (Compare(s1.ToUpper(), s2.ToUpper()))
                    {
                        hits++;
                    }
                }
            }

            return (hits >= a.Length ? true : false);
        }

        private bool Compare(string s1, string s2)
        {
            bool matchEd = false;

            if (_mt > 0)
            {
                var n = s1.Length;
                var m = s2.Length;

                if (n == 0 || m == 0)
                {
                    matchEd = false;
                }

                var max = (n > m) ? n : m;
                var dist = EditDistance(s1, s2, max * _mt);

                if (dist / (double)max <= _mt)
                {
                    matchEd = true;
                }
            }
            else
            {
                matchEd = s1.Equals(s2);
            }

            return matchEd;
        }
    }
}
