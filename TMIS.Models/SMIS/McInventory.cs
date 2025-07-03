using System.ComponentModel.DataAnnotations;

namespace TMIS.Models.SMIS
{
    public class McInventory
    {
        public int Id { get; set; }

        public string QrCode { get; set; } = string.Empty;

        [Required]
        public string SerialNo { get; set; } = string.Empty;

        public string? FarCode { get; set; } = string.Empty;
        public bool IsOwned { get; set; }
        public DateTime? DatePurchased { get; set; }
        public DateTime? DateBorrow { get; set; }
        public DateTime? DateDue { get; set; }

        [Required]
        [Range(1, 2000, ErrorMessage = "Please enter valid days")]
        public int ServiceSeq { get; set; }

        public int MachineBrandId { get; set; }

        [Required]
        public int MachineTypeId { get; set; }

        [Required]
        public int LocationId { get; set; }

        [Required]
        public int OwnedUnitId { get; set; }

        [Required]
        public int CurrentUnitId { get; set; }

        public int CurrentStatusId { get; set; }

        [Required]
        public int MachineModelId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please enter valid cost")]
        public decimal Cost { get; set; }

        public byte[]? ImageFR { get; set; }
        public byte[]? ImageBK { get; set; }

        public bool RemoveImageFront { get; set; }
        public bool RemoveImageBack { get; set; }

        public int SupplierId { get; set; }

        public int CostMethodId { get; set; }

        [MaxLength(90, ErrorMessage = "Comment sholud be short.")]
        public string? Comments { get; set; } = string.Empty;

        public string? LastScanDateTime { get; set; } = string.Empty;
    }
}
