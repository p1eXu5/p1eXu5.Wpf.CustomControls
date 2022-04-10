using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using p1eXu5.Wpf.CustomControls.Helpers;

namespace p1eXu5.Wpf.CustomControls.Tests.UnitTests.Helpers
{

    [TestFixture]
    public class ColorExtensionsTests
    {
        [Test]
        public void ToBrighten_ZeroAlfa_ReturnsNotZeroAlfa()
        {
            Color color = new Color() { R = 255, G = 255, B = 255, A = 0 };
            Assert.That( color.ToBrighten( 30 ).A, Is.GreaterThan( 0 ) );
        }

        [Test]
        public void ToBrighten_TransparentColor_ReturnsNotZeroAlfa()
        {
            Color color = Colors.Transparent;

            var brightenColor = color.ToBrighten( 30 );

            Assert.That(brightenColor.A, Is.GreaterThan(0));
        }

        #region Factory
        // Insert factory methods and test class variables hear:

        #endregion
    }

}
