using System.ComponentModel;

namespace CeramicaCanelas.Domain.Enums;
public enum UserRoles {
    [Description("Admin")]
    Admin = 0,
    [Description("Customer")]
    Customer = 1,
    [Description("Viewer")]
    Viewer = 2,
    [Description("Financial")]
    Financial = 3,
    [Description("Almoxarifado")]
    Almoxarifado = 4,

}
