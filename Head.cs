using System;
using System.IO;
using System.Xml;

namespace TestTaskAtrinity
{
    class Head
    {
        public static void Main(string[] args)
        {
            FileStream S = new FileStream("Menu.txt", FileMode.Open); // текст меню из-за его длины я решил поместить в файл.
            StreamReader s = new StreamReader(S);
            Menu menu = new Menu(new NodeList<Person>(), s.ReadToEnd());
            menu.viewMenu();
            s.Close();
        }        
    }

    class Menu
    {
        public Menu(NodeList<Person> L, string text)
        {
            this.text = text;
            this.L = L;
        }
        string text;
        NodeList<Person> L;

        public void viewMenu()
        {
            Console.WriteLine(text);
            retry: string x = Console.ReadLine();
            switch (x)
            {
                case ("1"): viewPeople(false); break;
                case ("2"): viewPeople(true); break;
                case ("3"): addPerson(); break;
                case ("4"): deletePerson(); break;
                case ("5"): deletePeopleBySurname(); break;
                case ("6"): sortList(); break;
                case ("7"): saveToTxt(); break;
                case ("8"): loadFromTxt(); break;
                case ("9"): saveToXml(); break;
                case ("0"): loadFromXml(); break;
                case ("*"): return;
                default: Console.WriteLine("Wrong entry. Please, try again."); goto retry;
            }
            Console.WriteLine("Done! Press any key...");
            Console.ReadKey();
            viewMenu();
        }

        private void viewPeople(bool reverse)
        {
            L.reverseiterator = reverse;
            foreach (Node<Person> x in L) Console.WriteLine(x.item);
        }
        private void addPerson()
        {
            try
            {
                Console.WriteLine("Enter newcomer's future index number in the list (must be above zero):");
                int i = Convert.ToInt32(Console.ReadLine());
                if (i < 0) throw new InvalidDataException();
                Console.WriteLine("Enter newcomer's surname:");
                string surname = Console.ReadLine();
                Console.WriteLine("Enter his height in cm, integer(!):");
                int height = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Enter his year of birth:");
                int year = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Enter the number of month he was born in:");
                int month = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Now, the day of birth:");
                int day = Convert.ToInt32(Console.ReadLine());
                Person newbie = new Person(surname, height, year, month, day);
                L.addValue(newbie);
            }
            catch
            {
                Console.WriteLine("Sorry, invalid data. Returning to the main menu...\n");
                viewMenu();
            }
        }
        private void deletePerson()
        {
            Console.WriteLine("Enter index of an element to be deleted (must be above zero):");
            int i = Convert.ToInt32(Console.ReadLine());
            if (i < 0) throw new InvalidDataException();
            L.removeAt(i);
        }
        private void deletePeopleBySurname()
        {
            Console.WriteLine("People with which surname should be removed from the list?");
            string s = Console.ReadLine();
            foreach (Node<Person> p in L) { if (p.item.Surname == s) L.remove(p); }
        }
        private void sortList()
        {
            L.sort();
        }
        private void saveToXml()
        {
            XmlDocument document = new XmlDocument();
            document.AppendChild(document.CreateElement("people"));
            XmlElement root = document.DocumentElement;
            XmlElement person = document.CreateElement("person");
            XmlElement surname = document.CreateElement("surname");
            XmlElement height = document.CreateElement("height");
            XmlElement birthdate = document.CreateElement("birthdate");
            birthdate.AppendChild(document.CreateElement("year"));
            birthdate.AppendChild(document.CreateElement("month"));
            birthdate.AppendChild(document.CreateElement("day"));
            person.AppendChild(surname); person.AppendChild(height); person.AppendChild(birthdate);
            L.reverseiterator = false;
            foreach (Node<Person> p in L)
            {
                XmlElement x = (XmlElement)person.CloneNode(true);
                x.SelectSingleNode("//surname").InnerText = p.item.Surname;
                x.SelectSingleNode("//height").InnerText = p.item.Heightcm.ToString();
                x.SelectSingleNode("//year").InnerText = p.item.Birthdate.Year.ToString();
                x.SelectSingleNode("//month").InnerText = p.item.Birthdate.Month.ToString();
                x.SelectSingleNode("//day").InnerText = p.item.Birthdate.Day.ToString();
                root.AppendChild(x);
            }
            document.Save("List.xml");
        }
        private void saveToTxt()
        {
            FileStream F = new FileStream("List.txt", FileMode.Create);
            StreamWriter stream = new StreamWriter(F);
            foreach (Node<Person> node in L)
            {
                stream.WriteLine(node.item.Surname + ',' + node.item.Heightcm + ',' + node.item.Birthdate.Year + ',' +
                    node.item.Birthdate.Month + ',' + node.item.Birthdate.Day);
            }
            stream.Close(); F.Close();
        }
        private void loadFromTxt()
        {
            try
            {
                L.removeAll();
                FileStream F = new FileStream("List.txt", FileMode.OpenOrCreate);
                StreamReader stream = new StreamReader(F);
                string line = stream.ReadLine();
                string[] a;
                while (line != null)
                {
                    a = line.Split(',');
                    L.addValue(new Person(a[0], Convert.ToInt32(a[1]),
                        Convert.ToInt32(a[2]), Convert.ToInt32(a[3]), Convert.ToInt32(a[4])));
                    line = stream.ReadLine();
                }
                stream.Close(); F.Close();
            }
            catch
            {
                Console.WriteLine("Invalid or nonexisting file. Returning to the main menu...\n");
                viewMenu();
            }
        }
        private void loadFromXml()
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load("List.xml");
                L.removeAll();
                XmlNode root = document.DocumentElement;
                XmlNodeList people = root.SelectNodes("*");
                foreach (XmlElement person in people)
                {
                    string surname = person.SelectSingleNode("//surname").InnerText;
                    int height = Convert.ToInt32(person.SelectSingleNode("//height").InnerText);
                    int year = Convert.ToInt32(person.SelectSingleNode("//year").InnerText);
                    int month = Convert.ToInt32(person.SelectSingleNode("//month").InnerText);
                    int day = Convert.ToInt32(person.SelectSingleNode("//day").InnerText);
                    L.addValue(new Person(surname, height, year, month, day));
                }
            }
            catch
            {
                Console.WriteLine("Invalid or nonexisting file. Returning to the main menu...\n");
                viewMenu();
            }

        }
    }
}
