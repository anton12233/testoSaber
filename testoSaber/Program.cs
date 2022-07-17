using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace testoSaber
{
    internal class Program
    {
        static void Main(string[] args)
        {

            ListRandom listNode = new ListRandom("someOne0");
            listNode.pushElement("someOne1");
            listNode.pushElement("someOne2");
            listNode.pushElement("someOne3");
            listNode.pushElement("someOne4");
            listNode.pushElement("someOne5");

            try
            {
                using (Stream fstream = new FileStream("Serialize.txt", FileMode.Create))
                {
                    listNode.Serialize(fstream);
                }
            }
            catch (IOException e) { Console.WriteLine(e); }

            ListRandom listNote1 = new ListRandom();
            try
            {
                using (Stream fstream = File.OpenRead("Serialize.txt"))
                {
                    listNote1.Deserialize(fstream);
                }
            }
            catch (IOException e) { Console.WriteLine(e); }

            try
            {
                using (Stream fstream = new FileStream("Serialize1.txt", FileMode.Create))
                {
                    listNote1.Serialize(fstream);
                }
            }
            catch (IOException e) { Console.WriteLine(e); }
        }

        class ListNode
        {
            public ListNode Previous;
            public ListNode Next;
            public ListNode Random; // произвольный элемент внутри списка
            public string Data;
        }

        class ListRandom
        {
            public ListNode Head;
            public ListNode Tail;
            public int Count;

            public ListRandom(string someOne)
            {
                ListNode listNode = new ListNode();
                listNode.Data = someOne;
                Tail = listNode;
                Head = listNode;
                Count = 1;
                Tail.Random = giveMeARandom();
            }
            public ListRandom()
            {
            }

            //Затолкнуть элемент в начало списка
            public void pushElement(string someOne)
            {
                if (Tail.Next == null)
                {
                    ListNode tempNode = new ListNode();
                    tempNode.Data = someOne;
                    tempNode.Previous = this.Tail;
                    this.Tail.Next = tempNode;
                    this.Head = tempNode;
                    this.Count++;
                    tempNode.Random = giveMeARandom();
                }
                else
                {
                    if (Tail.Next != null)
                    {
                        ListNode tempNode = new ListNode();
                        tempNode.Data = someOne;
                        this.Head.Next = tempNode;
                        tempNode.Previous = this.Head;
                        this.Head = tempNode;
                        this.Count++;
                        tempNode.Random = giveMeARandom();
                    }
                }
            }

            //Получить случайный элемент из начала списка
            //Когда элементов в списке <3 указывает на хвост
            //Когда элементов >=3 указывает на лучайный не крайний элемент
            private ListNode giveMeARandom()
            {
                if (this.Count <= 2)
                {
                    return this.Tail;
                }
                
                Random rnd = new Random();
                int value = rnd.Next(1, Count-1);
                return WeNeedToGoDeeper(value);
            }

            //Рекурсивно проходит внутрь списка с головы value раз
            private ListNode WeNeedToGoDeeper(int value)
            {
                ListNode Elem = this.Head;
                for (int i = 0; i < value; i++)
                {
                    Elem = ISeeIn(Elem);
                }

                return Elem;
            }

            //Показывает предыдущий элемент указанного узла
            private ListNode ISeeIn(ListNode element)
            {
                return element.Previous;
            }

            //Подготовка словаря для записи в файл
            private Dictionary<ListNode, int> toDict()
            {
                var nodesMyID = new Dictionary<ListNode, int>();
                var nodesRandomID = new Dictionary<ListNode, int>();

                ListNode Elem = this.Head;
                for (int i = 0; i < Count; i++)
                {
                    nodesMyID.Add(Elem, (Count-i));
                    Elem = ISeeIn(Elem);
                }
                foreach(var node in nodesMyID.Keys)
                {
                    nodesRandomID.Add(node, nodesMyID[node.Random]);
                }

                return nodesRandomID;
            }

            //Подготовка листа для извлечения из файла
            public List<KeyValuePair<string,int>> listFromFile(Stream s)
            {
                List<KeyValuePair<string, int>> keyValuePairs = new List<KeyValuePair<string, int>>();

                byte[] abc = new byte[s.Length];

                s.Read(abc, 0, (int)s.Length);
                string tempo = Encoding.Default.GetString(abc);

                string[] strings = tempo.Split('\n');

                foreach (var str in strings) 
                {
                    string[] tempoSplit = str.Split(" ");
                    if (tempoSplit[0] != "")
                    {
                        keyValuePairs.Add(new KeyValuePair<string, int>(tempoSplit[0], Int32.Parse(tempoSplit[1])));
                    }
                }


                keyValuePairs.Reverse();

                return keyValuePairs;
            }

            //Запись в файл
            public void Serialize(Stream s)
            {
                var dict = toDict();
                foreach (var node in dict)
                {
                    s.Write(Encoding.UTF8.GetBytes(node.Key.Data),  0, node.Key.Data.Length);
                    s.Write(Encoding.UTF8.GetBytes(" " + node.Value.ToString() + "\n"),  0, node.Value.ToString().Length + 2);
                }
            }

            //Извлечения из файла
            public void Deserialize(Stream s)
            {

                ListNode listNode = new ListNode();

                var list = listFromFile(s);
                listNode.Data = list.First().Key;

                Tail = listNode;
                Head = listNode;
                Count = 1;
                Tail.Random = giveMeARandom();

                list.Remove(list.First());

                foreach (var pair in list)
                {
                    this.pushElement(pair.Key);
                    this.Head.Random = WeNeedToGoDeeper(this.Count - pair.Value);
                }


            }

        }
    }
}
