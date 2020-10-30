# 第 13 章 委托和 Lambda 表达式

随着类创建的越来越多，会发现它们的关系存在固定的模式。一个常见的模式就是向方法传递对象，该方法再调用对象的一个方法。

「委托」是一个特殊类，它允许像处理其他任何数据那样处理对方法的引用。

## 13.1 委托概述

C++ 语言可以使用「函数指针」来将函数作为方法实参进行传递。 C# 使用「委托」提供相似的功能，委托允许捕捉对方法的引用，并像传递其他对象那样传递该引用。

### 13.1.1 背景

```C#
/* 代码清单 13.1 BubbleSort() 方法 */

public class SimpleSort
{
    public static void BubbleSort(int[] arr)
    {
		if (arr == null) return;
        for (int i = arr.Length; i > 0; --i) {
            for (int j = 0; j < i - 1; ++j) {
                if (arr[j] > arr[j + 1]) {
                    Swap(ref arr[j], ref arr[j + 1]);
                }
            }
        }
    }
    
    public static void Swap(ref int x, ref int y)
    {
        int temp = x;
        x = y;
        y = temp;
    }
    
    ...
}
```

但是这个代码只有升序，如果我们需要降序或者更多的排序种类该怎么办？

### 13.1.2 委托数据类型

可以考虑将比较方法作为参数传递给排序算法。 **<u>为了能将方法作为参数传递，要有一个能表示方法的数据类型</u>** 。这个数据类型就是委托。

因为它将调用委托给对象引用的方法。可以将方法名作为委托的实例。

```C#
/* 代码清单 13.3 带委托参数的 BubbleSort 方法 */

using System;

namespace PlayGround
{
    public class Program
    {
        public static void Swap(ref int x, ref int y)
        {
            int temp = x;
            x = y;
            y = temp;
        }

        public static void BubbleSort(int[] arr, Func<int, int, bool> Compare)
        {
            if (arr == null) { return; }
            for (int i = arr.Length; i > 0; --i) {
                for (int j = 0; j < i - 1; ++j) {
                    if (Compare(arr[j], arr[j + 1])) {
                        Swap(ref arr[j], ref arr[j + 1]);
                    }
                }
            }
        }

        public static bool isLess(int x, int y)
        {
            return x < y ? true : false;
        }

        public static void Main(String[] args)
        {
            int[] arr = { 5, 3, 7, 2, 8 };
            foreach (int elem in arr) { Console.Write("{0} ", elem); }
            Console.WriteLine("");
            BubbleSort(arr, isLess);
            foreach (int elem in arr) { Console.Write("{0} ", elem); }
        }
    }
}

// OUTS:
// 5 3 7 2 8
// 8 7 5 3 2
```

注意 `Func<int, int, bool>` 委托是强类型的。代表返回的 `bool` 值并且正好接收两个整数的方法，如果参数类型或者数量不匹配就会报错。

## 13.2 声明委托类型

声明委托类型要使用关键字 `delegate` ，后面跟着像是方法声明的内容。该方法的千米昂就是委托能引用的方法的签名。

### 13.2.1 常规用途的委托类型

```C#
/* 代码清单 13.5 声明委托类型 */

using System;

namespace PlayGround
{
    public delegate bool DComparer (int lhs, int rhs);

    public class Program
    {
        public static void Swap(ref int x, ref int y)
        {
            int temp = x;
            x = y;
            y = temp;
        }

        public static void BubbleSort(int[] arr, DComparer compare)
        {
            if (arr == null) { return; }
            for (int i = arr.Length; i > 0; --i) {
                for (int j = 0; j < i - 1; ++j) {
                    if (compare(arr[j], arr[j + 1])) {
                        Swap(ref arr[j], ref arr[j + 1]);
                    }
                }
            }
        }

        public static bool IsLess(int x, int y)
        {
            return x < y ? true : false;
        }

        public static void Main(String[] args)
        {
            int[] arr = { 5, 3, 7, 2, 8 };
            foreach (int elem in arr) { Console.Write("{0} ", elem); }
            Console.WriteLine("");
            DComparer isLess = new DComparer(IsLess);
            BubbleSort(arr, isLess);
            foreach (int elem in arr) { Console.Write("{0} ", elem); }
        }
    }
}
```

### 13.2.2 实例化委托

实例化委托需要和委托类型自身签名对应的一个方法。方法名无关紧要，但是签名剩余部分必须兼容委托的签名。

#### 高级主题：委托的内部机制

委托实际上是个特殊的类。委托必须直接或者间接派生自 `System.Delegate` 。但是编译器不允许声明直接或者间接从这个类派生的类。

> 如果你用过C/C++ 的函数指针，那么跟你说委托就是函数指针，事件就是保存多个函数指针的数组就够了。这就是委托和事件的本质。

#### 如何使用委托

```C#
/* 1. 声明，并且说明该委托的签名 */
public delegate int SomeDelegate (string str, bool boo);

/* 2. 定义符合委托签名（即参数个数与类型一致）的函数 */
private int SomeFunction(string s, bool b);

/* 3. 创建一个委托对象，将这个函数当作实参传给委托对象 */
SomeDelegate sd = new SomeDelegate(SomeFunction);

/* 4. 使用委托对象作为函数调用这个被当作实参传进去的函数 */
sd("someString", true);
```

