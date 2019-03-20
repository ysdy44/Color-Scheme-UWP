using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;

namespace 配色pro.Library
{
    public struct Transformer
    {
        

        public float Width;
        public float Height;

        public float Scale;// = 1.0f;

        public Vector2 Position;


        public Matrix3x2 Matrix =>
            Matrix3x2.CreateTranslation(-this.Width / 2, -this.Height / 2) *
            Matrix3x2.CreateScale(this.Scale) *
            Matrix3x2.CreateTranslation(this.Position);

        public Vector2 BitmapPoint(Vector2 point) => (point - this.Position) / this.Scale + new Vector2(this.Width / 2, this.Height / 2);

        public static Transformer Zero => new Transformer();
        public static Transformer One => new Transformer() { Scale = 1};




    }
}
