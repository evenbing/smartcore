using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SmartCore.Validator
{
   public class CustomerValidateion:ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime dateTime = Convert.ToDateTime(value);
            return dateTime <= DateTime.UtcNow.ToLocalTime();
        }
    }
    public class Min18Years : ValidationAttribute
    { 
        protected override ValidationResult IsValid(object va,ValidationContext validationContext)
        {
            var student=(StudentsDto)validationContext.ObjectInstance;
            if (student.DateofBirth == null)
            {
                return new ValidationResult("Date of Birth is required.");

            }
            var age = DateTime.Today.Year - student.DateofBirth.Year;
            return (age >= 18) ? ValidationResult.Success : new ValidationResult("Student should be at leasst 18 years old.");
        }
    }
    public class StudentsDto
    {

        [Range(1,9999,ErrorMessage ="排序号必须介于1~9999之间")]
        public int OrderNumber { get; set; }
        [Required(ErrorMessage ="学号不能为空")]
        [StringLength(100,ErrorMessage ="学号不能大于100个字符")]
        public string StudentsNo { get; set; }
        [CustomerValidateion(ErrorMessage ="Admission date must be less than or equal to Today's Date.")]
        public DateTime AdmissionDate { get; set; }
        [DataType(DataType.Date)]
        [Min18Years]
        public DateTime DateofBirth { get; set; }
    }
}
