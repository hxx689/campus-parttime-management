using System;
using System.Configuration;
using CampusPartTime.DAL;
using CampusPartTime.Model;

namespace CampusPartTime.BLL
{
    /// <summary>
    /// 用户业务逻辑类：负责学生与商家的注册、登录校验与密码哈希。
    /// 密码一律使用 BCrypt 加盐哈希存储，工作因子统一取 10（对应 2^10 次迭代），
    /// 库中不保存任何明文口令。
    /// </summary>
    public class UserBLL
    {
        private readonly StudentDAL _studentDal = new StudentDAL();
        private readonly EmployerDAL _employerDal = new EmployerDAL();
        private readonly AdminDAL _adminDal = new AdminDAL();

        /// <summary>密码哈希工作因子，默认 10</summary>
        private static int WorkFactor
        {
            get
            {
                string cfg = ConfigurationManager.AppSettings["BCryptWorkFactor"];
                int factor;
                return int.TryParse(cfg, out factor) ? factor : 10;
            }
        }

        /// <summary>
        /// 学生注册。
        /// </summary>
        /// <param name="m">学生实体，StuPassword 传入明文口令</param>
        /// <param name="message">输出参数：成功或失败的提示文本</param>
        /// <returns>注册成功返回 true</returns>
        public bool RegisterStudent(Student m, out string message)
        {
            if (string.IsNullOrWhiteSpace(m.StuNo) || string.IsNullOrWhiteSpace(m.StuName))
            {
                message = "学号与姓名不能为空。";
                return false;
            }
            if (_studentDal.ExistsNoOrPhone(m.StuNo, m.StuPhone))
            {
                message = "该学号或联系电话已被注册。";
                return false;
            }
            // 明文口令在此处一次性转为 BCrypt 哈希，调用之后不再保留原文
            m.StuPassword = BCrypt.Net.BCrypt.HashPassword(m.StuPassword, WorkFactor);
            int id = _studentDal.Insert(m);
            if (id <= 0)
            {
                message = "注册失败，请稍后重试。";
                return false;
            }
            message = "注册成功，请使用学号登录。";
            return true;
        }

        /// <summary>
        /// 商家注册。
        /// </summary>
        /// <param name="m">商家实体，EmpPassword 传入明文口令</param>
        /// <param name="message">输出参数：成功或失败的提示文本</param>
        /// <returns>注册成功返回 true</returns>
        public bool RegisterEmployer(Employer m, out string message)
        {
            if (string.IsNullOrWhiteSpace(m.EmpName) || string.IsNullOrWhiteSpace(m.EmpPhone))
            {
                message = "商家名称与联系电话不能为空。";
                return false;
            }
            if (_employerDal.ExistsPhone(m.EmpPhone))
            {
                message = "该联系电话已被注册。";
                return false;
            }
            m.EmpPassword = BCrypt.Net.BCrypt.HashPassword(m.EmpPassword, WorkFactor);
            int id = _employerDal.Insert(m);
            if (id <= 0)
            {
                message = "注册失败，请稍后重试。";
                return false;
            }
            message = "注册成功，请使用联系电话登录。";
            return true;
        }

        /// <summary>
        /// 统一登录校验。
        /// </summary>
        /// <param name="role">角色标识：student / employer / admin</param>
        /// <param name="account">登录账号：学号 / 联系电话 / 管理员姓名</param>
        /// <param name="password">明文口令</param>
        /// <param name="userId">输出参数：登录成功时的用户编号</param>
        /// <param name="displayName">输出参数：登录成功时的显示名称</param>
        /// <param name="message">输出参数：失败原因</param>
        /// <returns>登录成功返回 true</returns>
        public bool Login(string role, string account, string password, out int userId, out string displayName, out string message)
        {
            userId = 0;
            displayName = string.Empty;
            message = string.Empty;

            if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(password))
            {
                message = "账号与密码不能为空。";
                return false;
            }

            switch (role)
            {
                case "student":
                {
                    Student stu = _studentDal.GetByNo(account.Trim());
                    if (stu == null || !BCrypt.Net.BCrypt.Verify(password, stu.StuPassword))
                    {
                        message = "账号或密码错误。";
                        return false;
                    }
                    if (stu.StuStatus == 1)
                    {
                        message = "该账号已被管理员禁用，请联系管理员。";
                        return false;
                    }
                    userId = stu.StudentId;
                    displayName = stu.StuName;
                    return true;
                }
                case "employer":
                {
                    Employer emp = _employerDal.GetByPhone(account.Trim());
                    if (emp == null || !BCrypt.Net.BCrypt.Verify(password, emp.EmpPassword))
                    {
                        message = "账号或密码错误。";
                        return false;
                    }
                    if (emp.EmpStatus == 1)
                    {
                        message = "该账号已被管理员禁用，请联系管理员。";
                        return false;
                    }
                    userId = emp.EmployerId;
                    displayName = emp.EmpName;
                    return true;
                }
                case "admin":
                {
                    Admin admin = _adminDal.GetByName(account.Trim());
                    if (admin == null || !BCrypt.Net.BCrypt.Verify(password, admin.AdminPassword))
                    {
                        message = "账号或密码错误。";
                        return false;
                    }
                    userId = admin.AdminId;
                    displayName = admin.AdminName;
                    return true;
                }
                default:
                    message = "请选择用户类型。";
                    return false;
            }
        }
    }
}
