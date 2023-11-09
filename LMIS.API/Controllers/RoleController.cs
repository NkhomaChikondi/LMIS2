using AutoMapper;
using LMIS.DataStore.Core.DTOs.Role;
using LMIS.DataStore.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace LMIS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private IRoleRepository _roleRepository;
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public RoleController(IUnitOfWork unitOfWork, IRoleRepository roleRepository, IMapper mapper)
        {

            this._unitOfWork = unitOfWork;
            this._roleRepository = roleRepository;
            this._mapper = mapper;

        }
        // GET: api/<RoleController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadRoleDTO>>> Get()
        {
           //get all roles available in the system

            List<string> roles = new List<string>();

            var dbRoles = await this._roleRepository.GetRolesAsync();

            //map the result

            this._mapper.Map<List<ReadRoleDTO>>(dbRoles);

            return Ok(dbRoles);

            
        }

        // GET api/<RoleController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadRoleDTO>> Get(string id)
        {
            //get all roles available in the system

            var dbRole = await this._roleRepository.GetRoleByIdAsync(id);

            //map the result

            this._mapper.Map<ReadRoleDTO>(dbRole);

            return Ok(dbRole);
        }

        // POST api/<RoleController>
        [HttpPost]
        public async Task<ActionResult> Post(CreateRoleDTO createRoleDTO)
        {
            //check if the data sent is valid

            if (ModelState.IsValid)
            {
                //check it the record already exist

                var mappedRecord = new IdentityRole() { Name = createRoleDTO.Name };

                if(await this._roleRepository.Exists(createRoleDTO.Name) != true)
                {
                    //save the role to the data store

                    await this._roleRepository.AddRole(mappedRecord);

                    //sync changes to the data store

                    await this._unitOfWork.SaveToStoreAsync();

                    string location = Url.Action(nameof(Post));

                    return Created(location, this._mapper.Map<ReadRoleDTO>(mappedRecord));
                }
                else
                {
                    ModelState.AddModelError(nameof(createRoleDTO.Name), $"This Role {createRoleDTO.Name} already exists in the system");

                    return BadRequest(ModelState);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        // PUT api/<RoleController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(string id, [FromBody] string name)
        {
            //find the record having the id sent by the user

            IdentityRole? role = await this._roleRepository.GetRoleByIdAsync(id);

            if(role is not null)
            {

                role.Name = name.Trim();
                role.NormalizedName = name.Trim().ToUpper();

                //sync changes

                await this._unitOfWork.SaveToStoreAsync();
                
                //return mapped result
                return Ok(this._mapper.Map<ReadRoleDTO>(role));
            }
            else
            {
                return BadRequest("record not found");
            }
        }

        // DELETE api/<RoleController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            //find a role given the id

          
            IdentityResult result = await  this._roleRepository.remove(id);

            if(result.Succeeded)
            {
                return Ok();
            }


           return BadRequest("Record not found");
            
        }
    }
}
