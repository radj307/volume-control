// ReSharper disable NonReadonlyMemberInGetHashCode

using System;
using System.Xml.Serialization;

namespace Toastify.Common
{
    [Serializable]
    public struct WindowPosition : IEquatable<WindowPosition>
    {
        [XmlAttribute]
        public int Left { get; set; }

        [XmlAttribute]
        public int Top { get; set; }

        public static readonly WindowPosition Zero = new WindowPosition(0, 0);

        public WindowPosition(int left, int top)
        {
            this.Left = left;
            this.Top = top;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is WindowPosition))
                return false;

            WindowPosition w = (WindowPosition)obj;
            return this.Left == w.Left && this.Top == w.Top;
        }

        public bool Equals(WindowPosition other)
        {
            return this.Left == other.Left && this.Top == other.Top;
        }

        public override int GetHashCode()
        {
            return unchecked((this.Left * 397) ^ this.Top);
        }

        public static WindowPosition operator *(WindowPosition w1, int n)
        {
            return new WindowPosition(w1.Left * n, w1.Top * n);
        }

        public static WindowPosition operator /(WindowPosition w1, int n)
        {
            return new WindowPosition(w1.Left / n, w1.Top / n);
        }

        public static bool operator ==(WindowPosition w1, WindowPosition w2)
        {
            return w1.Left == w2.Left && w1.Top == w2.Top;
        }

        public static bool operator !=(WindowPosition w1, WindowPosition w2)
        {
            return !(w1 == w2);
        }
    }
}