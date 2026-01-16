using System;
using System.ComponentModel.DataAnnotations;
using Tasinmaz.Entities.Concrete;

public class Log 
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; } 

    [Required]
    public string OperationType { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public string Status { get; set; }

    [Required]
    public DateTime Timestamp { get; set; }

    [Required]
    public string UserIp { get; set; }
    public User User { get; set; }
}
