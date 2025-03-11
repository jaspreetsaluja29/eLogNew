using eLog.Models.ORB1;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using eLog.ViewModels.ORB1;

namespace eLog.Controllers.ORB1
{
    public class ORB1Controller : Controller
    {
        private readonly HttpClient _httpClient;

        public ORB1Controller(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public IActionResult CodeA()
        {
            return RedirectToAction("GetCodeAData", "CodeA");
        }
        [Route("ORB1/CodeA/CreateCodeA")]
        [HttpPost]
        public async Task<IActionResult> CreateCodeA([FromBody] CodeAModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeAController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeA/Create", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeAController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Route("ORB1/CodeA/UpdateCodeA")]
        [HttpPost]
        public async Task<IActionResult> UpdateCodeA([FromBody] CodeAViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeAController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeA/Update", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeAController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Route("ORB1/CodeA/ApproverUpdateCodeA")]
        [HttpPost]
        public async Task<IActionResult> ApproverUpdateCodeA([FromBody] CodeAViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeAController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeA/ApproverUpdate", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeAController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        public IActionResult CodeB()
        {
            return View();
        }
        public IActionResult CodeC()
        {
            return RedirectToAction("GetCodeCData", "CodeC");
        }

        [Route("ORB1/CodeC/CreateCodeC")]
        [HttpPost]
        public async Task<IActionResult> CreateCodeC([FromBody] CodeCModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeAController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeC/Create", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeAController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}