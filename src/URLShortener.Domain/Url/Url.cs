using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
namespace URLShortener.Url;

public class Url : FullAuditedAggregateRoot<Guid>
{
    [Required]
    public string OriginalUrl { get; private set; }
    [Required]
    public string ShortenedUrl { get; private set; }
    public DateTime ExpireDate { get; set; }

    protected Url()
    {
        
    }
    public Url(Guid id, string originalUrl, string shortenedUrl,DateTime expireDate) : base(id)
    {
        if (originalUrl.IsNullOrEmpty())
        {
            throw new BusinessException("Empty string is not allowed");
        }
        
        if (!IsValidUrl((originalUrl)))
        {
            throw new BusinessException("Url is not valid");
        }
        
        OriginalUrl = originalUrl;
        ShortenedUrl = shortenedUrl;
        ExpireDate = expireDate;
    }
    private static bool IsValidUrl(string urlString)
    {
        if(!urlString.StartsWith("https://") && !urlString.StartsWith("http://") && urlString.Contains('.'))
        { 
                urlString = "http://" + urlString;
        }
    
        Uri uri;
        return Uri.TryCreate(urlString, UriKind.Absolute, out uri)
               && (uri.Scheme == Uri.UriSchemeHttp
                   || uri.Scheme == Uri.UriSchemeHttps
                   || uri.Scheme == Uri.UriSchemeFtp
                   || uri.Scheme == Uri.UriSchemeMailto);
    }
}