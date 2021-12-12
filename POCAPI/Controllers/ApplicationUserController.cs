using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using POCModel.Security;
using POCModel.UserInfo;
using POCRepository.Extensions;
using POCRepository.Repository;
using POCService.Contexts;

namespace POCAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        private UnitOfWork<DedicatedDBContext> _dedicatedUnitOfWork { get; set; }
        private UnitOfWork<MasterDBContext> _masterUnitOfWork { get; set; }
        private UnitOfWork<SharedDBContext> _sharedUnitOfWork { get; set; }

        private GenericRepository<ApplicationUser, MasterDBContext> _masterApplicationUserRepo;

        private GenericRepository<FavoritePL, DedicatedDBContext> _dedicatedPLRepo;
        private GenericRepository<FavoritePL, MasterDBContext> _masterPLRepo;
        private GenericRepository<FavoritePL, SharedDBContext> _sharedPLRepo;

        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        public ApplicationUserController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
           , IHttpContextAccessor accessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;

            _dedicatedUnitOfWork = new UnitOfWork<DedicatedDBContext>(accessor.HttpContext);
            _masterUnitOfWork = new UnitOfWork<MasterDBContext>(accessor.HttpContext);
            _sharedUnitOfWork = new UnitOfWork<SharedDBContext>(accessor.HttpContext);

            _masterApplicationUserRepo = new GenericRepository<ApplicationUser, MasterDBContext>(_masterUnitOfWork);

            _dedicatedPLRepo = new GenericRepository<FavoritePL, DedicatedDBContext>(_dedicatedUnitOfWork);

            _masterPLRepo = new GenericRepository<FavoritePL, MasterDBContext>(_masterUnitOfWork);

            _sharedPLRepo = new GenericRepository<FavoritePL, SharedDBContext>(_sharedUnitOfWork);

        }

        [HttpPost]
        [Route("Register")]
        //POST : /api/ApplicationUser/Register
        public async Task<object> PostApplicationUserAsync(VMRegister model)
        {
            try
            {

                ApplicationUser applicationUserExists = null; // 

                applicationUserExists = await _userManager.FindByEmailAsync(model.Email);

                if (applicationUserExists != null)
                {

                    return Ok(new { Msg = $"User {model.Email} already exists. Did you forget your password?", StatusCode = -1});

                }
                else
                {

                    var applicationUserNew = new ApplicationUser();
                    CommonExtensions.CopyPropertiesTo(model, applicationUserNew);
                    // applicationUserNew.CreatedByIP = ip;
                    var result = await _userManager.CreateAsync(applicationUserNew, model.Password);
                    //await _userManager.AddToRoleAsync(applicationUserNew, role);

                    if (result.Succeeded)
                    {
                        return Ok(new { Msg = $"User was created successfully.", StatusCode=0 });
                    }
                    else
                    {
                        return Problem($"Failed to create user {model.Email}. Error: {result}");
                    }

                    
                }



            }
            catch (Exception ex)
            {
                return Problem($"Server error occured.");
            }

        }
    }
}
