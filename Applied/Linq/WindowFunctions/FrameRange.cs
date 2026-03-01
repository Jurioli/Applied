namespace System.Linq.WindowFunctions
{
    public sealed class FrameRange
    {
        internal static readonly FrameRange Default = new FrameRange(FrameBound.UnboundedPreceding, FrameBound.CurrentRow);
        private readonly FrameBound _start, _end;
        public FrameBound Start
        {
            get
            {
                return _start;
            }
        }
        public FrameBound End
        {
            get
            {
                return _end;
            }
        }
        public FrameRange(int start, int end) : this(new FrameBound(start), new FrameBound(end))
        {

        }
        public FrameRange(FrameBound start, FrameBound end)
        {
            _start = start;
            _end = end;
        }
    }
    public sealed class FrameRows
    {
        internal static readonly FrameRows Default = new FrameRows(FrameBound.UnboundedPreceding, FrameBound.CurrentRow);
        private readonly FrameBound _start, _end;
        public FrameBound Start
        {
            get
            {
                return _start;
            }
        }
        public FrameBound End
        {
            get
            {
                return _end;
            }
        }
        public FrameRows(int start, int end) : this(new FrameBound(start), new FrameBound(end))
        {

        }
        public FrameRows(FrameBound start, FrameBound end)
        {
            _start = start;
            _end = end;
        }
    }
    public struct FrameBound
    {
        public static readonly FrameBound CurrentRow = new FrameBound(0, FrameBoundType.Preceding);
        public static readonly FrameBound UnboundedPreceding = new FrameBound(null, FrameBoundType.Preceding);
        public static readonly FrameBound UnboundedFollowing = new FrameBound(null, FrameBoundType.Following);
        public int? Rows { get; set; }
        public FrameBoundType Type { get; set; }
        internal FrameBound(int rows)
        {
            if (rows > 0)
            {
                this.Rows = rows;
                this.Type = FrameBoundType.Following;
            }
            else
            {
                this.Rows = 0 - rows;
                this.Type = FrameBoundType.Preceding;
            }
        }
        public FrameBound(int? rows, FrameBoundType type)
        {
            this.Rows = rows;
            this.Type = type;
        }
        internal bool IsUnbounded(out int offset)
        {
            int? rows = this.Rows;
            if (rows == null)
            {
                offset = 0;
                return true;
            }
            else
            {
                int value = rows.Value;
                if (this.Type == FrameBoundType.Preceding)
                {
                    offset = 0 - value;
                }
                else
                {
                    offset = value;
                }
                return value < 0;
            }
        }
        internal int UnboundedIndex(int lastIndex, int offset)
        {
            return (this.Type == FrameBoundType.Preceding ? 0 : lastIndex) + offset;
        }
    }
    public enum FrameBoundType
    {
        Preceding,
        Following
    }
}
