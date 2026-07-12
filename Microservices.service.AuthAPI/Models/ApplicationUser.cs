using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Microservices.service.AuthAPI.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(100)]
    public string Name { get; set; } = null!;
}
