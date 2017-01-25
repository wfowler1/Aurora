﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Aurora.EffectsEngine.Animations
{
    public class AnimationLine : AnimationFrame
    {
        private PointF _start_point;
        private PointF _end_point;
        private Color _end_color;

        public PointF StartPoint { get { return _start_point; } }
        public PointF EndPoint { get { return _end_point; } }
        public Color EndColor { get { return _end_color; } }

        public AnimationFrame SetStartPoint(PointF startPoint)
        {
            _start_point = startPoint;
            _invalidated = true;

            return this;
        }

        public AnimationFrame SetEndPoint(PointF endPoint)
        {
            _end_point = endPoint;
            _invalidated = true;

            return this;
        }

        public AnimationFrame SetEndColor(Color endColor)
        {
            _end_color = endColor;
            _invalidated = true;

            return this;
        }

        public AnimationLine(PointF start_point, PointF end_point, Color color, int width = 1, float duration = 0.0f)
        {
            _start_point = start_point;
            _end_point = end_point;
            _color = color;
            _end_color = color;
            _width = width;
            _duration = duration;
        }

        public AnimationLine(Point start_point, Point end_point, Color color, int width = 1, float duration = 0.0f)
        {
            _start_point = start_point;
            _end_point = end_point;
            _color = color;
            _end_color = color;
            _width = width;
            _duration = duration;
        }

        public AnimationLine(float start_x, float start_y, float end_x, float end_y, Color color, int width = 1, float duration = 0.0f)
        {
            _start_point = new PointF(start_x, start_y);
            _end_point = new PointF(end_x, end_y); ;
            _color = color;
            _end_color = color;
            _width = width;
            _duration = duration;
        }

        public AnimationLine(PointF start_point, PointF end_point, Color start_color, Color end_color, int width = 1, float duration = 0.0f)
        {
            _start_point = start_point;
            _end_point = end_point;
            _color = start_color;
            _end_color = end_color;
            _width = width;
            _duration = duration;
        }

        public AnimationLine(Point start_point, Point end_point, Color start_color, Color end_color, int width = 1, float duration = 0.0f)
        {
            _start_point = start_point;
            _end_point = end_point;
            _color = start_color;
            _end_color = end_color;
            _width = width;
            _duration = duration;
        }

        public AnimationLine(float start_x, float start_y, float end_x, float end_y, Color start_color, Color end_color, int width = 1, float duration = 0.0f)
        {
            _start_point = new PointF(start_x, start_y);
            _end_point = new PointF(end_x, end_y); ;
            _color = start_color;
            _end_color = end_color;
            _width = width;
            _duration = duration;
        }

        public override void Draw(Graphics g, float scale = 1.0f)
        {
            if (_start_point.Equals(_end_point))
                return;

            if (_pen == null || _invalidated)
            {
                _pen = new Pen(new LinearGradientBrush(_start_point, _end_point, _color, _end_color));
                _pen.Width = _width;
                _pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Center;

                _invalidated = false;
            }

            _pen.ScaleTransform(scale, scale);
            PointF _scaledStartPoint = new PointF(_start_point.X * scale, _start_point.Y * scale);
            PointF _scaledEndPoint = new PointF(_end_point.X * scale, _end_point.Y * scale);

            g.DrawLine(_pen, _scaledStartPoint, _scaledEndPoint);

        }

        public override AnimationFrame BlendWith(AnimationFrame otherAnim, double amount)
        {
            if (!(otherAnim is AnimationLine))
            {
                throw new FormatException("Cannot blend with another type");
            }

            amount = GetTransitionValue(amount);

            PointF newstart = new PointF((float)(_start_point.X * (1.0 - amount) + (otherAnim as AnimationLine)._start_point.X * (amount)),
                (float)(_start_point.Y * (1.0 - amount) + (otherAnim as AnimationLine)._start_point.Y * (amount))
                );

            PointF newend = new PointF((float)(_end_point.X * (1.0 - amount) + (otherAnim as AnimationLine)._end_point.X * (amount)),
                (float)(_end_point.Y * (1.0 - amount) + (otherAnim as AnimationLine)._end_point.Y * (amount))
                );

            int newwidth = (int)Math.Round((_width * (1.0 - amount)) + (otherAnim._width * (amount)));

            return new AnimationLine(newstart, newend, Utils.ColorUtils.BlendColors(_color, otherAnim._color, amount), Utils.ColorUtils.BlendColors(_end_color, (otherAnim as AnimationLine)._end_color, amount), newwidth);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AnimationLine)obj);
        }

        public bool Equals(AnimationLine p)
        {
            return _color.Equals(p._color) &&
                _end_color.Equals(p._end_color) &&
                _start_point.Equals(p._start_point) &&
                _end_point.Equals(p._end_point) &&
                _width.Equals(p._width);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + _color.GetHashCode();
                hash = hash * 23 + _end_color.GetHashCode();
                hash = hash * 23 + _start_point.GetHashCode();
                hash = hash * 23 + _end_point.GetHashCode();
                hash = hash * 23 + _width.GetHashCode();
                hash = hash * 23 + _duration.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return "AnimationLine [ Start Color: " + _color.ToString() + " End Color: " + _color.ToString() + " Start Point: " + _start_point.ToString() + " End Point: " + _end_point.ToString() + " Width: " + _width + "]";
        }

    }
}
