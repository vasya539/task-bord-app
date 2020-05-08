using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Data.Models
{
    /// <summary>
    /// Relation which couple two items
    /// </summary>
    [Table("ItemsRelations")]
    public class ItemRelation
    {
        /// <summary>
        /// Id of first item in relation
        /// </summary>
        public int FirstItemId { get; set; }
        
        /// <summary>
        /// Id of second item in relation
        /// </summary>
        public int SecondItemId { get; set; }

        public virtual Item FirstItem { get; set; }
        public virtual Item SecondItem { get; set; }
    }
}