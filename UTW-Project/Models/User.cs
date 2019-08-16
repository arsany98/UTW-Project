//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UTW_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;
    using System.ComponentModel.DataAnnotations;

    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.Orders = new HashSet<Order>();
        }
    
        public int ID { get; set; }
        [RequiredAttribute]
        public string Username { get; set; }
        [RequiredAttribute]
        public string Password { get; set; }
        [RequiredAttribute]
        public string Email { get; set; }
        [RequiredAttribute]
        public string FirstNameEN { get; set; }
        [RequiredAttribute]
        public string LastNameEN { get; set; }
        [RequiredAttribute]
        public string FirstNameAR { get; set; }
        [RequiredAttribute]
        public string LastNameAR { get; set; }
        
        public string Phone { get; set; }

        public int Q_ID { get; set; }
        [RequiredAttribute]
        public string Answer { get; set; }
        public bool EmailConfirmed { get; set; }
        public decimal Wallet { get; set; }
        public bool Blocked { get; set; }
        public int LoginTrials { get; set; }
        public bool Admin { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
        public virtual Question Question { get; set; }


        public string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }
    }
}
