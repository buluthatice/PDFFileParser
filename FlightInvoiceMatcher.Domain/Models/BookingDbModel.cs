using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FlightInvoiceMatcher.Domain.Models;

[Table("Bookings")]
public class BookingDbModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [MaxLength(10)]
    public required string BookingId { get; set; }
    [Required]
    [MaxLength(50)]
    public required string CustomerName { get; set; }
    [Required]
    [MaxLength(10)]
    public required string CarrierCode { get; set; }
    [Required]
    [MaxLength(10)]
    public int FlightNumber { get; set; }
    [Required]
    public DateTime FlightDate { get; set; }
    [Required]
    [MaxLength(10)]
    public required string Origin { get; set; }
    [Required]
    [MaxLength(10)]
    public required string Destination { get; set; }
    [Required]
    public decimal Price { get; set; }
    [MaxLength(10)]
    public string? InvoiceNumber { get; set; }
}