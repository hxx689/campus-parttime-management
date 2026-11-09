namespace CampusPartTime.Model
{
    /// <summary>
    /// 管理员实体，对应数据库表 admin（3 个字段）。
    /// </summary>
    public class Admin
    {
        /// <summary>管理员编号（主键，自增）</summary>
        public int AdminId { get; set; }
        /// <summary>管理员姓名（登录名，唯一）</summary>
        public string AdminName { get; set; }
        /// <summary>密码（BCrypt 哈希）</summary>
        public string AdminPassword { get; set; }
    }
}
