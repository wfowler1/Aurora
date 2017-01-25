﻿using System;
using System.Drawing;

namespace Aurora.EffectsEngine.Animations
{
    public class AnimationCircle : AnimationFrame
    {
        internal float _radius = 0.0f;
        internal PointF _center = new PointF();

        public float Radius { get { return _radius; } }
        public PointF Center { get { return _center; } }


        public AnimationFrame SetRadius(float radius)
        {
            _radius = radius;
            _dimension = new RectangleF(_center.X - _radius, _center.Y - _radius, 2.0f * _radius, 2.0f * _radius);
            _invalidated = true;

            return this;
        }

        public AnimationFrame SetCenter(PointF center)
        {
            _center = center;
            _dimension = new RectangleF(_center.X - _radius, _center.Y - _radius, 2.0f * _radius, 2.0f * _radius);
            _invalidated = true;

            return this;
        }

        public AnimationCircle()
        {
            _radius = 0;
            _center = new PointF(0, 0);
            _dimension = new RectangleF(_center.X - _radius, _center.Y - _radius, 2.0f * _radius, 2.0f * _radius);
            _color = Utils.ColorUtils.GenerateRandomColor();
            _width = 1;
            _duration = 0.0f;
        }

        public AnimationCircle(Rectangle dimension, Color color, int width = 1, float duration = 0.0f) : base(dimension, color, width, duration)
        {
            _radius = dimension.Width / 2.0f;
            _center = new PointF(dimension.X + _radius, dimension.Y + _radius);
        }

        public AnimationCircle(RectangleF dimension, Color color, int width = 1, float duration = 0.0f) : base(dimension, color, width, duration)
        {
            _radius = dimension.Width / 2.0f;
            _center = new PointF(dimension.X + _radius, dimension.Y + _radius);
        }

        public AnimationCircle(PointF center, float radius, Color color, int width = 1, float duration = 0.0f)
        {
            _radius = radius;
            _center = center;
            _dimension = new RectangleF(_center.X - _radius, _center.Y - _radius, 2.0f * _radius, 2.0f * _radius);
            _color = color;
            _width = width;
            _duration = duration;
        }

        public AnimationCircle(float x, float y, float radius, Color color, int width = 1, float duration = 0.0f)
        {
            _radius = radius;
            _center = new PointF(x, y);
            _dimension = new RectangleF(_center.X - _radius, _center.Y - _radius, 2.0f * _radius, 2.0f * _radius);
            _color = color;
            _width = width;
            _duration = duration;
        }

        public override void Draw(Graphics g, float scale = 1.0f)
        {
            if(_pen == null || _invalidated)
            {
                _pen = new Pen(_color);
                _pen.Width = _width;
                _pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Center;

                _invalidated = false;
            }

            _pen.ScaleTransform(scale, scale);
            RectangleF _scaledDimension = new RectangleF(_dimension.X * scale, _dimension.Y * scale, _dimension.Width * scale, _dimension.Height * scale);

            g.DrawEllipse(_pen, _scaledDimension);
        }

        public override AnimationFrame BlendWith(AnimationFrame otherAnim, double amount)
        {
            if(!(otherAnim is AnimationCircle))
            {
                throw new FormatException("Cannot blend with another type");
            }

            amount = GetTransitionValue(amount);

            RectangleF newrect = new RectangleF((float)(_dimension.X * (1.0 - amount) + otherAnim._dimension.X * (amount)),
                (float)(_dimension.Y * (1.0 - amount) + otherAnim._dimension.Y * (amount)),
                (float)(_dimension.Width * (1.0 - amount) + otherAnim._dimension.Width * (amount)),
                (float)(_dimension.Height * (1.0 - amount) + otherAnim._dimension.Height * (amount))
                );

            int newwidth = (int)((_width * (1.0 - amount)) + (otherAnim._width * (amount)));

            return new AnimationCircle(newrect, Utils.ColorUtils.BlendColors(_color, otherAnim._color, amount), newwidth);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AnimationCircle)obj);
        }

        public bool Equals(AnimationCircle p)
        {
            return _color.Equals(p._color) &&
                _dimension.Equals(p._dimension) &&
                _width.Equals(p._width) &&
                _duration.Equals(p._duration);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + _color.GetHashCode();
                hash = hash * 23 + _dimension.GetHashCode();
                hash = hash * 23 + _width.GetHashCode();
                hash = hash * 23 + _duration.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return "AnimationCircle [ Color: " + _color.ToString() + " Dimensions: " + _dimension.ToString() + " Width: " + _width + "]";
        }
    }
}
