using System.Collections.Generic;

namespace ModCore.Logic.Table
{
    public class Table<TRow, TColumn, TValue> : Dictionary<TRow, IDictionary<TColumn, TValue>>, 
        ICollection<RowColumnValueTriple<TRow, TColumn, TValue>>, 
        IReadOnlyCollection<RowColumnValueTriple<TRow, TColumn, TValue>>
    {
        public bool IsReadOnly => false;

        public TValue this[TRow row, TColumn column]
        {
            get
            {
                if (!this.TryGetValue(row, out var rowEntry))
                    throw new RowNotFoundException(row);
                if (!rowEntry.TryGetValue(column, out var entry))
                    throw new ColumnNotFoundException(row, column);
                return entry;
            }
            set
            {
                if (!this.TryGetValue(row, out var rowEntry))
                    this[row] = new Dictionary<TColumn, TValue> {[column] = value};

                rowEntry[column] = value;
            }
        }

        public bool TryGetValue(TRow row, TColumn column, out TValue value)
        {
            if (!this.TryGetValue(row, out var rowEntry))
            {
                value = default;
                return false;
            }

            if (!rowEntry.TryGetValue(column, out var entry))
            {
                value = default;
                return false;
            }

            value = entry;
            return true;
        }

        public void Put(TRow row, IEnumerable<KeyValuePair<TColumn, TValue>> columns)
        {
            this[row] = new Dictionary<TColumn, TValue>(columns);
        }


        public void Put(TRow row, KeyValuePair<TColumn, TValue> column)
        {
            this[row] = new Dictionary<TColumn, TValue> {[column.Key] = column.Value};
        }

        public new IEnumerator<RowColumnValueTriple<TRow, TColumn, TValue>> GetEnumerator()
        {
            foreach (var (row, columns) in this.GetEnumerables())
            foreach (var (column, value) in columns)
                yield return new RowColumnValueTriple<TRow, TColumn, TValue>(row, column, value);
        }

        public IEnumerable<KeyValuePair<TRow, IDictionary<TColumn, TValue>>> GetEnumerables()
        {
            using (var enumerators = GetEnumerators())
            while (enumerators.MoveNext())
            {
                yield return enumerators.Current;
            }
        }

        public IEnumerator<KeyValuePair<TRow, IDictionary<TColumn, TValue>>> GetEnumerators() => base.GetEnumerator();

        public void Add(RowColumnValueTriple<TRow, TColumn, TValue> item) => this[item.Row, item.Column] = item.Value;

        public bool Contains(RowColumnValueTriple<TRow, TColumn, TValue> item) => this.TryGetValue(item.Row, item.Column, out _);

        public void CopyTo(RowColumnValueTriple<TRow, TColumn, TValue>[] array, int arrayIndex)
        {
            foreach (var triple in this)
            {
                array[arrayIndex] = triple;
                arrayIndex++;
            }
        }

        public bool Remove(RowColumnValueTriple<TRow, TColumn, TValue> item)
        {
            if (!this.TryGetValue(item.Row, out var rowEntry))
                return false;

            if (!rowEntry.TryGetValue(item.Column, out var entry))
                return false;

            return EqualityComparer<TValue>.Default.Equals(entry, item.Value) 
                   && rowEntry.Remove(item.Column);
        }

        public bool Remove(TRow row, TColumn column) => this.TryGetValue(row, out var rowEntry) 
                                                        && rowEntry.Remove(column);
    }
}