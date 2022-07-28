using System.ComponentModel.DataAnnotations;

namespace URLShortener.Url;

public class GetUrlDto
{
    [Required]
    public string OriginalUrl { get; set; }
}