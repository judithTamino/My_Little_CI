using DemoProject;

namespace TestProject
{

    public class Tests
    {
        public Calc calc; 

        [SetUp]
        public void Setup()
        {
            calc = new Calc();
        }


        [Test]
        public void Test1()
        {

            int ret = calc.Check(1, 1);

            Assert.That(2,Is.EqualTo(ret));
        }

        public void Test2()
        {

            int ret = calc.Check(5, 7);

            Assert.That(12, Is.EqualTo(ret));
        }

    }



}