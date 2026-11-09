using System;

namespace CampusPartTime.Model
{
    /// <summary>
    /// 商家实体，对应数据库表 employer（10 个字段）。
    /// </summary>
    public class Employer
    {
        /// <summary>商家编号（主键，自增）</summary>
        public int EmployerId { get; set; }
        /// <summary>商家名称</summary>
        public string EmpName { get; set; }
        /// <summary>联系人</summary>
        public string EmpContact { get; set; }
        /// <summary>联系电话（11 位手机号，唯一）</summary>
        public string EmpPhone { get; set; }
        /// <summary>地址</summary>
        public string EmpAddress { get; set; }
        /// <summary>商家简介（不超过 500 字）</summary>
        public string EmpIntro { get; set; }
        /// <summary>邮箱</summary>
        public string EmpEmail { get; set; }
        /// <summary>密码（BCrypt 哈希）</summary>
        public string EmpPassword { get; set; }
        /// <summary>注册日期</summary>
        public DateTime RegDate { get; set; }
        /// <summary>状态（0 正常 / 1 禁用）</summary>
        public byte EmpStatus { get; set; }
    }
}
