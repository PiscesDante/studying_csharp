# 第 19 章 LINQ



## 19.1 什么是 LINQ

和数据库不同，从对象中获取数据的方法一直都是作为程序设计的一部分来设计的。 **<u>然而使用 LINQ 可以很轻松地查询对象集合</u>** 。

#### LINQ 的重要高级特性

* LINQ （ 发音：Link ）代表语言集成查询（ Language INtegrated Query ）。
* LINQ 是 .NET 框架的扩展，它允许我们使用 SQL 查询数据库的方式来查询数据集合。
* 使用 LINQ 可以从数据库、程序对象的集合以及 XML 文档中查询数据。

```C#
using System;
using System.Linq;
using System.Collections.Generic;

namespace PlayGround
{    
    public class Program
    {
        public static void Main(string[] args)
        {
            int[] numbers = new int[]{ 2, 12, 5, 15 };
            IEnumerable<int> lowNumbers =
                from number in numbers
                where number < 10
                select number;
            foreach (int number in lowNumbers) {
                Console.Write("{0} ", number);
            }
        }
    }
}
```

## 19.2 LINQ 提供程序

对于每一种数据源类型，在其背后肯定有根据该数据源类型事件 LINQ 查询的代码模块。这些代码模块叫做「 LINQ 提供程序 」。

* 微软为常见的数据源类型提供了 LINQ 提供程序；
* 我们可以使用任何支持 LINQ 的语言来查询有 LINQ 提供程序的数据源类型。

本章我们会解释 `LINQ to Object` 和 `LINQ to XML` 。

#### 匿名类型

匿名类型经常使用在 LINQ 查询的结果中。

```C#
using System;

namespace PlayGround
{    
    public class Program
    {
        public static void Main(string[] args)
        {
            /* 声明中的类型必须选择 var */
            var student = new {
                Name = "Inigo Montoya",
                Age = 18,
                Major = "History"
            };
            Console.WriteLine(student.Name); /* Inigo Montoya */
        }
    }
}
```

需要了解的有关匿名类型的重要事项如下：

* 匿名类型只能配合局部变量使用，不能用作类成员；
* 由于没有名字，故只能使用 `var` 关键字来声明；
* 不能设置匿名类型对象的属性，匿名类型的属性是只读的。

## 19.3 方法语法和查询语法

* 方法语法：使用标准的方法调用。这些方法是一组叫做标准查询运算符的方法，本章稍后会介绍；
* 查询语法：看上去和 SQL 语句很相似，使用查询表达式形式书写。

微软推荐使用查询语法，因为更易读。

```C#
using System;
using System.Linq;
using System.Collections.Generic;

namespace PlayGround
{    
    public class Program
    {
        public static void Main(string[] args)
        {
            int[] numbers = new int[]{ 2, 5, 28, 31, 17, 16, 42 };
            var numbersLessThanTwenty = /* 查询语法 */
                from number in numbers
                where number < 20
                select number;
            foreach(int number in numbersLessThanTwenty) {
                Console.Write("{0} ", number);
            }
            Console.WriteLine("");

            var numbersLessThanTen = numbers.Where(x => x < 20); /* 方法语法 */
            foreach(int number in numbersLessThanTen) {
                Console.Write("{0} ", number);
            }
            Console.WriteLine("");

            Console.WriteLine(numbersLessThanTwenty.Count());
        }
    }
}

// 2 5 17 16
// 2 5 17 16
// 4
```

## 19.4 查询变量

LINQ 的查询可以返回两种类型的结果——它可以是一个枚举，它满足查询参数的项列表；也可以是是一个单值，它是满足查询条件的结果的某种摘要形式。

```C#
int[] numbers = new int[] { 2, 5, 8 };

IEnumerable<int> lowNumbers = from element in numbers
    where element < 20
    select element;

int numberCounts = (
	from number in numbers
    where number < 20
    select number
).Count(); /* 返回从查询返回的项目总数 */
```

## 19.5 查询表达式的结构

* 子句必须按照一定的顺序出现。
* `from` 子句和 `select...group` 子句这两个部分是必须的。
* 其他子句是可选的。
* 在 LINQ 查询表达式中， `select` 子句在表达式的最后。这与 SQL 的 `SELECT` 语句在查询的开始不太一样。
* 可由任意多的 `from...let...where` 子句。

### 19.5.1 `from` 子句

```
from Typename Item in Items
```

其中 `Typename` ，也就是元素类型是可选的。

```C#
int arr = new int[] { 10, 11, 12, 13 };

IEnumerable<int> queryResult = 
    from int elem in arr
    where elem < 13 /* 使用迭代变量 elem */
    select elem;    /* 使用迭代变量 elem */
```

