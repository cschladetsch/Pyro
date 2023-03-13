using System;
using NUnit.Framework;

namespace Pyro.TestUnity
{
    [TestFixture]
    public class AppTests
    {
        [SetUp]
        public void BeforeEachTest()
        {
        }

        // see graph at https://docs.google.com/spreadsheets/d/1rVPEuRXezlsEIURaEMnihVuJJ8yexntQ_QcFNBdj1dc/edit#gid=0
        [Test]
        public void FilteredFloat()
        {
            var x = new FilterButterworth(1, 20 , FilterButterworth.PassType.Lowpass);
            for (var i = 0; i < 40; ++i)
            {
                x.Update(1);
                Console.WriteLine($"{x.Value}");
            }
<<<<<<< HEAD
<<<<<<< HEAD
            
=======
>>>>>>> 889fbac (Add Unit test for Butterworth Filter.)
=======
            
>>>>>>> c8d15cc (Add graphed output for Butterworth filter.)
            for (var i = 0; i < 40; ++i)
            {
                x.Update(0);
                Console.WriteLine($"{x.Value}");
            }
        }
    }
}