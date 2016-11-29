using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Collections;

namespace TestTaskAtrinity
{
    class Person : IComparable<Person>
    {
        string surname;
        int heightcm;
        DateTime birthdate;

        public string Surname
        {
            get
            {
                return surname;
            }
        }
        public int Heightcm
        {
            get
            {
                return heightcm;
            }
        }
        public DateTime Birthdate
        {
            get
            {
                return birthdate;
            }
        }

        public Person(string surname)
        {
            this.surname = surname;
        }
        public Person(string surname, int heightcm, int year, int month, int day) : this(surname)
        {
            this.heightcm = heightcm;
            birthdate = new DateTime(year, month, day);
        }
        public override string ToString()
        {
            return "Surname: " + surname + ", height, in cm: " + heightcm + " birthdate: " + birthdate.ToLongDateString();
        }
        public int CompareTo(Person other)
        {
            return this.surname.CompareTo(other.surname);
        }
    }

    class Node<t> : IComparable<Node<t>> // реализация узла двунаправленного связного списка
 where t : class, IComparable<t>                 // из-за метода Remove пришлось сделать его публичным.
    {
        public t item;
        public Node<t> next;
        public Node<t> prev;

        public Node(t init) { item = init; }
        public int CompareTo(Node<t> other)
        {
            return (this.item.CompareTo(other.item));
        }
        public static Node<t> operator ++(Node<t> x) { x = x.next; return x; } // инкремент = следующий узел в списке.
        public static Node<t> operator --(Node<t> x) { x = x.prev; return x; }
    }

    class NodeList<p> : IEnumerable<Node<p>>
        where p : class, IComparable<p>
    {
        private Node<p> first;
        public bool reverseiterator = false; // влияет на направление итерации по списку

        private Node<p> Last
        {
            get
            {
                if (first == null) return null;
                else
                {
                    Node<p> current = first;
                    while (current.next != null) current++;
                    return current;
                }
            }
        } // свойство - последний элемент

        private Node<p> nodeSeek(int ind)
        {
            Node<p> current = first;
            for (int i = 0; i < ind; i++)
            {
                if (current.next == null || current == null) return null;
                current++;
            }
            return current;
        }
        private void swapWithPrev(Node<p> y)
        {
            if (y == null || y.prev == null) return;
            Node<p> x = y.prev;
            /* X - это предшествующий Y элемент.
             Следующие два условных оператора определяют преобразования связей элементов, соседствующих с парой XY.
             */
            if (x != first) { x.prev.next = y; y.prev = x.prev; }
            else { first = y; y.prev = null; }

            if (y.next != null) { y.next.prev = x; x.next = y.next; }
            else x.next = null;

            y.next = x;
            x.prev = y;
        }
        public p seek(int ind)
        {
            if (nodeSeek(ind) == null) return default(p);
            else return nodeSeek(ind).item;
        }
        public void addValue(p input)  // добавление элемента в конец списка
        {
            Node<p> entry = new Node<p>(input);
            if (first == null) first = entry;
            else
            {
                entry.prev = Last;
                Last.next = entry;
            }
        }
        public void addValue(p input, int ind) // добавление элемента с индексом ind со сдвигом остатка списка вправо
        {
            Node<p> entry = new Node<p>(input);
            if (first == null) first = entry; // в случае пустого списка
            else if (ind == 0) // вставка в начало списка
            {
                first.prev = entry;
                entry.next = first;
                first = entry;
            }
            else if (nodeSeek(ind) == null) // если замещаемого эл-та с указанным индексом не существует, 
            {                               // то элемент добавляется в конец.
                entry.prev = Last;
                Last.next = entry;
            }
            else    // самый сложный случай: добавляемый элемент - не концевой
            {
                Node<p> current = nodeSeek(ind);
                entry.prev = current.prev;
                entry.prev.next = entry;
                current.prev = entry;
                entry.next = current;
            }
        }
        public void remove(Node<p> current)
        {
            if (current == null) return; // элемент с несуществующим индексом => ф-ция ничего не делает
            if (current == first) // удаляется элемент из начала списка (2 варианта: единственный и неединственный элемент)
            {
                if (first.next == null) { first = null; return; }
                else
                {
                    first++;
                    first.prev = null;
                    return;
                }
            }
            if (current.next == null) { current.prev.next = null; return; } // удаляется элемент из конца списка
            else                                            // удаляется элемент, который не в начале и не в конце списка
            {
                current.prev.next = current.next;
                current.next.prev = current.prev;
            }
        }
        public void removeAt(int f)            // удаление элемента с индексом f
        {
            Node<p> current = nodeSeek(f);
            remove(current);
        }
        public void removeAll()
        {
            first = null;
        }
        public IEnumerator<Node<p>> GetEnumerator()
        {
            Node<p> current;
            if (reverseiterator == false)
            {
                current = first;
                while (current != null) { yield return current; current = current.next; }
            }
            else
            {
                current = Last;
                while (current != null) { yield return current; current = current.prev; }
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public void sort() // сортировка вставками
                           /*
                           Во втором цикле присваивание j=j означает, что переход к предыдущему элементу в порядке списка
                           осуществляется процедурой swapWithPrev, которая меняет местами два узла, но не изменяет подаваемой
                           на вход ссылки. Индекс элемента с адресом j становится меньше на единицу. 
                           Похожая причина и у присваивания i=i.next в теле цикла. 
                           */
        {
            for (Node<p> i = first.next; i != null; i++)
                for (Node<p> j = i; j.prev != null && j.prev.CompareTo(j) > 0; j = j)
                {
                    swapWithPrev(j);
                    if (i == j) i++;
                }
        }

    }
}
