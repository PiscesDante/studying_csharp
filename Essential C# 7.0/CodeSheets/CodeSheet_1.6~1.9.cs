/**
 * 代码清单 1.6
 * 一行上的多条语句
*/

System.Console.WriteLine("Up");System.Console.WriteLine("Down");

/**
 * 代码清单 1.7
 * 一条语句跨越多条行
*/

System.Console.WriteLine(
    "Hello, my name is Inigo Montoya."
);

/**
 * 代码清单 1.8
 * 不缩进
*/

public class HelloWorld
{
public static void Main(string[] args)
{
System.Console.WriteLine("Hello Inigo Montoya.");
}
}

/**
 * 代码清单 1.9
 * 删除一切可以删除的空白
*/

public class HelloWorld{public static void Main(string[] args)
{System.Console.WriteLine("Hello Inigo Montoya.");}}

/**
 * 为了增强可读性，用空白对代码进行缩进很有必要
 * 本书约定每个大括号都单独占一行，并缩进大括号内的代码
 * 这不是强制标准，而是一种风格
*/