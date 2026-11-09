using System;

namespace CampusPartTime.Model
{
    /// <summary>
    /// 评价实体，对应数据库表 comment（8 个字段）。
    /// </summary>
    public class Comment
    {
        /// <summary>评价编号（主键，自增）</summary>
        public int CommentId { get; set; }
        /// <summary>学生编号（外键）</summary>
        public int StudentId { get; set; }
        /// <summary>岗位编号（外键）</summary>
        public int JobId { get; set; }
        /// <summary>商家编号（外键），由岗位关联取得</summary>
        public int EmployerId { get; set; }
        /// <summary>评分（1～5 的整数）</summary>
        public int Score { get; set; }
        /// <summary>评价内容</summary>
        public string Content { get; set; }
        /// <summary>评价日期</summary>
        public DateTime CommentDate { get; set; }
        /// <summary>审核状态（0 待审核 / 1 通过 / 2 驳回）</summary>
        public byte AuditStatus { get; set; }
    }
}
