using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace CampusPartTime.DAL
{
    /// <summary>
    /// 数据访问基础类：统一连接管理、参数化命令执行与异常包装。
    /// 全部对外方法只接受 SqlParameter 数组传参，禁止在调用处拼接 SQL 字符串，
    /// 从数据访问层统一阻断 SQL 注入。
    /// </summary>
    public static class SqlHelper
    {
        /// <summary>连接字符串名称，与 Web.config 中 connectionStrings 节点对应</summary>
        private const string ConnName = "CampusConn";

        /// <summary>
        /// 获取数据库连接字符串。
        /// </summary>
        /// <returns>连接字符串</returns>
        public static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings[ConnName].ConnectionString; }
        }

        /// <summary>
        /// 创建并打开数据库连接。
        /// </summary>
        /// <returns>已打开的 SqlConnection，调用方使用 using 释放</returns>
        public static SqlConnection CreateOpenConnection()
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// 执行增删改语句。
        /// </summary>
        /// <param name="sql">参数化 SQL 语句</param>
        /// <param name="ps">参数数组</param>
        /// <returns>受影响行数</returns>
        public static int ExecuteNonQuery(string sql, params SqlParameter[] ps)
        {
            using (SqlConnection conn = CreateOpenConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                if (ps != null && ps.Length > 0)
                {
                    cmd.Parameters.AddRange(ps);
                }
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 执行查询并返回首行首列。
        /// </summary>
        /// <param name="sql">参数化 SQL 语句</param>
        /// <param name="ps">参数数组</param>
        /// <returns>首行首列的值；无结果返回 null</returns>
        public static object ExecuteScalar(string sql, params SqlParameter[] ps)
        {
            using (SqlConnection conn = CreateOpenConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                if (ps != null && ps.Length > 0)
                {
                    cmd.Parameters.AddRange(ps);
                }
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// 执行查询并返回数据表。
        /// </summary>
        /// <param name="sql">参数化 SQL 语句</param>
        /// <param name="ps">参数数组</param>
        /// <returns>结果数据表</returns>
        public static DataTable ExecuteDataTable(string sql, params SqlParameter[] ps)
        {
            using (SqlConnection conn = CreateOpenConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                if (ps != null && ps.Length > 0)
                {
                    cmd.Parameters.AddRange(ps);
                }
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        /// <summary>
        /// 执行存储过程，并返回指定名称的输出参数值。
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="ps">参数数组，须包含输出参数</param>
        /// <param name="outputName">输出参数名，如 @result</param>
        /// <returns>输出参数的值；执行异常时抛出</returns>
        public static int ExecuteProcedure(string procName, SqlParameter[] ps, string outputName)
        {
            using (SqlConnection conn = CreateOpenConnection())
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (ps != null && ps.Length > 0)
                {
                    cmd.Parameters.AddRange(ps);
                }
                cmd.ExecuteNonQuery();
                object val = cmd.Parameters[outputName].Value;
                return val == null || val == DBNull.Value ? -99 : Convert.ToInt32(val);
            }
        }

        /// <summary>
        /// 构造存储过程输出参数。
        /// </summary>
        /// <param name="name">参数名</param>
        /// <returns>输出方向、整型的参数对象</returns>
        public static SqlParameter CreateOutputParameter(string name)
        {
            return new SqlParameter(name, SqlDbType.Int) { Direction = ParameterDirection.Output };
        }

        /// <summary>
        /// 安全读取可空字符串列。
        /// </summary>
        /// <param name="row">数据行</param>
        /// <param name="column">列名</param>
        /// <returns>字符串值，DBNull 转为空串</returns>
        public static string GetString(DataRow row, string column)
        {
            return row[column] == DBNull.Value ? string.Empty : row[column].ToString();
        }
    }
}
