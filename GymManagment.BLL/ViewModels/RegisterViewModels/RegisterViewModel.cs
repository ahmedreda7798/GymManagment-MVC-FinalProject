using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.ViewModels.RegisterViewModels;
public class RegisterViewModel
{


    [Required(ErrorMessage = "First Name Is Required")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces")]
    [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = null!;



    [Required(ErrorMessage = "Last Name Is Required")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces")]
    [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = null!;



    [Required(ErrorMessage = "User Name Is Required")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$",
    ErrorMessage = "Username can only contain letters, numbers, and underscores")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    public string UserName { get; set; } = null!;



    [Required(ErrorMessage = "Email Is Required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;



    [Required(ErrorMessage = "Phone Number Is Required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Phone number must be a valid Egyptian mobile number")]
    [DataType(DataType.PhoneNumber)]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = null!;



    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Confirm Password is required")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = null!;

}
