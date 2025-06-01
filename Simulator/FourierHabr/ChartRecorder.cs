namespace FourierHabr
{
    internal class ChartRecorder : AbstractRecorder
    {
        public ChartRecorder(float amplitude, float speed, int x, int y, int width, int height) :
            base(amplitude, speed, x, y, width, height)
        {
        }

        public override void RegisterValue(double value)
        {
            if (value < -_amplitude) value = -_amplitude;
            else if (value > _amplitude) value = _amplitude;
            if (_values.Count > (_width - _pointerWidth) / _speed)
            {
                _values.RemoveFirst();
            }
            _values.AddLast(value);
        }

        public override void Render(Graphics g)
        {
            int nValues = Math.Min(_width - _pointerWidth, _values.Count);
            var xOffset = _x + _width - _pointerWidth;
            var scaledValue = ScaledValue(_values.Last());
            if (_ptPrev.IsEmpty)
            {
                _ptPrev = new PointDbl(xOffset, scaledValue);
            }
            var prevClip = g.ClipBounds;
            g.SetClip(new RectangleF(_x, _y, _width, _height));
            RenderGrid(g);
            RenderPointer(g, (float)scaledValue, _x + _width - _pointerWidth, false);
            double i = 0;

            var node = _values.Last;
            while (node != null)
            {
                var ptCurrent = new PointDbl(xOffset - i, ScaledValue(node.Value));
                g.DrawLine(_graphPen, _ptPrev.PointF, ptCurrent.PointF);
                int k = (int)(_nStep - i);
                if (k % _markerIntervalInSteps == 0)
                {
                    g.FillEllipse(_graphBrush, new RectangleF((float)(ptCurrent.X - 2), (float)(ptCurrent.Y - 2), 4, 4));
                }
                _ptPrev = ptCurrent;
                i += _speed;
                node = node.Previous;
            }
            _ptPrev = PointDbl.Empty;
            g.SetClip(prevClip);
        }

        private void RenderGrid(Graphics g)
        {
            const int cellAggregate = 5;
            const int vCells = 20;
            const int frameRate = 30;
            var r = new RectangleF(_x, _y, _width, _height);
            g.DrawRectangle(Pens.Black, r);
            g.FillRectangle(Brushes.White, r);
            float dotPerSecond = frameRate / _speed;// (_width - _pointerWidth) / (float)hCells;
            float vertDelta = (_height - _pointerHeight) / (float)vCells;
            var xWidth = _width - _pointerWidth;

            int n = 0;
            for (float x = 0; x < _width + cellAggregate * dotPerSecond; x += dotPerSecond, n++)
            {
                var startX = _x + x - (_nStep % (cellAggregate * dotPerSecond));
                if (startX > _x && startX < _x + _width)
                {
                    var pt1 = new PointF((float)startX, (float)(_y + _halfPointerHeight));
                    var pt2 = new PointF((float)startX, (float)(_y + _height - _halfPointerHeight));
                    var pen = n % cellAggregate == 0 ? _gridPen : _ltGridPen;
                    g.DrawLine(pen, pt1, pt2);
                    if (n % cellAggregate == 0)
                    {
                        var t = (x - _nStep % (cellAggregate * dotPerSecond) + _nStep - xWidth) / dotPerSecond;
                        string s = string.Format("{0:d}", (int)(t));
                        var sz = g.MeasureString(s, _fontChartTime);
                        PointF ptCaption = new PointF(pt2.X - sz.Width / 2, _y + _height / 2f + 2);
                        g.FillRectangle(Brushes.White, new RectangleF(ptCaption, sz));
                        g.DrawString(s, _fontChartTime, _gridPen.Brush, ptCaption);
                    }
                }
            }
            n = 0;
            for (double y = _halfPointerHeight; y < _height - _halfPointerHeight + vertDelta; y += vertDelta, n++)
            {
                var pt1 = new PointF(_x, (float)(_y + y));
                var pt2 = new PointF(_x + _width, (float)(_y + y));
                if (n == vCells / 2)
                {
                    g.DrawLine(_gridPenThik, pt1, pt2);
                }
                else
                {
                    var pen = n % cellAggregate == 0 ? _gridPen : _ltGridPen;
                    g.DrawLine(pen, pt1, pt2);
                }
            }
        }
    }
}
