using Client.Models;
using Client.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Client.Controllers
{
    public class EmployeesController : Controller
    {
        private string apiUrl = "http://localhost:5029/api/Employees";
        public async Task<IActionResult> Index()
        {
            ViewBag.ApiUrl = "http://localhost:5029";
            List<Employee> empList = new List<Employee>();
            using (var httpClient=new HttpClient())
            {
                using (var response=await httpClient.GetAsync(apiUrl))
                {
                    string apiResponse=await response.Content.ReadAsStringAsync();
                    empList = JsonConvert.DeserializeObject<List<Employee>>(apiResponse);
                }
            }
            return View(empList);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
           EmployeeViewModel model = new EmployeeViewModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(EmployeeViewModel employee)
        {
            Common obj = new Common();
            obj.Experiences = JsonConvert.SerializeObject(employee.Experiences, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(employee.EmployeeId.ToString()), "EmployeeId");
            content.Add(new StringContent(employee.EmployeeName.ToString()), "EmployeeName");
            content.Add(new StringContent(employee.IsActive.ToString()), "IsActive");
            content.Add(new StringContent(employee.JoinDate.ToString("yyyy-MM-dd")), "JoinDate");
            if (employee.ProfileFile != null)
            {
                content.Add(new StreamContent(employee.ProfileFile.OpenReadStream()), "ImageFile", employee.ProfileFile.FileName);
                content.Add(new StringContent(employee.ProfileFile.FileName), "ImageName");
            }
            content.Add(new StringContent(obj.Experiences), "Experiences");
            try
            {
                using (var httpClient=new HttpClient())
                {
                    var response = await httpClient.PostAsync(apiUrl, content);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "Error occurred while creating employee.");
                    }
                }
            }
            catch (Exception ex)
            {

                return StatusCode((500), ex.Message);
            }
           
        }

        public async Task<IActionResult> Delete(int id)
        {

            using (var httpClient = new HttpClient())
            {
                using (var response=await httpClient.DeleteAsync(apiUrl + $"?id={id}"))
                {
                    if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index");
                }
                    

            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.ApiUrl = "http://localhost:5029";
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(apiUrl + $"/{id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var employee = JsonConvert.DeserializeObject<Employee>(apiResponse);
                        if (employee != null)
                        {
                            var employeeViewModel = new EmployeeViewModel
                            {
                                EmployeeId = employee.EmployeeId,
                                EmployeeName = employee.EmployeeName,
                                IsActive = employee.IsActive,
                                JoinDate = employee.JoinDate,
                                ImageUrl = employee.ImageUrl,
                                Experiences = employee.Experiences.ToList()
                            };
                            return View(employeeViewModel);
                        }
                        else
                        {
                            return StatusCode((int)response.StatusCode, "Employee not found.");
                        }
                    }
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeViewModel employee)
        {
            Common obj = new Common();
            obj.Experiences = JsonConvert.SerializeObject(employee.Experiences, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(employee.EmployeeId.ToString()), "EmployeeId");
            content.Add(new StringContent(employee.EmployeeName.ToString()), "EmployeeName");
            content.Add(new StringContent(employee.IsActive.ToString()), "IsActive");
            content.Add(new StringContent(employee.JoinDate.ToString("yyyy-MM-dd")), "JoinDate");
            if (employee.ProfileFile != null)
            {
                content.Add(new StreamContent(employee.ProfileFile.OpenReadStream()), "ImageFile", employee.ProfileFile.FileName);
                content.Add(new StringContent(employee.ProfileFile.FileName), "ImageName");
            }
            else
            {
                if (!string.IsNullOrEmpty(employee.ImageUrl))
                {
                    content.Add(new StringContent(employee.ImageUrl), "ImageName"); 
                }
            }
            content.Add(new StringContent(obj.Experiences), "Experiences");
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.PutAsync(apiUrl+$"?id={employee.EmployeeId}", content);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "Error occurred while creating employee.");
                    }
                }
            }
            catch (Exception ex)
            {

                return StatusCode((500), ex.Message);
            }

        }
    }
}
 