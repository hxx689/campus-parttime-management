using System;

namespace CampusPartTime.Model
{
    /// <summary>
    /// 申请记录实体，对应数据库表 application（6 个字段）。
    /// 类名使用 ApplicationRecord，避免与 System.Windows.Forms.Application 命名冲突。
    /// </summary>
    public class ApplicationRecord
    {
        /// <summary>申请编号（主键，自增）</summary>
        public int AppId { get; set; }
        /// <summary>学生编号（外键）</summary>
        public int StudentId { get; set; }
        /// <summary>岗位编号（外键）</summary>
        public int JobId { get; set; }
        /// <summary>申请日期</summary>
        public DateTime AppDate { get; set; }
        /// <summary>申请状态（0 待处理 / 1 已录用 / 2 已拒绝）</summary>
        public byte AppStatus { get; set; }
        /// <summary>备注</summary>
        public string AppRemark { get; set; }
    }
}
