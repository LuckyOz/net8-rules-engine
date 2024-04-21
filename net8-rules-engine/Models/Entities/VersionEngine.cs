
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net8_rules_engine.Models.Entities
{
    [Table("version_engine")]
    public class VersionEngine
    {
        [Key]
        [Column("id", Order = 0)]
        public Guid Id { get; set; }

        [Column("created_date", Order = 1)]
        public DateTime CreatedDate { get; set; }

        public List<VersionEngineDetail>? Details { get; set; }
    }

    [Table("version_engine_detail")]
    public class VersionEngineDetail
    {
        [Key]
        [Column("id", Order = 0)]
        public Guid Id { get; set; }

        [ForeignKey("version_engine")]
        [Column("version_engine_id", Order = 1)]
        public Guid? VersionEngineId { get; set; }

        [Column("engine_code", Order = 2)]
        public string? EngineCode { get; set; }
    }
}
