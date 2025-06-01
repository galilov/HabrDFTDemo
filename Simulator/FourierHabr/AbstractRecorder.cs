namespace FourierHabr
{
    internal abstract class AbstractRecorder
    {
        protected readonly float _amplitude;
        protected readonly int _x, _y, _width, _height;
        protected readonly LinkedList<double> _values = new LinkedList<double>();
        protected readonly int _pointerWidth = 20, _pointerHeight = 8;
        protected readonly double _k, _halfPointerHeight;
        protected double _nStep = 1;
        protected float _speed;
        protected PointDbl _ptPrev = PointDbl.Empty;
        protected readonly Pen _redPen = new Pen(Color.Red, 1f);
        protected readonly Pen _ltGridPen = new Pen(Color.FromArgb(200, 200, 200), 1f);
        protected readonly Pen _gridPen = new Pen(Color.FromArgb(150, 150, 150), 1f);
        protected readonly Pen _gridPenThik = new Pen(Color.FromArgb(150, 150, 150), 2f);
        protected readonly Pen _graphPen = new Pen(Color.Blue, 1.5f);
        protected readonly Pen _dottedGraphPen;
        protected readonly Brush _graphBrush = new SolidBrush(Color.Blue);
        public readonly Font _font = new Font("Consolas", 14f, FontStyle.Bold);
        public readonly Font _fontChartTime = new Font("Consolas", 10f, FontStyle.Regular);
        protected readonly int _markerIntervalInSteps = 5;

        public AbstractRecorder(float amplitude, float speed, int x, int y, int width, int height)
        {
            _amplitude = amplitude;
            _speed = speed;
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            _k = (_height - _pointerHeight) / (2.0 * _amplitude);
            _halfPointerHeight = _pointerHeight / 2f;
            _dottedGraphPen = new Pen(Color.Blue, 1f);
            _dottedGraphPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
        }

        public abstract void RegisterValue(double value);
        public abstract void Render(Graphics g);

        public void NextStep()
        {
            _nStep += 1;
        }
        public void Clear()
        {
            _values.Clear();
            _nStep = 0;
            _ptPrev = PointDbl.Empty;
        }

        protected void RenderPointer(Graphics g, float v, float x, bool toTheRight)
        {
            if (toTheRight)
            {
                g.DrawLines(_redPen,
                    [
                        new PointF(x, v),
                        new PointF(x - 5, (float)(v - _halfPointerHeight)),
                        new PointF(x - _pointerWidth - 1, (float)(v + _halfPointerHeight)),
                        new PointF(x - 5, (float)(v + _halfPointerHeight)),
                        new PointF(x, v)
                    ]
                );
                var d = x - _pointerWidth - 1;
                g.DrawLine(_redPen, d, _y, d, _y + _height);
                var d1 = x - _pointerWidth + 3;
                g.DrawLine(_redPen, d1, _y, d1, _y + _height);
            }
            else
            {
                g.DrawLines(_redPen,
                    [
                        new PointF(x, v),
                        new PointF(x + 5, (float)(v - _halfPointerHeight)),
                        new PointF(x + _pointerWidth - 1, (float)(v - _halfPointerHeight)),
                        new PointF(x + _pointerWidth - 1, (float)(v + _halfPointerHeight)),
                        new PointF(x + 5, (float)(v + _halfPointerHeight)),
                        new PointF(x, v)
                    ]
                );
                var d = x + _pointerWidth - 1;
                g.DrawLine(_redPen, d, _y, d, _y + _height);
                var d1 = x + _pointerWidth - 5;
                g.DrawLine(_redPen, d1, _y, d1, _y + _height);
            }
        }

        protected double ScaledValue(double value)
        {
            return _y + _height / 2f - _k * value;
        }
    }
}
