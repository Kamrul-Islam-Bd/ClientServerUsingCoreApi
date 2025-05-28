using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Server.Models;
using Server.Models.DTOs;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _web;

        public EmployeesController(AppDbContext db, IWebHostEnvironment web)
        {
            _db = db;
            _web = web;
        }
        [HttpGet]
        public IActionResult GetEmployees()
        {
            List<Employee> employees = _db.Employees.Include(e => e.Experiences).ToList();
            string jsonString = JsonConvert.SerializeObject(employees, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling=ReferenceLoopHandling.Ignore,
            });
            return Content(jsonString,"application/json");
        }

        [HttpGet("{id}")]
        public IActionResult GetEmployee(int id)
        {
            Employee employee = _db.Employees.Include(e => e.Experiences).SingleOrDefault(e=>e.EmployeeId==id);
            if (id==null)
            {
                return NotFound();
            }
            string jsonString = JsonConvert.SerializeObject(employee, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });
            return Content(jsonString, "application/json");


        }
        [HttpPost]
        public async Task<IActionResult>  PostEmployee([FromForm]Common objCommon)
        {
            ImageUpload fileApi = new ImageUpload();
            string fileName = objCommon.ImageName+".png";
            fileApi.ImgName = "\\images\\"+fileName;
            if (objCommon.ImageFile?.Length > 0)
            {
                if (!Directory.Exists(_web.WebRootPath + "\\images"))
                {
                    Directory.CreateDirectory(_web.WebRootPath + "\\images\\");
                }
                string filePath=_web.WebRootPath + "\\images\\" + fileName;
                using (FileStream stream=System.IO.File.Create(filePath))
                {
                    objCommon.ImageFile.CopyTo(stream);
                    stream.Flush();
                } 
                fileApi.ImgName = "/images/" + fileName;
            }
            Employee empObj=new Employee();
            empObj.EmployeeName = objCommon.EmployeeName;
            empObj.IsActive = objCommon.IsActive;
            empObj.JoinDate = objCommon.JoinDate;
            empObj.ImageName = fileApi.ImgName;
            empObj.ImageUrl = fileApi.ImgName;


            List<Experience> expList = JsonConvert.DeserializeObject<List<Experience>>(objCommon.Experiences!);
            empObj.Experiences = expList;
          _db.Employees.Add(empObj);
          await  _db.SaveChangesAsync();

            return Ok(empObj);
        }

        [HttpPut()]
        public async Task<IActionResult> PutEmployee(int id,[FromForm] Common objCommon)
        {
            var empObj = await _db.Employees.FindAsync(id);
            if (empObj == null) return NotFound("Employee not found");
            ImageUpload fileApi = new ImageUpload();
           
            if (objCommon.ImageFile?.Length > 0)
            {
                string fileName = objCommon.ImageName + ".png";
                fileApi.ImgName = "\\images\\" + fileName;
                if (!Directory.Exists(_web.WebRootPath + "\\images"))
                {
                    Directory.CreateDirectory(_web.WebRootPath + "\\images\\");
                }
                string filePath = _web.WebRootPath + "\\images\\" + fileName;
                using (FileStream stream = System.IO.File.Create(filePath))
                {
                    objCommon.ImageFile.CopyTo(stream);
                    stream.Flush();
                }
                fileApi.ImgName = "/images/" + fileName;
            }
            else
            {
                fileApi.ImgName = objCommon.ImageName;
            }
            empObj.EmployeeName = objCommon.EmployeeName;
            empObj.IsActive = objCommon.IsActive;
            empObj.JoinDate = objCommon.JoinDate;
            empObj.ImageName = fileApi.ImgName;
            empObj.ImageUrl = fileApi.ImgName;
            List<Experience> expList = JsonConvert.DeserializeObject<List<Experience>>(objCommon.Experiences!);
            var existingExperoences = _db.Experiences.Where(e => e.EmployeeId == id);
            _db.Experiences.RemoveRange(existingExperoences);

            if (expList.Any())
            {
                foreach (var item in expList)
                {
                    Experience exp = new Experience
                    {
                        EmployeeId = empObj.EmployeeId,
                        Title = item.Title,
                        Duration=item.Duration
                    };
                    _db.Experiences.Add(exp);

                }
                await _db.SaveChangesAsync();
            }       

            return Ok("Employee Updated successfully");
        }

        [HttpDelete()]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var empObj = await _db.Employees.FindAsync(id);
            if (empObj == null) return NotFound("Employee not found");
            _db.Employees.Remove(empObj);
            await _db.SaveChangesAsync();
            return Ok("Employee deleted successfully");
        }

    }
}
