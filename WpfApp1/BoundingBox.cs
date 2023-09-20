namespace WpfApp1
{
    struct BoundingBox
    {
        public double left;
        public double right;
        public double bottom;
        public double top;

        public BoundingBox(double left, double right, double top, double bottom)
        {
            this.left = left;
            this.right = right;
            this.bottom = bottom;
            this.top = top;
        }

        public bool Intersects(BoundingBox other)
        {
            foreach (BoundingPoint point in other.GetBoundingPoints())
            {
                if (this.Contains(point))
                {
                    return true;
                }
            }
            foreach (BoundingPoint point in this.GetBoundingPoints())
            {
                if (other.Contains(point))
                {
                    return true;
                }
            }
            return false;
        }

        private BoundingPoint[] GetBoundingPoints()
        {
            return new BoundingPoint[]{
                new BoundingPoint(this.left, this.top),
                new BoundingPoint(this.right, this.top),
                new BoundingPoint(this.right, this.bottom),
                new BoundingPoint(this.left, this.bottom)
            };
        }

        public bool Contains(BoundingPoint point)
        {
            return point.X >= this.left && point.X <= this.right && point.Y >= this.top && point.Y <= this.bottom;
        }
    }

    record BoundingPoint(double X, double Y);
}