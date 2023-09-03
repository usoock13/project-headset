using System.Collections.Generic;

public class Heap<T> where T : System.IComparable<T> {
    private bool desc = false;
    List<T> list;

    public Heap(bool desc=false) {
        this.desc = desc;
        list = new List<T>();
    }

    public void Add(T item) {
        list.Add(item);

        int i = list.Count / 2;

        while(i > 0) {
            int parent = (i-1) / 2;
            if(desc && list[parent].CompareTo(list[i]) > 0
            || !desc && list[parent].CompareTo(list[i]) < 0) {
                Swap(i, parent);
            }
        }
    }
    private void Swap(int i, int j) {
        T t = list[i];
        list[i] = list[j];
        list[j] = t;
    }
}