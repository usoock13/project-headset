namespace Utility {
    using System.Collections.Generic;
    static class List {
        static public void Shuffle<T>(this List<T> list) {
            int c = list.Count;
            var rand = new System.Random();
            for(int i=c; i>0; i--) {
                int ri = rand.Next(0, i);
                list.Add(list[ri]);
                list.RemoveAt(ri);
            }
        }
        static public string ItemList<T>(this List<T> list)  {
            string str = "";
            for(int i=0; i<list.Count; i++) {
                str += list[i].ToString() + ", ";
            }
            return str;
        }
    }
}