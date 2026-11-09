/* ==========================================================================
   校园兼职招聘管理系统 —— 客户端公共脚本
   作用：表单提交前的本地校验，减少无效回发、提升交互体验。
   说明：服务端仍对全部输入做二次校验（见各页面的 .aspx.cs 与 BLL 层），
          客户端校验仅作体验优化，不作为安全边界。
   控件取值统一走 byId()，兼容 WebForms 命名容器改写的 ClientID。
   ========================================================================== */

/* ------------------------------ 1. 基础工具 ------------------------------ */

/**
 * 按控件 ID 取元素。
 * WebForms 渲染后 ID 可能被加上命名容器前缀（如 form1_txtUser），
 * 因此依次按 id、name（等于 UniqueID）、id 后缀三种方式匹配。
 * @param {string} id 控件 ID
 * @returns {HTMLElement|null}
 */
function byId(id) {
    var el = document.getElementById(id);
    if (el) {
        return el;
    }

    var nodes = document.getElementsByTagName("*");
    for (var i = 0; i < nodes.length; i++) {
        var node = nodes[i];
        if (node.name === id) {
            return node;
        }
        if (node.id && node.id.length > id.length &&
            node.id.substr(node.id.length - id.length - 1) === "_" + id) {
            return node;
        }
    }
    return null;
}

/**
 * 取控件值并去除首尾空格；控件不存在时返回空串。
 * @param {string} id 控件 ID
 * @returns {string}
 */
function val(id) {
    var el = byId(id);
    if (!el) {
        return "";
    }
    return (el.value === null || el.value === undefined) ? "" : String(el.value).replace(/^\s+|\s+$/g, "");
}

/**
 * 统计文本长度（按字符计，兼容中文）。
 * @param {string} text 文本
 * @returns {number}
 */
function len(text) {
    return text ? text.length : 0;
}

/**
 * 提示并聚焦到出错控件。
 * @param {string} message 提示文本
 * @param {string} id 出错的控件 ID，可为空
 * @returns {boolean} 恒返回 false，便于直接 return
 */
function fail(message, id) {
    alert(message);
    var el = id ? byId(id) : null;
    if (el && el.focus) {
        el.focus();
    }
    return false;
}

/**
 * 是否只含数字。
 * @param {string} text 文本
 * @returns {boolean}
 */
function isDigits(text) {
    return /^\d+$/.test(text);
}

/**
 * 是否合法金额/数字（可含小数）。
 * @param {string} text 文本
 * @returns {boolean}
 */
function isDecimal(text) {
    return /^\d+(\.\d{1,2})?$/.test(text);
}

/**
 * 是否 11 位手机号（1 开头，第二位为 3~9）。
 * @param {string} text 文本
 * @returns {boolean}
 */
function isMobile(text) {
    return /^1[3-9]\d{9}$/.test(text);
}

/**
 * 是否合法邮箱；允许为空（为空返回 true，由调用方决定是否必填）。
 * @param {string} text 文本
 * @returns {boolean}
 */
function isEmail(text) {
    if (text === "") {
        return true;
    }
    return /^[\w.\-]+@[\w\-]+(\.[\w\-]+)+$/.test(text);
}

/* ------------------------------ 2. 登录 ------------------------------ */

/**
 * 登录表单校验：用户名、密码非空，密码不少于 6 位。
 * @returns {boolean} 校验通过返回 true
 */
function checkLoginForm() {
    var user = val("txtUser");
    var pwd = val("txtPwd");

    if (user === "") {
        return fail("请输入登录账号。", "txtUser");
    }
    if (len(user) > 50) {
        return fail("登录账号长度不能超过 50 个字符。", "txtUser");
    }
    if (pwd === "") {
        return fail("请输入登录密码。", "txtPwd");
    }
    if (len(pwd) < 6) {
        return fail("密码长度不能少于 6 位。", "txtPwd");
    }
    return true;
}

/* ------------------------------ 3. 学生注册 ------------------------------ */

/**
 * 学生注册表单校验：学号、姓名、学院、专业、年级、手机、密码必填，
 * 手机号 11 位、邮箱格式、两次密码一致。
 * @returns {boolean} 校验通过返回 true
 */
