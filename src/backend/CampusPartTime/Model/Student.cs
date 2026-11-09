using System;

namespace CampusPartTime.Model
{
    /// <summary>
    /// 学生实体，对应数据库表 student（13 个字段）。
    /// </summary>
    public class Student
    {
        /// <summary>学生编号（主键，自增）</summary>
        public int StudentId { get; set; }
        /// <summary>学号（唯一）</summary>
        public string StuNo { get; set; }
        /// <summary>学生姓名</summary>
        public string StuName { get; set; }
        /// <summary>性别（男 / 女）</summary>
        public string StuSex { get; set; }
        /// <summary>出生日期</summary>
        public DateTime? StuBirth { get; set; }
        /// <summary>学院</summary>
        public string StuCollege { get; set; }
        /// <summary>专业</summary>
        public string StuMajor { get; set; }
        /// <summary>年级</summary>
        public string StuGrade { get; set; }
        /// <summary>联系电话（11 位手机号）</summary>
        public string StuPhone { get; set; }
        /// <summary>邮箱</summary>
        public string StuEmail { get; set; }
        /// <summary>密码（BCrypt 哈希）</summary>
        public string StuPassword { get; set; }
        /// <summary>注册日期</summary>
        public DateTime RegDate { get; set; }
        /// <summary>状态（0 正常 / 1 禁用）</summary>
        public byte StuStatus { get; set; }
    }
}
