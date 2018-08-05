using System;
using System.Runtime.Serialization;

namespace ModCore.Logic.Table
{
    public class ColumnNotFoundException : RowNotFoundException
    {
        public object ColumnObject { get; set; }

        public ColumnNotFoundException()
            : base("column not found in table")
        {
        }

        public ColumnNotFoundException(string message)
            : base(message)
        {
        }

        public ColumnNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ColumnNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ColumnNotFoundException(object row, object column)
            : base($"[[{row?.ToString() ?? "null"}]]:[[{column?.ToString() ?? "null"}]]")
        {
            RowObject = row;
            ColumnObject = column;
        }
    }
}