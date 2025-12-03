using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Domain.Common.Enums
{
    /// <summary>
    /// Các vai trò của User trong hệ thống.
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// Quản trị viên - full access.
        /// </summary>
        Admin = 1,

        /// <summary>
        /// Giáo viên - quản lý khoá học và điểm
        /// </summary>
        Teacher = 2,
        
        /// <summary>
        /// Sinh viên - Chỉ xem được thông tin của mình
        /// </summary>
        Student = 3,

        /// <summary>
        /// Nhân viên - chức năng quản trị
        /// </summary>
        Staff = 4
    }
}
