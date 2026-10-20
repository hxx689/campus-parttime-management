// 登录页前端校验
function validateLogin() {
    var username = document.getElementById('username').value.trim();
    var password = document.getElementById('password').value;
    var msg = document.getElementById('msg');

    if (username === '') {
        msg.textContent = '用户名不能为空';
        return false;
    }
    if (password === '') {
        msg.textContent = '密码不能为空';
        return false;
    }
    if (password.length < 6) {
        msg.textContent = '密码长度不能少于 6 位';
        return false;
    }

    msg.textContent = '';
    return true;
}
