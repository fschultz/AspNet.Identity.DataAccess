AspNet.Identity.DataAccess
==========================

ASP.NET Identity provider for Telerik Data Access.

##Features##

This library tries to match the functionality of the Entity Framework ASP.NET Identity provider.

###UserStore implementation###
The **UserStore** implements **_IUserStore_, _IUserRoleStore_, _IUserClaimStore_, _IUserPasswordStore_, _IUserSecurityStampStore_, _IUserEmailStore_, _IUserPhoneNumberStore_, _IUserTwoFactorStore_, _IUserLockoutStore_, _IUserLoginStore_** and **_IQueryableUserStore_**

###RoleStore implementation###
The **RoleStore** implements **_IRoleStore_** and **_IQueryableRoleStore_**.

##Example##

This example creates a role if it not already exists and assigns a user to the role:
```
    var roleManager = new RoleManager<IdentityRole, Guid>(new RoleStore());

    var roleName = "MyGroup";

    var role = roleManager.FindByName(roleName);

    if (role == null) {
        role = new IdentityRole(roleName);
        roleManager.Create(role);
    }

    var userManager = new UserManager<IdentityUser, Guid>(new UserStore());

    var user = userManager.FindByName("MyUser");

    if (user != null) {
        userManager.AddToRole(user.Id, roleName);
    }
```

If you wish to use this implementation together with Owin check out how to migrate from the Entity Framework implemention in the ASP.NET web application template here: http://kaliko.com/blog/aspnet-template-for-data-access-identity/

## Requirements ##

* Microsoft ASP.NET Indentity Core 2.2.0 or later
* Telerik Data Access Core
