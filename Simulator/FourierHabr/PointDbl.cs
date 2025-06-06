﻿using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace FourierHabr
{
    public struct PointDbl : IEquatable<PointDbl>
    {
        /// <summary>
        /// Creates a new instance of the <see cref='System.Drawing.PointF'/> class with member data left uninitialized.
        /// </summary>
        public static readonly PointDbl Empty;
        private double x; // Do not rename (binary serialization)
        private double y; // Do not rename (binary serialization)

        /// <summary>
        /// Initializes a new instance of the <see cref='System.Drawing.PointF'/> class with the specified coordinates.
        /// </summary>
        public PointDbl(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='System.Drawing.PointF'/> struct from the specified
        /// <see cref="System.Numerics.Vector2"/>.
        /// </summary>
        public PointDbl(Vector2 vector)
        {
            x = vector.X;
            y = vector.Y;
        }

        /// <summary>
        /// Creates a new <see cref="System.Numerics.Vector2"/> from this <see cref="System.Drawing.PointF"/>.
        /// </summary>
        public Vector2 ToVector2() => new Vector2((float)x, (float)y);

        /// <summary>
        /// Gets a value indicating whether this <see cref='System.Drawing.PointF'/> is empty.
        /// </summary>
        [Browsable(false)]
        public readonly bool IsEmpty => x == 0f && y == 0f;

        /// <summary>
        /// Gets the x-coordinate of this <see cref='System.Drawing.PointF'/>.
        /// </summary>
        public double X
        {
            readonly get => x;
            set => x = value;
        }

        /// <summary>
        /// Gets the y-coordinate of this <see cref='System.Drawing.PointF'/>.
        /// </summary>
        public double Y
        {
            readonly get => y;
            set => y = value;
        }

        /// <summary>
        /// Converts the specified <see cref="System.Drawing.PointF"/> to a <see cref="System.Numerics.Vector2"/>.
        /// </summary>
        public static explicit operator Vector2(PointDbl point) => point.ToVector2();

        /// <summary>
        /// Converts the specified <see cref="System.Numerics.Vector2"/> to a <see cref="System.Drawing.PointF"/>.
        /// </summary>
        public static explicit operator PointDbl(Vector2 vector) => new PointDbl(vector);

        /// <summary>
        /// Translates a <see cref='System.Drawing.PointF'/> by a given <see cref='System.Drawing.Size'/> .
        /// </summary>
        public static PointDbl operator +(PointDbl pt, Size sz) => Add(pt, sz);

        /// <summary>
        /// Translates a <see cref='System.Drawing.PointF'/> by the negative of a given <see cref='System.Drawing.Size'/> .
        /// </summary>
        public static PointDbl operator -(PointDbl pt, Size sz) => Subtract(pt, sz);

        /// <summary>
        /// Translates a <see cref='System.Drawing.PointF'/> by a given <see cref='System.Drawing.SizeF'/> .
        /// </summary>
        public static PointDbl operator +(PointDbl pt, SizeF sz) => Add(pt, sz);

        /// <summary>
        /// Translates a <see cref='System.Drawing.PointF'/> by the negative of a given <see cref='System.Drawing.SizeF'/> .
        /// </summary>
        public static PointDbl operator -(PointDbl pt, SizeF sz) => Subtract(pt, sz);

        /// <summary>
        /// Compares two <see cref='System.Drawing.PointF'/> objects. The result specifies whether the values of the
        /// <see cref='System.Drawing.PointF.X'/> and <see cref='System.Drawing.PointF.Y'/> properties of the two
        /// <see cref='System.Drawing.PointF'/> objects are equal.
        /// </summary>
        public static bool operator ==(PointDbl left, PointDbl right) => left.X == right.X && left.Y == right.Y;

        /// <summary>
        /// Compares two <see cref='System.Drawing.PointF'/> objects. The result specifies whether the values of the
        /// <see cref='System.Drawing.PointF.X'/> or <see cref='System.Drawing.PointF.Y'/> properties of the two
        /// <see cref='System.Drawing.PointF'/> objects are unequal.
        /// </summary>
        public static bool operator !=(PointDbl left, PointDbl right) => !(left == right);

        /// <summary>
        /// Translates a <see cref='System.Drawing.PointF'/> by a given <see cref='System.Drawing.Size'/> .
        /// </summary>
        public static PointDbl Add(PointDbl pt, Size sz) => new PointDbl(pt.X + sz.Width, pt.Y + sz.Height);

        /// <summary>
        /// Translates a <see cref='System.Drawing.PointF'/> by the negative of a given <see cref='System.Drawing.Size'/> .
        /// </summary>
        public static PointDbl Subtract(PointDbl pt, Size sz) => new PointDbl(pt.X - sz.Width, pt.Y - sz.Height);

        /// <summary>
        /// Translates a <see cref='System.Drawing.PointF'/> by a given <see cref='System.Drawing.SizeF'/> .
        /// </summary>
        public static PointDbl Add(PointDbl pt, SizeF sz) => new PointDbl(pt.X + sz.Width, pt.Y + sz.Height);

        /// <summary>
        /// Translates a <see cref='System.Drawing.PointF'/> by the negative of a given <see cref='System.Drawing.SizeF'/> .
        /// </summary>
        public static PointDbl Subtract(PointDbl pt, SizeF sz) => new PointDbl(pt.X - sz.Width, pt.Y - sz.Height);

        public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is PointDbl && Equals((PointDbl)obj);

        public readonly bool Equals(PointDbl other) => this == other;

        public override readonly int GetHashCode() => HashCode.Combine(X.GetHashCode(), Y.GetHashCode());

        public override readonly string ToString() => $"{{X={x}, Y={y}}}";

        public PointF PointF
        {
            get
            {
                return new PointF((float)X, (float)Y);
            }
        }
    }
}

