# -*- coding: utf-8 -*-
"""数据库脚本结构校验：核对表、视图、存储过程、触发器、索引与约束是否齐备。
用法：python tests/verify_db_script.py
"""
import os, re, sys

SQL = os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))),
                   'database', 'CampusPartTime.sql')

EXPECT_TABLES = ['student', 'employer', 'admin', 'job', 'application', 'comment']
EXPECT_PROCEDURES = ['proc_apply_job', 'proc_hire_applicant', 'proc_reject_applicant']
EXPECT_TRIGGERS = ['trg_job_after_apply']
EXPECT_VIEWS = ['v_job_detail']
MIN_INDEXES = 10

def check():
    if not os.path.exists(SQL):
        print('[FAIL] 未找到 database/CampusPartTime.sql')
        return 1
    text = open(SQL, encoding='utf-8').read()
    low = text.lower()
    failed = 0

    print('待校验脚本: %s（%d 行）' % (SQL, len(text.split('\n'))))

    for t in EXPECT_TABLES:
        ok = re.search(r'create\s+table\s+(?:\[?dbo\]?\.)?\[?%s\]?' % t, low) is not None
        print(('[PASS] ' if ok else '[FAIL] ') + '数据表 %s' % t)
        failed += 0 if ok else 1

    for v in EXPECT_VIEWS:
        ok = re.search(r'create\s+view\s+(?:\[?dbo\]?\.)?\[?%s\]?' % v, low) is not None
        print(('[PASS] ' if ok else '[FAIL] ') + '视图 %s' % v)
        failed += 0 if ok else 1

    for p in EXPECT_PROCEDURES:
        ok = re.search(r'create\s+proc(?:edure)?\s+(?:\[?dbo\]?\.)?\[?%s\]?' % p, low) is not None
        print(('[PASS] ' if ok else '[FAIL] ') + '存储过程 %s' % p)
        failed += 0 if ok else 1

    for g in EXPECT_TRIGGERS:
        ok = re.search(r'create\s+trigger\s+(?:\[?dbo\]?\.)?\[?%s\]?' % g, low) is not None
        print(('[PASS] ' if ok else '[FAIL] ') + '触发器 %s' % g)
        failed += 0 if ok else 1

    idx_count = len(re.findall(r'create\s+(?:unique\s+)?(?:nonclustered\s+)?index', low))
    ok = idx_count >= MIN_INDEXES
    print(('[PASS] ' if ok else '[FAIL] ') + '索引数量 %d（期望 >= %d）' % (idx_count, MIN_INDEXES))
    failed += 0 if ok else 1

    fk_count = len(re.findall(r'foreign\s+key', low))
    ok = fk_count >= 5
    print(('[PASS] ' if ok else '[FAIL] ') + '外键数量 %d（期望 >= 5）' % fk_count)
    failed += 0 if ok else 1

    print('\n结果: %s' % ('全部通过' if failed == 0 else '%d 项未通过' % failed))
    return 0 if failed == 0 else 1

if __name__ == '__main__':
    sys.exit(check())
