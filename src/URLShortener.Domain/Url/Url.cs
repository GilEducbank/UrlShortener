using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
namespace URLShortener.Url;

public sealed class Url : FullAuditedAggregateRoot<Guid>
{
    [Required]
    public string OriginalUrl { get; private set; }
    [Required]
    public string ShortenedUrl { get; private set; }
    public DateTime ExpireDate { get; private set; }

    private Url()
    {
        
    }
    public Url(Guid id, string originalUrl, string shortenedUrl,DateTime expireDate) : base(id)
    {
        if (originalUrl.IsNullOrEmpty())
            throw new BusinessException("Empty string is not allowed");

        if (!IsValidUrl(originalUrl))
            throw new BusinessException("Url is not valid");
        
        if (expireDate.Date < DateTime.Now.Date)
            throw new BusinessException("Invalid Expiration Date time");
         
        
        if (shortenedUrl.Length > 10)
            throw new BusinessException("Shortened Url length cannot be greater than 10");

        if (HasInvalidCharacter(shortenedUrl))
            throw new BusinessException("Shortened url has invalid characters");
        
        AddLocalEvent(
        
            new UrlCreateEto
            {
                UrlId = Id
            }
        );
        OriginalUrl = originalUrl;
        ShortenedUrl = shortenedUrl;
        ExpireDate = expireDate;
    }

    //Private methods
    private static bool IsValidUrl(string urlString)
    {
        if (HasInvalidCharacter(urlString))
            return false;
            
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
    private static bool HasInvalidCharacter(string shortenedUrl)
    {
        const string validKeys = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890+-$/–_.+!*‘()";
        return shortenedUrl.Any(c => !validKeys.Contains(c));
    }
}