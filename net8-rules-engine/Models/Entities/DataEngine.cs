
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net8_rules_engine.Models.Entities
{
    [Table("data_engine")]
    public class DataEngine
    {
        [Key]
        [Column("id", Order = 0)]
        public Guid Id { get; set; }

        [Column("code", Order = 1)]
        public string? Code { get; set; }

        [Column("v_loop", Order = 2)]
        public int Vloop { get; set; }

        [Column("created_date", Order = 3)]
        public DateTime CreatedDate { get; set; }

        [Column("updated_date", Order = 4)]
        public DateTime UpdatedDate { get; set; }
    }
}
