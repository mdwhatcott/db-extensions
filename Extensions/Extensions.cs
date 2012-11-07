namespace Extensions
{
	using System.Collections.Generic;
	using System.Data;

	public static class Extensions
	{
		public static IDbCommand WithCommand(
			this IDbConnection connection, string statement, params object[] numberedArgs)
		{
			var command = connection.CreateCommand();
			command.CommandText = statement;
			if (numberedArgs != null)
				foreach (var arg in numberedArgs)
					command = command.WithNumberedParameter(arg);

			return command;
		}
		public static IDbCommand WithParameter(this IDbCommand command, string name, object value)
		{
			try
			{
				var parameter = command.CreateParameter();
				parameter.ParameterName = name;
				parameter.Value = value;
				command.Parameters.Add(parameter);
				return command;
			}
			catch
			{
				command.Dispose();
				throw;
			}
		}
		public static IDbCommand WithNumberedParameter(this IDbCommand command, object value)
		{
			return command.WithParameter("@" + command.Parameters.Count, value);
		}
		public static object AndExecuteScalar(this IDbCommand command)
		{
			using (command)
				return command.ExecuteScalar();
		}
		public static int AndExecuteNonQuery(this IDbCommand command)
		{
			using (command)
				return command.ExecuteNonQuery();
		}
		public static IEnumerable<IDataReader> AndExecuteReader(this IDbCommand command)
		{
			using (command)
			using (var reader = command.ExecuteReader())
				while (reader.Read())
					yield return reader;
		}
	}
}