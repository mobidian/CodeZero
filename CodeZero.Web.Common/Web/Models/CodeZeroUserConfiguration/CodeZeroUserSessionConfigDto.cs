//  <copyright file="CodeZeroUserSessionConfigDto.cs" project="CodeZero.Web.Common" solution="CodeZero">
//      Copyright (c) 2018 CodeZero Framework.  All rights reserved.
//  </copyright>
//  <author>Nasr Aldin M.</author>
//  <email>nasr2ldin@gmail.com</email>
//  <website>https://nasraldin.com/codezero</website>
//  <github>https://nasraldin.github.io/CodeZero</github>
//  <date>01/01/2018 01:00 AM</date>
using CodeZero.MultiTenancy;

namespace CodeZero.Web.Models.CodeZeroUserConfiguration
{
    public class CodeZeroUserSessionConfigDto
    {
        public long? UserId { get; set; }

        public int? TenantId { get; set; }

        public long? ImpersonatorUserId { get; set; }

        public int? ImpersonatorTenantId { get; set; }

        public MultiTenancySides MultiTenancySide { get; set; }
    }
}