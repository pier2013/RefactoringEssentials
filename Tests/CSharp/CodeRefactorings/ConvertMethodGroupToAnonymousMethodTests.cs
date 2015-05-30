using NUnit.Framework;
using RefactoringEssentials.CSharp.CodeRefactorings;

namespace RefactoringEssentials.Tests.CSharp.CodeRefactorings
{
    [TestFixture]
    public class ConvertMethodGroupToAnonymousMethodTests : ContextActionTestBase
    {
        [Test]
        public void TestVoidMethod()
        {
            Test<ConvertMethodGroupToAnonymousMethodCodeRefactoringProvider>(@"
using System;
public class Test
{
    void Foo ()
    {
        Action act = $Foo;
    }
}
", @"
using System;
public class Test
{
    void Foo ()
    {
        Action act = delegate
        {
            Foo();
        };
    }
}
");
        }

        [Test]
        public void TestVoidMethodWithComment()
        {
            Test<ConvertMethodGroupToAnonymousMethodCodeRefactoringProvider>(@"
using System;
public class Test
{
    void Foo ()
    {
		// Some comment
        Action act = $Foo;
    }
}
", @"
using System;
public class Test
{
    void Foo ()
    {
		// Some comment
        Action act = delegate
        {
            Foo();
        };
    }
}
");
        }

        [Test]
        public void TestParameter()
        {
            Test<ConvertMethodGroupToAnonymousMethodCodeRefactoringProvider>(@"
using System;
public class Test
{
    void Foo (int x, int y)
    {
        Action<int,int> act = $Foo;
    }
}
", @"
using System;
public class Test
{
    void Foo (int x, int y)
    {
        Action<int,int> act = delegate (int arg1, int arg2)
        {
            Foo(arg1, arg2);
        };
    }
}
");
        }

        [Test]
        public void TestFunction()
        {
            Test<ConvertMethodGroupToAnonymousMethodCodeRefactoringProvider>(@"
using System;
public class Test
{
    bool Foo (int x, int y)
    {
        Func<int,int,bool> act = $Foo;
    }
}
", @"
using System;
public class Test
{
    bool Foo (int x, int y)
    {
        Func<int,int,bool> act = delegate (int arg1, int arg2)
        {
            return Foo(arg1, arg2);
        };
    }
}
");
        }

        [Test]
        public void TestOverloads()
        {
            Test<ConvertMethodGroupToAnonymousMethodCodeRefactoringProvider>(@"
using System;
public class Test
{
    static void Foo (int x) { }
    static void Foo (int x, int y) { }
    static void Foo () { }

    void Bar ()
    {
        Action<int, int> act = Test.$Foo;
    }
}", @"
using System;
public class Test
{
    static void Foo (int x) { }
    static void Foo (int x, int y) { }
    static void Foo () { }

    void Bar ()
    {
        Action<int, int> act = delegate (int arg1, int arg2)
        {
            Test.Foo(arg1, arg2);
        };
    }
}");
        }
    }
}