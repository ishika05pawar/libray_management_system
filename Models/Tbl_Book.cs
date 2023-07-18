//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace libray_management_system.Models
{
    using System;
    using System.Collections.Generic;
    using System.Web;

    public partial class Tbl_Book
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tbl_Book()
        {
            this.Tbl_BookFine = new HashSet<Tbl_BookFine>();
            this.Tbl_IssueBook = new HashSet<Tbl_IssueBook>();
            this.Tbl_ReturnBook = new HashSet<Tbl_ReturnBook>();
        }
    
        public int bookid { get; set; }
        public string bookname { get; set; }
        public int categoryid { get; set; }
        public int authorid { get; set; }
        public int departmentid { get; set; }
        public int publisherid { get; set; }
        public int publisheryear { get; set; }
        public string edition { get; set; }
        public string bookimae { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public int availablestock { get; set; }
        public int totalstok { get; set; }
        public string isbnnumber { get; set; }
        public int accessionNo { get; set; }
        public string description { get; set; }
        public System.DateTime regdate { get; set; }
    
        public virtual Tbl_Author Tbl_Author { get; set; }
        public virtual Tbl_Category Tbl_Category { get; set; }
        public virtual Tbl_Department Tbl_Department { get; set; }
        public virtual Tbl_Publisher Tbl_Publisher { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_BookFine> Tbl_BookFine { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_IssueBook> Tbl_IssueBook { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tbl_ReturnBook> Tbl_ReturnBook { get; set; }
    }
}
