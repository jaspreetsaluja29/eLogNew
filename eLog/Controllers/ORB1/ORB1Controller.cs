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
        private readonly string _basePath;

        public ORB1Controller(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _basePath = configuration["BasePath"] ?? "";
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
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
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
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
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
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
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
        public IActionResult CodeC()
        {
            return RedirectToAction("GetCodeCData", "CodeC");
        }

        [Route("ORB1/CodeC/CreateCodeC")]
        [HttpPost]
        public async Task<IActionResult> CreateCodeC([FromForm] CodeCModel model, IFormFile? DisposalShoreAttachment)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                // For direct file handling approach
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";

                // Create a new HttpClient instance for this specific request
                using (var client = new HttpClient())
                {
                    using (var multipartContent = new MultipartFormDataContent())
                    {
                        // Add the model data
                        // Convert model properties to string form values
                        foreach (var prop in typeof(CodeCModel).GetProperties())
                        {
                            var value = prop.GetValue(model)?.ToString();
                            if (value != null)
                            {
                                multipartContent.Add(new StringContent(value), prop.Name);
                            }
                        }

                        // Add the file if it exists
                        if (DisposalShoreAttachment != null && DisposalShoreAttachment.Length > 0)
                        {
                            // Create a byte array and read all file content into it
                            var fileBytes = new byte[DisposalShoreAttachment.Length];
                            using (var stream = DisposalShoreAttachment.OpenReadStream())
                            {
                                await stream.ReadAsync(fileBytes, 0, (int)DisposalShoreAttachment.Length);
                            }

                            // Create a ByteArrayContent from the bytes we just read
                            var fileContent = new ByteArrayContent(fileBytes);
                            multipartContent.Add(fileContent, "attachment", DisposalShoreAttachment.FileName);
                        }

                        var response = await client.PostAsync($"{baseUrl}/ORB1/CodeC/Create", multipartContent);

                        if (response.IsSuccessStatusCode)
                        {
                            return Json(new { success = true, message = "Data submitted successfully!" });
                        }
                        else
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            return StatusCode((int)response.StatusCode, $"Error in CodeCController: {errorContent}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Route("ORB1/CodeC/UpdateCodeC")]
        [HttpPost]
        public async Task<IActionResult> UpdateCodeC([FromBody] CodeCViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeCController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeC/Update", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeCController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Route("ORB1/CodeC/ApproverUpdateCodeC")]
        [HttpPost]
        public async Task<IActionResult> ApproverUpdateCodeC([FromBody] CodeCViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeCController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeC/ApproverUpdate", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeCController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public IActionResult CodeH()
        {
            return RedirectToAction("GetCodeHData", "CodeH");
        }

        [Route("ORB1/CodeH/CreateCodeH")]
        [HttpPost]
        public async Task<IActionResult> CreateCodeH([FromBody] CodeHModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeHController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeH/Create", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeHController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Route("ORB1/CodeH/UpdateCodeH")]
        [HttpPost]
        public async Task<IActionResult> UpdateCodeH([FromBody] CodeHViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeHController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeH/Update", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeHController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Route("ORB1/CodeH/ApproverUpdateCodeH")]
        [HttpPost]
        public async Task<IActionResult> ApproverUpdateCodeH([FromBody] CodeHViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeHController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeH/ApproverUpdate", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeHController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}