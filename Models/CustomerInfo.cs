using System.ComponentModel.DataAnnotations;

namespace KuaforRandevu.Models;

public class CustomerInfo
{
    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    [StringLength(60, MinimumLength = 3, ErrorMessage = "Ad soyad 3-60 karakter olmalıdır.")]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [StringLength(100)]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Telefon zorunludur.")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
    [RegularExpression(@"^0?5\d{9}$", ErrorMessage = "Telefon 05XXXXXXXXX biçimde olmalıdır.")]
    public string Phone { get; set; } = "";
}