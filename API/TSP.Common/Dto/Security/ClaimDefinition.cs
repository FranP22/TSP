using System.ComponentModel.DataAnnotations;

namespace TSP.Common.Dto.Security
{
    public class ClaimDefinition
    {
        [Required]
        public string Permission { get; set; } = string.Empty;
        [Required]
        public bool Enabled { get; set; }
    }
}