* `from` 子句仅仅规定集合中的所有元素都要被访问，但是不要求以什么样的顺序；
* `foreach` 语句在遇到代码就开始执行主体，但是 `from` 子句什么也不执行，它创建可以执行查询的后台代码对象。只有在程序的控制流遇到访问查询变量的语句时，才会开始查询。

### 19.5.2 `join` 子句

有关联结( `join` )的语法如下：

* 使用联结来结合两个或更多集合中的数据；
* 联结操作接受两个集合，然后创建一个临时的对象集合。每一个对象包含原始集合对象中的所有字段。

```C#
IEnumerable<Student> queryResult =
    from eachStudent in allStudents /* 第一个集合 ID */
    join targetedStudent in studentsInCourses /* 第二个集合 ID */
    on eachStudent.ID equals targetedStudent.ID; /* 联结的条件 */
```

### 19.5.3 什么是联结

```C#
using System;
using System.Linq;
using System.Collections.Generic;

namespace PlayGround
{
    public class Student
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public Student(int id, string lastName)
        {
            ID = id;
            LastName = lastName;
        }
    }

    public class StudentInCourses
    {
        public int ID { get; set; }
        public string CourseName { get; set; }

        public StudentInCourses(int id, string courseName)
        {
            ID = id;
            CourseName = courseName;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Student[] allStudents = new Student[] {
                new Student(1, "Carson"),
                new Student(2, "Klassen"),
                new Student(3, "Fleming"),
            };

            StudentInCourses[] studentInCourses = new StudentInCourses[] {
                new StudentInCourses(1, "Art"),
                new StudentInCourses(2, "Art"),
                new StudentInCourses(1, "History"),
                new StudentInCourses(3, "History"),
                new StudentInCourses(3, "Physics"),
            };

            IEnumerable<string> lastNames =
                from Student eachStudent in allStudents
                join student in studentInCourses on eachStudent.ID equals student.ID
                where student.CourseName == "History"
                select eachStudent.LastName;
            foreach (string lastName in lastNames) {
                Console.WriteLine("History Course: {0}", lastName);
            }
        }
    }
}
```

### 19.5.4 查询主体中的 `from...let...where` 片段

* `from` 子句：每个 `from` 子句都指定了一个额外的源数据集合并且引入了要在之后运算的迭代变量，所有的 `from` 子句的语法和含义都是一样的。

    ```C#
    using System;
    using System.Linq;
    using System.Collections.Generic;
    
    namespace PlayGround
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                int[] arr1 = new int[] { 1, 2, 3, 4, 5, 7 };
                int[] arr2 = new int[] { 4, 7, 2, 3, 5, 9 };
                var queryResult = 
                    from num1 in arr1
                    from num2 in arr2
                    where num1 < 6 && num2 > 8
                    select new { num1, num2, sum = num1 + num2 };
                foreach(var item in queryResult) {
                    Console.WriteLine(item);
                }
            }
        }
    }
    
    // { num1 = 1, num2 = 9, sum = 10 }
    // { num1 = 2, num2 = 9, sum = 11 }
    // { num1 = 3, num2 = 9, sum = 12 }
    // { num1 = 4, num2 = 9, sum = 13 }
    // { num1 = 5, num2 = 9, sum = 14 }
    ```

* `let` 子句：接受一个表达式的运算，并且将其赋值给一个需要在其他运算中使用的标识符。

    ```C#
    using System;
    using System.Linq;
    using System.Collections.Generic;
    
    namespace PlayGround
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                int[] arr1 = new int[] { 1, 2, 3, 4, 5, 7 };
                int[] arr2 = new int[] { 4, 7, 2, 3, 5, 9 };
                var queryResult = 
                    from num1 in arr1
                    from num2 in arr2
                    where num1 < 6 && num2 > 4
                    let sum = num1 + num2 /* let 的用法 */
                    where sum == 10
                select new { num1, num2, sum };
                foreach(var item in queryResult) {
                    Console.WriteLine(item);
                }
            }
        }
    }
    
    // { num1 = 1, num2 = 9, sum = 10 }
    // { num1 = 3, num2 = 7, sum = 10 }
    // { num1 = 5, num2 = 5, sum = 10 }
    ```

* `where` 子句：根据之后的运算来过滤不符合指定条件的项 `where BooleanExpression` 。

    ```C#
    using System;
    using System.Linq;
    using System.Collections.Generic;
    
    namespace PlayGround
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                int[] arr1 = new int[] { 1, 2, 3, 4, 5, 7 };
                int[] arr2 = new int[] { 4, 7, 2, 3, 5, 9 };
                var queryResult = 
                    from num1 in arr1
                    from num2 in arr2
                    where num1 < 6
                    where num2 > 5
                    let sum = num1 + num2
                    where sum == 10
                select new { num1, num2, sum };
                foreach(var item in queryResult) {
                    Console.WriteLine(item);
                }
            }
        }
    }
    
    // { num1 = 1, num2 = 9, sum = 10 }
    // { num1 = 3, num2 = 7, sum = 10 }
    ```

