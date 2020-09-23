using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportServiceAPI.sources.Exceptions
{
	/// <summary>
	/// Исключение нарушения ограничения уникальности
	/// </summary>
	public class UniqueConstraintException : Exception
	{
		/// <summary>
		/// Название столбца, которое вызвало исключение
		/// </summary>
		public string ColumnName { get; set; } = string.Empty;

		public UniqueConstraintException()
		{
		}

		public UniqueConstraintException(string message, string columnName) : base(message)
		{
			ColumnName = columnName;
		}

		public UniqueConstraintException(string message, string columnName, Exception innerException) : base(message, innerException)
		{
			ColumnName = columnName;
		}
	}
}
