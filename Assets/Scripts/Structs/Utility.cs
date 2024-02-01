namespace Utility {
    using System.Collections.Generic;
    using UnityEngine;

    static class List {
        static public void Shuffle<T>(this List<T> list) {
            if(list.Count == 0)
                return;
            int c = list.Count;
            var rand = new System.Random();
            for(int i=c; i>0; i--) {
                int ri = rand.Next(0, i);
                list.Add(list[ri]);
                list.RemoveAt(ri);
            }
        }
        static public string ItemsToString<T>(this List<T> list)  {
            string str = "";
            for(int i=0; i<list.Count; i++) {
                str += list[i].ToString() + ", ";
            }
            return str;
        }
    }

    static class Utility {
        static public void LookAtWithUp(this Transform transform, Vector2 worldPosition) {
            // Vector2 dir = new Vector2(worldPosition.x - transform.position.x, worldPosition.y - transform.position.y);
            // float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            // transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + -90f));
            transform.up = worldPosition - new Vector2(transform.position.x, transform.position.y);
        }
    }

    public interface IMultiLanguage {
        public IMultiLanguage EN();
        public IMultiLanguage KO();
    }
}