### 19.5.5 `orderby` 子句

`orderby` 子句接受一个表达式并且根据表达式按照顺序返回结果项。

```C#
using System;
using System.Linq;
using System.Collections.Generic;

namespace PlayGround
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int[] arr1 = new int[] { 1, 2, 3, 4, 5, 7 };
            int[] arr2 = new int[] { 4, 7, 2, 3, 5, 9 };
            var queryResult = 
                from num1 in arr1
                from num2 in arr2
                where num1 < 8
                where num2 > 2
                let sum = num1 + num2
                orderby sum ascending
            select new { num1, num2, sum };
            foreach(var item in queryResult) {
                Console.WriteLine(item);
            }
        }
    }
}

// { num1 = 1, num2 = 3, sum = 4 }
// { num1 = 1, num2 = 4, sum = 5 }
// { num1 = 2, num2 = 3, sum = 5 }
// { num1 = 1, num2 = 5, sum = 6 }
// { num1 = 2, num2 = 4, sum = 6 }
// { num1 = 3, num2 = 3, sum = 6 }
// { num1 = 2, num2 = 5, sum = 7 }
// { num1 = 3, num2 = 4, sum = 7 }
// { num1 = 4, num2 = 3, sum = 7 }
// { num1 = 1, num2 = 7, sum = 8 }
// { num1 = 3, num2 = 5, sum = 8 }
// { num1 = 4, num2 = 4, sum = 8 }
// { num1 = 5, num2 = 3, sum = 8 }
// { num1 = 2, num2 = 7, sum = 9 }
// { num1 = 4, num2 = 5, sum = 9 }
// { num1 = 5, num2 = 4, sum = 9 }
// { num1 = 1, num2 = 9, sum = 10 }
// { num1 = 3, num2 = 7, sum = 10 }
// { num1 = 5, num2 = 5, sum = 10 }
// { num1 = 7, num2 = 3, sum = 10 }
// { num1 = 2, num2 = 9, sum = 11 }
// { num1 = 4, num2 = 7, sum = 11 }
// { num1 = 7, num2 = 4, sum = 11 }
// { num1 = 3, num2 = 9, sum = 12 }
// { num1 = 5, num2 = 7, sum = 12 }
// { num1 = 7, num2 = 5, sum = 12 }
// { num1 = 4, num2 = 9, sum = 13 }
// { num1 = 5, num2 = 9, sum = 14 }
// { num1 = 7, num2 = 7, sum = 14 }
// { num1 = 7, num2 = 9, sum = 16 }
```

### 19.5.6 `select...group` 子句



### 19.5.7 查询中的匿名类型

```C#
using System;
using System.Linq;
using System.Collections.Generic;

namespace PlayGround
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var studnets = new[] {
                new { LastName = "Jones", FirstName = "Mary", Age = 19, Major = "History" },
                new { LastName = "Smith", FirstName = "Bob", Age = 20, Major = "CompSic" },
                new { LastName = "Fleming", FirstName = "Carol", Age = 21, Major = "History" },
            };

            var queryResult = 
                from student in studnets
                select new { student.LastName, student.FirstName };
            
            foreach (var record in queryResult) {
                Console.WriteLine(record);
            }
            // { LastName = Jones, FirstName = Mary }
            // { LastName = Smith, FirstName = Bob }
            // { LastName = Fleming, FirstName = Carol }

            foreach (var record in queryResult) {
                Console.WriteLine("{0}.{1}", record.FirstName, record.LastName);
            }
            // Mary.Jones
            // Bob.Smith
            // Carol.Fleming
        }
    }
}
```

### 19.5.8 `group` 子句

这个子句的功能是将查询的对象根据一些标准进行分组。有关 `group` 子句需要了解的重要事项如下：

* 如果项包含在查询的结果中，它们就可以根据某个字段的值进行分组。作为分组依据的属性叫做键。
* `group` 子句返回的不是原始数据源中项的枚举，而是返回可以枚举已经形成的项的分组的可枚举类型（？？？？）。
* 分组本身是可枚举类型，它们可以枚举实际的项。

```C#
using System;
using System.Linq;
using System.Collections.Generic;

namespace PlayGround
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var studnets = new[] {
                new { LastName = "Jones", FirstName = "Mary", Age = 19, Major = "History" },
                new { LastName = "Smith", FirstName = "Bob", Age = 20, Major = "CompSic" },
                new { LastName = "Fleming", FirstName = "Carol", Age = 21, Major = "History" },
            };

            var queryResult = 
                from student in studnets
                group student by student.Major;
            
            foreach (var record in queryResult) {
                Console.Write("{0}: ", record.Key);
                foreach (var name in record) {
                    Console.Write("{0}.{1} ", name.FirstName, name.LastName);
                }
                Console.WriteLine("");
            }
        }
    }
}

// History: Mary.Jones Carol.Fleming
// CompSic: Bob.Smith
```

