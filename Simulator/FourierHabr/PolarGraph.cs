namespace FourierHabr
{
    internal class PolarGraph : AbstractRecorder
    {
        public PolarGraph(float amplitude, float speed, int x, int y, int width) :
           base(amplitude, speed, x, y, width, width)
        {
        }

        public override void RegisterValue(double value)
        {
            if (value < -_amplitude) value = -_amplitude;
            else if (value > _amplitude) value = _amplitude;
            if (_values.Count > 10000)
            {
                _values.RemoveFirst();
            }
            _values.AddLast(value);
        }

        public override void Render(Graphics g)
        {
            int nValues = Math.Min(_width - _pointerWidth, _values.Count);
            var zeroPos = ScalePosition(new PointDbl(0, 0));
            PointDbl ptCurrent = PointDbl.Empty, ptPrev = ScalePosition(GetUnscaledPosition(_values.First(), 0));
            RenderGrid(g);
            int i = 0;
            var node = _values.First;
            var mPos = new PointDbl(0, 0);
            string s = "O";
            while (node != null)
            {
                var pos = GetUnscaledPosition(node.Value, i);
                ptCurrent = ScalePosition(pos);
                g.DrawLine(_graphPen, ptPrev.PointF, ptCurrent.PointF);
                mPos.X += pos.X;
                mPos.Y += pos.Y;
                if (i % _markerIntervalInSteps == 0)
                {
                    g.FillEllipse(_graphBrush, new RectangleF((float)(ptCurrent.X - 2), (float)(ptCurrent.Y - 2), 4, 4));
                }

                g.DrawString(s, _font, Brushes.Black, zeroPos.PointF);
                ptPrev = ptCurrent;
                i++;
                node = node.Next;
            }
            if (!ptCurrent.IsEmpty)
                g.DrawLine(_redPen, zeroPos.PointF, ptCurrent.PointF);

            mPos.X /= _values.Count;
            mPos.Y /= _values.Count;

            //var mPos = new PointDbl(0, 6);
            RenderM(g, mPos);
        }

        private void RenderGrid(Graphics g)
        {
            const int vCellAggregate = 5;
            const int hCells = 12;
            const int vCells = 10;
            var radius = (_width) / 2.0;
            var rect = new RectangleF(_x, _y, _width, _height);
            g.FillEllipse(Brushes.White, rect);
            int n = 0;
            var circleSpace = (float)(_height - _pointerHeight) / vCells / 2;
            for (double i = (_height - _pointerHeight) / 2; i > 0; i -= circleSpace)
            {
                if (i > 0)
                {
                    var rectCell = new RectangleF((float)(_x + radius - i), (float)(_y + radius - i), (float)(i * 2), (float)(i * 2));
                    var pen = n % vCellAggregate == 0 ? _gridPen : _ltGridPen;
                    g.DrawEllipse(pen, rectCell);
                }
                n++;
            }
            n = 0;
            var cellAngle = 360 / hCells;
            for (double i = 0; i < 360.0; i += cellAngle)
            {
                var rads = i * Math.PI / 180.0;
                var cos = Math.Cos(rads);
                var sin = Math.Sin(rads);
                var x1 = cos * 2;
                var y1 = sin * 2;
                var x2 = cos * radius;
                var y2 = sin * radius;
                var pt1 = new PointF((float)(_x + radius + x1), (float)(_y + radius - y1));
                var pt2 = new PointF((float)(_x + radius + x2), (float)(_y + radius - y2));
                Pen pen;
                if (n == 0)
                {
                    g.DrawString("X", _font, Brushes.Black, pt2.X, pt2.Y);
                    pen = _gridPenThik;
                }
                else if (n == hCells / 4)
                {
                    g.DrawString("Y", _font, Brushes.Black, pt2.X, pt2.Y);
                    pen = _gridPenThik;
                }
                else if (n == hCells * 2 / 4 || n == hCells * 3 / 4)
                {
                    pen = _gridPenThik;
                }
                else
                {
                    pen = /*n % cellAggregate == 0 ? _gridPen :*/ _ltGridPen;
                }
                g.DrawLine(pen, pt1, pt2);
                n++;
            }
            g.DrawEllipse(Pens.Black, rect);

        }

        private void RenderM(Graphics g, PointDbl mPos)
        {
            var radius = _width / 2.0;
            var ptCenter = ScalePosition(mPos);
            var rect = new RectangleF((float)(ptCenter.X - 3), (float)(ptCenter.Y - 3), 6, 6);
            g.DrawString("M", _font, Brushes.Black, rect.X, rect.Y);
            var pt1 = new PointF((float)(_x + radius), (float)(_y + radius));
            g.DrawLine(_dottedGraphPen, pt1, ptCenter.PointF);
            g.DrawEllipse(_redPen, rect);
            var d = mPos.ToVector2().Length();
            string s = string.Format("Mx = {0:f3}\nMy = {1:f3}\nMd = {2:f3}", mPos.X, mPos.Y, d);
            var size = g.MeasureString(s, _font);
            rect = new RectangleF(_x + _width - size.Width, _y + _height, size.Width, size.Height);
            g.FillRectangle(Brushes.White, rect);
            g.DrawString(s, _font, Brushes.Black, _x + _width - size.Width, _y + _height);
        }
        private PointDbl ScalePosition(PointDbl pos)
        {
            return new PointDbl(_x + _width / 2.0 + pos.X * _k, _y + _height / 2.0 - pos.Y * _k);
        }

        private PointDbl GetUnscaledPosition(double value, double angleDegrees)
        {
            var rads = angleDegrees * Math.PI * _speed / 180.0;
            return new PointDbl(Math.Cos(rads) * value, Math.Sin(rads) * value);
        }
    }
}
