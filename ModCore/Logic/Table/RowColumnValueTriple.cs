using System.ComponentModel;
using System.Text;

namespace ModCore.Logic.Table
{
    public readonly struct RowColumnValueTriple<TRow, TColumn, TValue>
    {
        public RowColumnValueTriple(TRow row, TColumn column, TValue value)
        {
            this.Row = row;
            this.Column = column;
            this.Value = value;
        }

        public TRow Row { get; }
        public TColumn Column { get; }
        public TValue Value { get; }

        public override string ToString()
        {
            var sb = new StringBuilder(16);
            sb.Append('[');
            if (this.Row != null) sb.Append(this.Row);
            sb.Append(", ");
            if (this.Column != null) sb.Append(this.Column);
            sb.Append(", ");
            if (this.Value != null) sb.Append(this.Value);
            sb.Append(']');
            return sb.ToString();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out TRow row, out TColumn column, out TValue value)
        {
            row = this.Row;
            column = this.Column;
            value = this.Value;
        }
    }
}