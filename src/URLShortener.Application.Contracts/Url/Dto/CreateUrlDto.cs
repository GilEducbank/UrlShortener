using System;
using System.ComponentModel.DataAnnotations;

namespace URLShortener.Url;

public class CreateUrlDto
{
    [Required]
    public string OriginalUrl { get; set; }
    [Required] 
    [StringLength(10)]
    public string ShortenedUrl { get; set; }

    public DateTime ExpireDate { get; set; }
}