function checkStudentRegisterForm() {
    var stuNo = val("txtStuNo");
    var stuName = val("txtStuName");
    var college = val("txtCollege");
    var major = val("txtMajor");
    var grade = val("txtGrade");
    var phone = val("txtPhone");
    var email = val("txtEmail");
    var pwd = val("txtPwd");
    var pwd2 = val("txtPwd2");

    if (stuNo === "") {
        return fail("请输入学号。", "txtStuNo");
    }
    if (!isDigits(stuNo) || len(stuNo) < 6 || len(stuNo) > 20) {
        return fail("学号应为 6～20 位数字。", "txtStuNo");
    }
    if (stuName === "") {
        return fail("请输入姓名。", "txtStuName");
    }
    if (len(stuName) > 20) {
        return fail("姓名长度不能超过 20 个字符。", "txtStuName");
    }
    if (college === "") {
        return fail("请输入所在学院。", "txtCollege");
    }
    if (major === "") {
        return fail("请输入所学专业。", "txtMajor");
    }
    if (grade === "") {
        return fail("请输入年级。", "txtGrade");
    }
    if (phone === "") {
        return fail("请输入手机号。", "txtPhone");
    }
    if (!isMobile(phone)) {
        return fail("手机号格式不正确，应为 11 位数字且以 1 开头。", "txtPhone");
    }
    if (!isEmail(email)) {
        return fail("邮箱格式不正确，例如 name@example.com。", "txtEmail");
    }
    if (pwd === "") {
        return fail("请输入密码。", "txtPwd");
    }
    if (len(pwd) < 6) {
        return fail("密码长度不能少于 6 位。", "txtPwd");
    }
    if (pwd !== pwd2) {
        return fail("两次输入的密码不一致，请重新输入。", "txtPwd2");
    }
    return true;
}

/* ------------------------------ 4. 商家注册 ------------------------------ */

/**
 * 商家注册表单校验：商家名称、联系人、手机、地址、密码必填，
 * 手机号 11 位、邮箱格式、两次密码一致、须勾选协议。
 * @returns {boolean} 校验通过返回 true
 */
function checkEmployerRegisterForm() {
    var empName = val("txtEmpName");
    var contact = val("txtContact");
    var phone = val("txtPhone");
    var address = val("txtAddress");
    var email = val("txtEmail");
    var pwd = val("txtPwd");
    var pwd2 = val("txtPwd2");

    if (empName === "") {
        return fail("请输入商家名称。", "txtEmpName");
    }
    if (len(empName) > 50) {
        return fail("商家名称长度不能超过 50 个字符。", "txtEmpName");
    }
    if (contact === "") {
        return fail("请输入联系人。", "txtContact");
    }
    if (phone === "") {
        return fail("请输入联系电话。", "txtPhone");
    }
    if (!isMobile(phone)) {
        return fail("联系电话格式不正确，应为 11 位手机号。", "txtPhone");
    }
    if (address === "") {
        return fail("请输入商家地址。", "txtAddress");
    }
    if (!isEmail(email)) {
        return fail("邮箱格式不正确，例如 name@example.com。", "txtEmail");
    }
    if (pwd === "") {
        return fail("请输入密码。", "txtPwd");
    }
    if (len(pwd) < 6) {
        return fail("密码长度不能少于 6 位。", "txtPwd");
    }
    if (pwd !== pwd2) {
        return fail("两次输入的密码不一致，请重新输入。", "txtPwd2");
    }

    var agree = byId("chkAgree");
    if (agree && !agree.checked) {
        return fail("请先阅读并同意《校园兼职招聘平台服务协议》。", "chkAgree");
    }
    return true;
}

/* ------------------------------ 5. 岗位发布 ------------------------------ */

/**
 * 岗位发布 / 修改表单校验：岗位名称、类型、薪资、地点、时段、人数、截止日期必填；
 * 薪资为正数、招聘人数为 1～100 的整数、截止日期不早于今天。
 * @returns {boolean} 校验通过返回 true
 */
