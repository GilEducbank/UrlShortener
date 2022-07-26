using System;
using System.ComponentModel.DataAnnotations;

namespace URLShortener.Url;

public class UrlCreateDto
{
    [Required]
    public string OriginalUrl { get; set; }
    [Required] 
    public string ShortenedUrl { get; set; }

    public DateTime ExpireDate { get; set; }
}