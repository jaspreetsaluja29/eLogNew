﻿using eLog.Models.ORB1;
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
        //Code A Start
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
        //Code A Ends

        //Code C Start
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
        //Code C Ends

        //Code H Start
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
        //Code H Ends

        //Code D Start
        public IActionResult CodeD()
        {
            return RedirectToAction("GetCodeDData", "CodeD");
        }

        [Route("ORB1/CodeD/CreateCodeD")]
        [HttpPost]
        public async Task<IActionResult> CreateCodeD([FromForm] CodeDModel model, IFormFile? ReceptionAttachment)
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
                        foreach (var prop in typeof(CodeDModel).GetProperties())
                        {
                            var value = prop.GetValue(model)?.ToString();
                            if (value != null)
                            {
                                multipartContent.Add(new StringContent(value), prop.Name);
                            }
                        }

                        // Add the file if it exists
                        if (ReceptionAttachment != null && ReceptionAttachment.Length > 0)
                        {
                            // Create a byte array and read all file content into it
                            var fileBytes = new byte[ReceptionAttachment.Length];
                            using (var stream = ReceptionAttachment.OpenReadStream())
                            {
                                await stream.ReadAsync(fileBytes, 0, (int)ReceptionAttachment.Length);
                            }

                            // Create a ByteArrayContent from the bytes we just read
                            var fileContent = new ByteArrayContent(fileBytes);
                            multipartContent.Add(fileContent, "attachment", ReceptionAttachment.FileName);
                        }

                        var response = await client.PostAsync($"{baseUrl}/ORB1/CodeD/Create", multipartContent);

                        if (response.IsSuccessStatusCode)
                        {
                            return Json(new { success = true, message = "Data submitted successfully!" });
                        }
                        else
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            return StatusCode((int)response.StatusCode, $"Error in CodeDController: {errorContent}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Route("ORB1/CodeD/UpdateCodeD")]
        [HttpPost]
        public async Task<IActionResult> UpdateCodeD([FromBody] CodeDViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeDController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeD/Update", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeDController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Route("ORB1/CodeD/ApproverUpdateCodeD")]
        [HttpPost]
        public async Task<IActionResult> ApproverUpdateCodeD([FromBody] CodeDViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeDController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeD/ApproverUpdate", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeDController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //Code D Ends

        //Code G Start
        public IActionResult CodeG()
        {
            return RedirectToAction("GetCodeGData", "CodeG");
        }
        [Route("ORB1/CodeG/CreateCodeG")]
        [HttpPost]
        public async Task<IActionResult> CreateCodeG([FromBody] CodeGModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeGController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeG/Create", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeGController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Route("ORB1/CodeG/UpdateCodeG")]
        [HttpPost]
        public async Task<IActionResult> UpdateCodeG([FromBody] CodeGViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeGController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeG/Update", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeGController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Route("ORB1/CodeG/ApproverUpdateCodeG")]
        [HttpPost]
        public async Task<IActionResult> ApproverUpdateCodeG([FromBody] CodeGViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeGController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeG/ApproverUpdate", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeGController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //Code G Ends

        //Code F Start
        public IActionResult CodeF()
        {
            return RedirectToAction("GetCodeFData", "CodeF");
        }
        [Route("ORB1/CodeF/CreateCodeF")]
        [HttpPost]
        public async Task<IActionResult> CreateCodeF([FromBody] CodeFModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeFController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeF/Create", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeFController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Route("ORB1/CodeF/UpdateCodeF")]
        [HttpPost]
        public async Task<IActionResult> UpdateCodeF([FromBody] CodeFViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeFController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeF/Update", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeFController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Route("ORB1/CodeF/ApproverUpdateCodeF")]
        [HttpPost]
        public async Task<IActionResult> ApproverUpdateCodeF([FromBody] CodeFViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeFController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeF/ApproverUpdate", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeFController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //Code F Ends

        //Code B Start
        public IActionResult CodeB()
        {
            return RedirectToAction("GetCodeBData", "CodeB");
        }

        [Route("ORB1/CodeB/CreateCodeB")]
        [HttpPost]
        public async Task<IActionResult> CreateCodeB([FromForm] CodeBModel model, IFormFile? ReceptionAttachment)
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
                        foreach (var prop in typeof(CodeBModel).GetProperties())
                        {
                            var value = prop.GetValue(model)?.ToString();
                            if (value != null)
                            {
                                multipartContent.Add(new StringContent(value), prop.Name);
                            }
                        }

                        // Add the file if it exists
                        if (ReceptionAttachment != null && ReceptionAttachment.Length > 0)
                        {
                            // Create a byte array and read all file content into it
                            var fileBytes = new byte[ReceptionAttachment.Length];
                            using (var stream = ReceptionAttachment.OpenReadStream())
                            {
                                await stream.ReadAsync(fileBytes, 0, (int)ReceptionAttachment.Length);
                            }

                            // Create a ByteArrayContent from the bytes we just read
                            var fileContent = new ByteArrayContent(fileBytes);
                            multipartContent.Add(fileContent, "attachment", ReceptionAttachment.FileName);
                        }

                        var response = await client.PostAsync($"{baseUrl}/ORB1/CodeB/Create", multipartContent);

                        if (response.IsSuccessStatusCode)
                        {
                            return Json(new { success = true, message = "Data submitted successfully!" });
                        }
                        else
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            return StatusCode((int)response.StatusCode, $"Error in CodeBController: {errorContent}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Route("ORB1/CodeB/UpdateCodeB")]
        [HttpPost]
        public async Task<IActionResult> UpdateCodeB([FromBody] CodeBViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeBController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeB/Update", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeBController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Route("ORB1/CodeB/ApproverUpdateCodeB")]
        [HttpPost]
        public async Task<IActionResult> ApproverUpdateCodeB([FromBody] CodeBViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeBController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeB/ApproverUpdate", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeBController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //Code B Ends

        //Code E Start
        public IActionResult CodeE()
        {
            return RedirectToAction("GetCodeEData", "CodeE");
        }
        [Route("ORB1/CodeE/CreateCodeE")]
        [HttpPost]
        public async Task<IActionResult> CreateCodeE([FromBody] CodeEModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeEController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeE/Create", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeEController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Route("ORB1/CodeE/UpdateCodeE")]
        [HttpPost]
        public async Task<IActionResult> UpdateCodeE([FromBody] CodeEViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeEController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeE/Update", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeEController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Route("ORB1/CodeE/ApproverUpdateCodeE")]
        [HttpPost]
        public async Task<IActionResult> ApproverUpdateCodeE([FromBody] CodeEViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeEController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeE/ApproverUpdate", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeEController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //Code E Ends

        //Code I Start
        public IActionResult CodeI()
        {
            return RedirectToAction("GetCodeIData", "CodeI");
        }
        [Route("ORB1/CodeI/CreateCodeI")]
        [HttpPost]
        public async Task<IActionResult> CreateCodeI([FromBody] CodeIModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeIController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeI/Create", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeIController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Route("ORB1/CodeI/UpdateCodeI")]
        [HttpPost]
        public async Task<IActionResult> UpdateCodeI([FromBody] CodeIViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeIController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeI/Update", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeIController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Route("ORB1/CodeI/ApproverUpdateCodeI")]
        [HttpPost]
        public async Task<IActionResult> ApproverUpdateCodeI([FromBody] CodeIViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data received.");
            }

            try
            {
                string json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call CodeIController.Create via HTTP POST
                var baseUrl = $"{Request.Scheme}://{Request.Host}{_basePath}";
                var response = await _httpClient.PostAsync($"{baseUrl}/ORB1/CodeI/ApproverUpdate", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Data submitted successfully!" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error in CodeIController.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //Code I Ends
    }
}