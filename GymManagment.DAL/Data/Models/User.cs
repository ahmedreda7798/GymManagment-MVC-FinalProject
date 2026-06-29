using GymManagment.DAL.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.DAL.Data.Models;
public abstract class User : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public DateOnly DateOfBirth { get; set; }
    //Gender [Male,Female]
    public Gender Gender { get; set; }
    //Address [Building Number, City, Street]
    public Address Address { get; set; } = default!;

}
[Owned]
public class Address
{ 
    public string Street { get; set; } = default!;
    public string City { get; set; } = default!;
    public int BuildingNumber { get; set; } = default!;
}
