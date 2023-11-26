namespace Utility {
    using System.Collections.Generic;
    static class List {
        static public void Shuffle<T>(this List<T> list) {
            int c = list.Count;
            var rand = new System.Random();
            for(int i=c-1; i>=0; i--) {
                int ri = rand.Next(0, i);
                list.Add(list[ri]);
                list.RemoveAt(ri);
            }
        }
    }
}