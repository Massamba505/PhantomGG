using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.Auth;

public class RefreshRequest
{
    public bool PersistCookie { get; set; } = true;
}