* 从查询表达式返回的对象是从查询中枚举分组结果的可枚举类型。
* 每一个分组由一个叫做键的字段区分。
* 每一个分组本身是可枚举类型并且可以枚举它的项。

### 19.5.9 查询延续： `into` 子句

也就是说，如果已经查询得到了一个可枚举类型，那么可以继续深入这个查询得到的可枚举变量内部再次进行查询。

```C#
using System;
using System.Linq;
using System.Collections.Generic;

namespace PlayGround
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int[] arr1 = new int[] { 12, 45, 56, 76, 76, 87, 9898, 123, 34, 45, 4 };
            int[] arr2 = new int[] { 45, 56, 67, 76, 34, 45, 9898, 4 };

            var queryResult =
            from num1 in arr1
                join num2 in arr2 on num1 equals num2
                into Arr1EqualsArr2 /* 将联结结果命名为 Arr1EqualsArr2 */
                    from elem in Arr1EqualsArr2
                        where elem < 50
            select elem;
            
            foreach (var number in queryResult) {
                Console.Write("{0} ", number);
            }
        }
    }
}

// 45 45 34 45 45 4
```

## 19.6 标准查询运算符

标准查询运算由一系列 API 方法组成，它能让我们查询任何 .NET 数组或者集合。有关查询运算符的重要特性如下：

* 被查询的集合对象叫做序列，它必须事件 `IEnumerable<Typename>` 接口；
* 标准查询运算符使用方法语法；
* 一些运算符返回可枚举对戏那个，而其他返回标量。返回标量的运算符立即执行。
* 很多操作都以一个谓词作为参数。谓词是一个方法，返回 `true` 或者 `false` 。

「序列」是指 **<u>实现了 `IEnumerable<T>` 接口的类</u>** 。

### 19.6.1 标准查询运算符的签名



### 19.6.2 查询表达式和标准查询运算符

```C#
int sizeOfNumber = 
    (from number in numbers
    	where number < 10
    select number).Count();
```

### 19.6.3 将委托作为参数

```C#
using System;
using System.Linq;
using System.Collections.Generic;

namespace PlayGround
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int[] numbers = new int[] { 12, 45, 56, 76, 76, 87, 9898, 123, 34, 45, 4 };

            int countingOdds = (
                from number in numbers
                select number
            ).Count( (x) => { return x % 2 != 0; } );
            Console.WriteLine("Odds: {0}", countingOdds);
        }
    }
}

// Odds: 4
```



## 19.7 LINQ to XML

* 可以使用单一语句自顶向下创建 XML 树。
* 可以不使用包含树的 XML 文档在内存中创建并且操作 XML 。
* 可以不使用 `Text` 子结点来创建和操作字符串结点。
* 在搜索一个 XML 树时，不需要进行遍历。只需要查询树并且返回想要的结果即可。

### 19.7.1 标记语言

**<u>「标记语言」是文档中的一组标签，它提供有关文档的信息并且组织其内容。标记标签并不是文档的数据——它们包含关于数据的数据，有关数据的数据称为「元数据」</u>** 。

### 19.7.2 XML 基础

「元素」是 XML 树的基本要素。每一个元素都有名字并且包含数据，一些元素还可以包含其他被嵌套的元素。元素有开始和关闭标签进行划分。任何元素包含的数据都必须介于开始和关闭的标签之间。

```C#
<EmployeeName>Sally Jones</EmployeeName>
/* 开始标签    数据          结束标签 */

<PhoneNumber /> /* 空元素 */
```

没有内容的元素可以直接由单个标签表示，从每个左尖括号开始，后面是元素名和斜杠，并且以右尖括号结束。

有关 XML 的重要事项如下：

* XML 文档必须有一个根元素来包含所有其他元素。
* XML 标签必须合理嵌套。
* 与 HTML 标签不同，XML 标签是区分大小写的。
* 「XML 特性」是「名字/值」配对，它包含了元素的额外元数据。特性的「值部分」必须包含在引号内，可以是单引号或者双引号。
* XML 文档中的空格是有效的。

```XML
<Employees>
    <Employee>
        <Name>Bob Smith</Name>
        <PhoneNumber>408-555-1000</PhoneNumber>
        <CellPhone />
    </Employee>
</Employees>
```

### 19.7.3 XML 类

三个最重要的类： `XElement` ，`XAttribute` 和 `XDocument` 。

