using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDemo
{
    /*
     * 枚举器IEnumerator foreach 实现例子
     * **/
    class TestIEnumerator
    {
        //static void Main(string[] args)
        //{
        //    Console.WriteLine("Hello TestIEnumerator");

        //    Person[] peopleArray = new Person[3]
        //    {
        //        new Person("a", "aa"),
        //        new Person("b", "bb"),
        //        new Person("c", "cc"),
        //    };

        //    People people = new People(peopleArray);
        //    foreach(Person p in people)  // people类实现了IEnumerable接口，IEnumerator枚举器的Current要实现Person类型的返回，才能使用foreach
        //    {
        //        Console.WriteLine(p.firstName + " - " + p.lastName);
        //    }

        //    Console.ReadKey();
        //}
    }

    public class Person
    {
        public Person(string fName, string lName)
        {
            this.firstName = fName;
            this.lastName = lName;
        }

        public string firstName { get; set;}
        public string lastName { get; set;}
    }

    public class People:IEnumerable
    {
        private Person[] _people;
        public People(Person[] pArray)
        {
            _people = new Person[pArray.Length];
            for(int i=0; i < pArray.Length; i++)
            {
                _people[i] = pArray[i];
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public PeopleEnum GetEnumerator()
        {
            return new PeopleEnum(_people);
        }

    }

    public class PeopleEnum: IEnumerator
    {
        public Person[] _people;
        int position = -1; // 在MoveNext()首次调用之前，枚举器都是位于-1这个位置

        public PeopleEnum(Person[] list)
        {
            _people = list;
        }

        public bool MoveNext()
        {
            position++;
            return (position < _people.Length);
        }

        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current
        {
            get{
                return Current;
            }
        }

        public Person Current
        {
            get
            {
                try
                {
                    return _people[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