function checkJobForm() {
    var jobName = val("txtJobName");
    var jobType = val("ddlJobType");
    var salary = val("txtSalary");
    var workPlace = val("txtWorkPlace");
    var workTime = val("txtWorkTime");
    var recruitNum = val("txtRecruitNum");
    var deadline = val("txtDeadline");
    var desc = val("txtJobDesc");

    if (jobName === "") {
        return fail("请输入岗位名称。", "txtJobName");
    }
    if (len(jobName) > 50) {
        return fail("岗位名称长度不能超过 50 个字符。", "txtJobName");
    }
    if (jobType === "") {
        return fail("请选择岗位类型。", "ddlJobType");
    }
    if (salary === "") {
        return fail("请输入薪资待遇。", "txtSalary");
    }
    if (!isDecimal(salary) || parseFloat(salary) <= 0) {
        return fail("薪资应为大于 0 的数字，最多保留两位小数。", "txtSalary");
    }
    if (workPlace === "") {
        return fail("请输入工作地点。", "txtWorkPlace");
    }
    if (workTime === "") {
        return fail("请输入工作时段。", "txtWorkTime");
    }
    if (recruitNum === "") {
        return fail("请输入招聘人数。", "txtRecruitNum");
    }
    if (!isDigits(recruitNum) || parseInt(recruitNum, 10) < 1 || parseInt(recruitNum, 10) > 100) {
        return fail("招聘人数应为 1～100 之间的整数。", "txtRecruitNum");
    }
    if (deadline === "") {
        return fail("请选择报名截止日期。", "txtDeadline");
    }
    if (!isValidDate(deadline)) {
        return fail("报名截止日期格式不正确，应为 yyyy-MM-dd。", "txtDeadline");
    }
    if (isBeforeToday(deadline)) {
        return fail("报名截止日期不能早于今天。", "txtDeadline");
    }
    if (desc === "") {
        return fail("请输入岗位描述。", "txtJobDesc");
    }
    if (len(desc) > 1000) {
        return fail("岗位描述不能超过 1000 个字符。", "txtJobDesc");
    }
    return true;
}

/**
 * 判断日期文本是否形如 yyyy-MM-dd 且为有效日期。
 * @param {string} text 日期文本
 * @returns {boolean}
 */
function isValidDate(text) {
    if (!/^\d{4}-\d{2}-\d{2}$/.test(text)) {
        return false;
    }
    var parts = text.split("-");
    var year = parseInt(parts[0], 10);
    var month = parseInt(parts[1], 10);
    var day = parseInt(parts[2], 10);
    if (month < 1 || month > 12 || day < 1 || day > 31) {
        return false;
    }
    var d = new Date(year, month - 1, day);
    return d.getFullYear() === year && d.getMonth() === month - 1 && d.getDate() === day;
}

/**
 * 判断日期是否早于今天（忽略时分秒）。
 * @param {string} text yyyy-MM-dd 文本
 * @returns {boolean}
 */
function isBeforeToday(text) {
    var parts = text.split("-");
    var target = new Date(parseInt(parts[0], 10), parseInt(parts[1], 10) - 1, parseInt(parts[2], 10));
    var now = new Date();
    var today = new Date(now.getFullYear(), now.getMonth(), now.getDate());
    return target.getTime() < today.getTime();
}

/* ------------------------------ 6. 岗位检索 ------------------------------ */

/**
 * 检索条最低薪资校验：允许为空，非空时必须是数字。
 * @returns {boolean} 校验通过返回 true
 */
function checkSalaryInput() {
    var salary = val("txtSalary");
    if (salary === "") {
        return true;
    }
    if (!isDecimal(salary) || parseFloat(salary) < 0) {
        return fail("最低薪资应填写不小于 0 的数字。", "txtSalary");
    }
    return true;
}

/* ------------------------------ 7. 评价提交 ------------------------------ */

/**
 * 评价表单校验：必须选择 1～5 星评分，评价内容 5～500 字。
 * @returns {boolean} 校验通过返回 true
 */
function checkCommentForm() {
    var score = "";
    var radios = document.getElementsByName ? document.getElementsByName("rblScore") : [];
    for (var i = 0; i < radios.length; i++) {
        if (radios[i].checked) {
            score = radios[i].value;
            break;
        }
    }
    if (score === "") {
        var list = byId("rblScore");
        if (list && list.value) {
            score = list.value;
        }
    }
    if (score === "") {
        return fail("请选择 1～5 星评分。", "rblScore");
    }

    var content = val("txtContent");
    if (content === "") {
        return fail("请填写评价内容。", "txtContent");
    }
    if (len(content) < 5) {
        return fail("评价内容至少 5 个字符。", "txtContent");
    }
    if (len(content) > 500) {
        return fail("评价内容不能超过 500 个字符。", "txtContent");
    }
    return true;
}
