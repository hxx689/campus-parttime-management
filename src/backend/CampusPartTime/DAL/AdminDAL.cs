using System;
using System.Data;
using System.Data.SqlClient;
using CampusPartTime.Model;

namespace CampusPartTime.DAL
{
    /// <summary>
    /// 管理员表数据访问类，对应 dbo.admin。
    /// </summary>
    public class AdminDAL
    {
        /// <summary>
        /// 按管理员姓名（登录名）查询。
        /// </summary>
        /// <param name="adminName">管理员姓名</param>
        /// <returns>管理员实体；不存在返回 null</returns>
        public Admin GetByName(string adminName)
        {
            const string sql = "SELECT admin_id, admin_name, admin_password FROM dbo.admin WHERE admin_name = @admin_name";
            DataTable dt = SqlHelper.ExecuteDataTable(sql, new SqlParameter("@admin_name", adminName));
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            DataRow row = dt.Rows[0];
            return new Admin
            {
                AdminId = Convert.ToInt32(row["admin_id"]),
                AdminName = SqlHelper.GetString(row, "admin_name"),
                AdminPassword = SqlHelper.GetString(row, "admin_password")
            };
        }

        /// <summary>
        /// 修改管理员密码。
        /// </summary>
        /// <param name="adminId">管理员编号</param>
        /// <param name="passwordHash">新的 BCrypt 哈希</param>
        /// <returns>受影响行数</returns>
        public int UpdatePassword(int adminId, string passwordHash)
        {
            const string sql = "UPDATE dbo.admin SET admin_password = @pwd WHERE admin_id = @admin_id";
            return SqlHelper.ExecuteNonQuery(sql,
                new SqlParameter("@pwd", passwordHash),
                new SqlParameter("@admin_id", adminId));
        }
    }
}
