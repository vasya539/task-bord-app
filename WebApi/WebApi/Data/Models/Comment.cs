using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApi.Data.Models
{
    /// <summary>
    /// Comment which can be written to specific item on board
    /// </summary>
    [Table("Comments")]
    public class Comment
    {
        /// <summary>
        /// Comment's id. Primary key
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Id of item which store this comment
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Comment body-text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Id of user which write this comment
        /// </summary>
        public string UserId { get; set; }
        
        /// <summary>
        /// Datetime when comment were written
        /// </summary>
        public DateTime Date { get; set; }

        public Item Item { get; set; }

        public User User { get; set; }
    }
}