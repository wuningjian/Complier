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
     * foreach 和 for 的优劣对比 https://www.cnblogs.com/GreenLeaves/p/7401605.html
     * foreach：
     * 优：
     * 1.语句简洁
     * 2.效率比for要高(C#是强类型检查,for循环对于数组访问的时候,要对索引的有效值进行检查)
     * 3.处理多维数组(不包括锯齿数组)更加的方便
     * 4.在类型转换方面foreach不用显示地进行类型转换
     * 5.当集合元素如List<T>等在使用foreach进行循环时,每循环完一个元素,就会释放对应的资源
     * 劣：
     * 1.上面说了foreach循环的时候会释放使用完的资源,所以会造成额外的gc开销,所以使用的时候
     * 2.foreach也称为只读循环,所以再循环数组/集合的时候,无法对数组/集合进行修改
     * 3.数组中的每一项必须与其他的项类型相等
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
