using System;

namespace SudokuMinimizer.Database
{
    public static class UtilFunctions
    {

        public static bool IsNull(object dbObject)
        {
            return (dbObject == null || dbObject == DBNull.Value);
        }

        public static string DbString(object dbObject)
        {
            if (IsNull(dbObject)) { return null; }
            else { return dbObject.ToString(); }
        }

        public static double DbDouble(object dbObject)
        {
            if (IsNull(dbObject)) { return 0f; }
            else { return Convert.ToDouble(dbObject); }
        }

        public static int DbInt(object dbObject)
        {
            if (IsNull(dbObject)) { return 0; }
            else { return Convert.ToInt32(dbObject); }
        }

        public static bool DbBool(object dbObject)
        {
            if (IsNull(dbObject)) { return false; }
            else { return Convert.ToBoolean(dbObject); }
        }

        public static DateTime? DbNullableDateTime(object dbObject)
        {
            if (IsNull(dbObject)) { return null; }
            else { return (DateTime?)dbObject; }
        }

        public static DateTime DbDateTime(object dbObject)
        {
            if (IsNull(dbObject)) { return DateTime.MinValue; }
            else { return (DateTime)dbObject; }
        }

        public static TimeSpan DbTimeSpan(object dbObject)
        {
            if (IsNull(dbObject)) { return TimeSpan.MinValue; }
            else { return (TimeSpan)dbObject; }
        }
    }
}