为啥能这么麻烦？本来可以直接 `SomeFunction("someString", true)` 一步搞定，为啥现在要整整四步？那是因为： 

* 首先 C# 中函数参数不允许传递函数。有了委托相当于把函数包装成一个对象，而 C# 中可以传递对象。

但是把函数当成参数变量传递有什么好处？

* 可以传递函数意味着可以将业务逻辑作为参数传递。能极大的提高函数的通用性。

## 13.3 Lambda 表达式

Lambda 表达式统称为「匿名函数」。 Lambda 表达式本身分为两种：「语句 Lambda」和「表达式 Lambda」。

### 13.3.1 语句 Lambda

语句 Lambda 由形参列表， Lambda 操作符 `=>` 和代码块构成：

```C#
/* 代码清单 13.12 用语句 Lambda 创建委托 */

BubbleSort(arr, (int lhs, int rhs) => { return lhs < rhs; });
```

```C#
/* 代码清单 13.13 在语句 Lambda 中省略参数类型 */

BubbleSort(arr, (lhs, rhs) => { return lhs < rhs; });
```

> 设计规范：如果类型对于读者显而易见，或者无关紧要的细节，那么就可以省略形参列表中的类型。

### 13.3.2 表达式 Lambda

表达式 Lambda 甚至更加简洁，如果一个 Lambda 语句中只需要一个返回语句，这种语句完全可以写成 Lambda 表达式。

```C#
/* 代码清单 13.16 使用表达式 Lambda 传递委托 */

BubbleSort(arr, (lhs, rhs) => lhs < rhs);
```

这个表达式可以解读成：「 若 `lhs` 小于 `rhs` 」 。

目前为止所有 Lambda 表达式并非天生就是 `Func<int, int, bool>` 或者 `Comparer` 类型。它们只是与那个类型兼容，才能转换成那个类型。

## 13.4 匿名方法

也就是在参数列表之前添加关键字 `delegate` ，强调匿名方法必须转换成委托类型。

```C#
/* 代码清单 13.17 在 C# 2.0 中传递匿名方法 */

BubbleSort(
	arr, delegate (int lhs, int rhs) {
        return lhs < rhs;
    }
);
```

> 设计规范：避免在新的代码中使用匿名方法语法，优先使用 Lambda 表达式。

#### 高级主题： Lambda 缘起

Lambda 表达式的概念来自于「阿隆佐・邱奇」，也就是「邱奇记号法」： **<u>如果函数要获取参数 $x$ ，最终的表达式是 $y$ ，就将希腊字母 $\lambda$ 作为前缀，再用点号分隔参数和表达式</u>** 。所以， C# 的Lambda 表达式 `x => y` 用邱奇的记号法应当写为： $\lambda x.y$ 。

### 13.4.1 委托没有结构相等性

也就是说，不能将一个委托类型对象转换成另一个不相关的委托类型，即使两者的形参和返回类型完全一致。

如果需要结构一致但是不相关的新委托类型，为了使用该类型的委托，唯一的办法就是创建新委托对象并且让它引用旧委托的 `Invoke()` 方法：

```C#
public delegate bool DIsLess (int lhs, int rhs);

public bool IsLess(int lhs, int rhs) { return lhs < rhs; }

DIsLess isLess = new DIsLess(IsLess);
Func<bool, int, int> delegateFunc = isLess.Invoke();
```

### 13.4.2 外部变量

在 Lambda 表达式外部声明的局部变量称为该 Lambda 的外部变量。使用外部变量的过程称为「捕捉」。

```C#
/* 代码清单 13.20 在 Lambda 表达式中使用外部变量 */

public class Program
{
    public static void Main(string[] args)
    {
        int[] arr = new int[]{ 5, 2, 4, 6 };
        int comparisonCount = 0;
        ...
        BubbleSort(
        	arr, (int lhs, int rhs) => {
                /* 使用函数中的局部变量 */
                comparisonCount += 1;
                return lhs < rhs;
            }
        );
        ...
    }
}
```

#### 高级主题：不小心捕捉循环变量

```C#
/* 代码清单 13.22 在 C# 5.0 中捕捉循环变量 */

public class CaptureLoop
{
    public static void Main(string[] args)
    {
        string[] items = new string[]{ "Moe", "Larry", "Curly" };
        List<Action> actions = new List<Action>();
        foreach (string item in items) {
            actions.Add( ()=>{Console.WriteLine(item);} );
        }
        foreach (Action action in actions) { action; }
    }
}

// C# 4.0 Outputs:
// Curly
// Curly
// Curly

// C# 5.0 Outputs:
// Moe
// Larry
// Curly
```

4.0 之前当 Lambda 捕捉了外部变量时，会延长外部变量的生命周期，所以就会看到上面的输出。

> 设计规范：避免在匿名函数中捕捉循环变量。

### 13.4.3 表达式树

 



## 13.5 小结

