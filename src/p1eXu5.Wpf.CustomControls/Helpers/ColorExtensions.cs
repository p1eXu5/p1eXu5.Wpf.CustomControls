using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using p1eXu5.Wpf.CustomControls.Exceptions;

[assembly: InternalsVisibleTo("p1eXu5.Wpf.CustomControls.Tests")]

namespace p1eXu5.Wpf.CustomControls.Helpers
{

    internal enum ColorHeights
    {
        Lower,
        Higher
    }

    internal static class ColorExtensions
    {
        public static Color ToBrighten( this Color color, byte percent )
        {
            if ( percent > 100 ) throw new PercentException();
            var resColor = new Color {
                R = color.R.CalculateBrighten( percent ), 
                G = color.G.CalculateBrighten( percent ), 
                B = color.B.CalculateBrighten( percent ),
                A = color.A < 0xFF ? color.A.CalculateBrighten( percent ) : color.A
            };
            return resColor;
        }

        public static Color ToDarken(this Color color, byte percent)
        {
            if (percent > 100) throw new PercentException();
            var resColor = new Color
            {
                R = color.R.CalculateDarken( percent ),
                G = color.G.CalculateDarken( percent ),
                B = color.B.CalculateDarken( percent ),
                A = color.A < 0xFF ? color.A.CalculateBrighten(percent) : color.A
            };
            return resColor;
        }

        public static Color Modify( this Color color, byte percent )
        {
            if ( new[] { color.R, color.G, color.B }.Max() > byte.MaxValue / 2 ) {
                return color.ToDarken( percent );
            }

            return color.ToBrighten( percent );
        }

        public static Color Desaturate( this Color color )
        {
            var colorArr = new[] { color.R, color.G, color.B };
            byte averange = ( byte )((colorArr.Max() + colorArr.Min()) / 2);
            return new Color() { A = color.A, R = averange, G = averange, B = averange };
        }

        public static ColorHeights GetColorHeight( this Color color )
        {
            var colorArr = new[] { color.R, color.G, color.B };
            if ( colorArr.Max() > byte.MaxValue / 2 ) return ColorHeights.Higher;

            return ColorHeights.Lower;
        }

        private static byte CalculateBrighten( this byte colorValue, byte percent )
        {
            checked {
                return (byte)( ((byte.MaxValue - colorValue) * percent) / 100 + colorValue );
            }
        }

        private static byte CalculateDarken(this byte colorValue, byte percent)
        {
            checked
            {
                return (byte)( colorValue - ( (colorValue * percent) / 100 ) ) ;
            }
        }
    }
}
