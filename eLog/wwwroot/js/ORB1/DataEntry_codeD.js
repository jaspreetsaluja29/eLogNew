$(document).ready(function () {
    // Set default date in the format YYYY-MM-DD
    let today = new Date();
    let formattedDate = today.getFullYear() + '-' +
        String(today.getMonth() + 1).padStart(2, '0') + '-' +
        String(today.getDate()).padStart(2, '0');

    // Set the formatted date as the value of the input
    $('#EntryDate').val(formattedDate);

    // Hide all fields except Date & Ballasting or Cleaning at the start
    $('#EquipmentFields, #ReceptionFields, #SlopFields').hide();

    // Show respective fields based on selection
    $('#MethodDischargeTransferDisposal').change(function () {
        var selectedValue = $(this).val();
        if (selectedValue === 'Through 15 PPM Equipment') {
            $('#EquipmentFields').show();
            $('#ReceptionFields').hide();
            $('#SlopFields').hide();
        } else if (selectedValue === 'To Reception facilities') {
            $('#EquipmentFields').hide();
            $('#ReceptionFields').show();
            $('#SlopFields').hide();

        } else if (selectedValue === 'To Slop, holding or other tank') {
            $('#EquipmentFields').hide();
            $('#ReceptionFields').hide();
            $('#SlopFields').show();
        } else {
            $('#EquipmentFields, #ReceptionFields, #SlopFields').hide();
        }
    });

    // Add this to your existing document.ready function
    $('#ReceptionAttachment').change(function () {
        previewAttachment(this);
    });

    // Call the function to fetch and populate dropdowns on page load
    fetchTankData();

    var basePath = document.querySelector('base')?.getAttribute('href') || ''; // Get base path

    // Save form data via AJAX
    $('#saveButton').click(function (event) {
        event.preventDefault();

        var formData = new FormData(); // Use FormData to handle both text and file data

        // Append text fields
        formData.append("UserId", userId);
        formData.append("EntryDate", $('#EntryDate').val());
        formData.append("MethodDischargeTransferDisposal", $('#MethodDischargeTransferDisposal').val() || "");

        // Equipment Fields
        formData.append("EquipmentQuantity", parseFloat($('#EquipmentQuantity').val()) || "");
        formData.append("EquipmentResidue", $('#EquipmentResidue').val() || "");
        formData.append("EquipmentTransferredFrom", $('#EquipmentTransferredFrom').val() || "");
        formData.append("EquipmentQuantityRetained", parseFloat($('#EquipmentQuantityRetained').val()) || "");
        formData.append("EquipmentStartTime", $('#EquipmentStartTime').val() || "");
        formData.append("EquipmentPositionStart", $('#EquipmentPositionStart').val() || "");
        formData.append("EquipmentStopTime", $('#EquipmentStopTime').val() || "");
        formData.append("EquipmentPositionStop", $('#EquipmentPositionStop').val() || "");

        // Reception Fields
        formData.append("ReceptionQuantity", parseFloat($('#ReceptionQuantity').val()) || "");
        formData.append("ReceptionResidue", $('#ReceptionResidue').val() || "");
        formData.append("ReceptionTransferredFrom", $('#ReceptionTransferredFrom').val() || "");
        formData.append("ReceptionQuantityRetained", parseFloat($('#ReceptionQuantityRetained').val()) || "");
        formData.append("ReceptionStartTime", $('#ReceptionStartTime').val() || "");
        formData.append("ReceptionStopTime", $('#ReceptionStopTime').val() || "");
        formData.append("ReceptionPortFacilities", $('#ReceptionPortFacilities').val() || "");
        formData.append("ReceptionReceiptNo", $('#ReceptionReceiptNo').val() || "");

        // Slop Fields
        formData.append("SlopTransferredTo", $('#SlopTransferredTo').val() || "");
        formData.append("SlopQuantity", parseFloat($('#SlopQuantity').val()) || "");
        formData.append("SlopResidue", $('#SlopResidue').val() || "");
        formData.append("SlopTransferredFrom", $('#SlopTransferredFrom').val() || "");
        formData.append("SlopQuantityRetainedFrom", parseFloat($('#SlopQuantityRetainedFrom').val()) || "");
        formData.append("SlopStartTime", $('#SlopStartTime').val() || "");
        formData.append("SlopStopTime", $('#SlopStopTime').val() || "");
        formData.append("SlopQuantityRetainedTo", parseFloat($('#SlopQuantityRetainedTo').val()) || "");

        // Get the file input and append the file if available
        var fileInput = $('#ReceptionAttachment')[0].files[0];
        if (fileInput) {
            formData.append("ReceptionAttachment", fileInput);
        }

        $.ajax({
            url: basePath + "/ORB1/CodeD/CreateCodeD", // Corrected URL
            type: "POST",
            processData: false, // Prevent jQuery from processing data
            contentType: false, // Let the browser set Content-Type to multipart/form-data
            data: formData,
            success: function (response) {
                $('#message').html('<div class="alert alert-success">' + response.message + '</div>');

                // Reset form fields after successful save
                $('#DataEntryCodeDForm')[0].reset();

                // Reset date field with today's date
                let today = new Date();
                let formattedDate = today.getFullYear() + '-' +
                    String(today.getMonth() + 1).padStart(2, '0') + '-' +
                    String(today.getDate()).padStart(2, '0');
                $('#EntryDate').val(formattedDate);

                // Hide dependent fields
                $('#EquipmentFields, #ReceptionFields, #SlopFields').hide();
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", xhr.responseText);
                $('#message').html('<div class="alert alert-danger">Error: ' + xhr.responseText + '</div>');
            }
        });
    });

    // Reset form fields
    $('#resetButton').click(function () {
        $('#DataEntryCodeDForm')[0].reset();
        $('#EquipmentFields, #ReceptionFields, #SlopFields').hide();
        $('#message').html('');
    });

    // Cancel button - Redirect to another page
    $("#cancelButton").click(function () {
        const { pageNumber, pageSize } = getQueryParams();
        // Redirect only after the user clicks "OK"
        var basePath = document.querySelector('base')?.getAttribute('href') || '/';
        window.location.href = `${basePath}/ORB1/CodeD/GetCodeDData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    });
});

// Function to get query parameters from the URL
function getQueryParams() {
    const urlParams = new URLSearchParams(window.location.search); // Get the query string from the URL
    const pageNumber = urlParams.get('pageNumber') || 1; // Default to 1 if not provided
    const pageSize = urlParams.get('pageSize') || 10; // Default to 10 if not provided

    return { pageNumber, pageSize };
}

function fetchTankData() {
    var basePath = document.querySelector('base')?.getAttribute('href') || ''; // Get base path

    $.ajax({
        url: basePath + "/ORB1/CodeD/GetTanks",
        type: "GET",
        dataType: "json",
        success: function (response) {
            console.log("Response received:", response);
            if (response.error) {
                console.error("Error fetching tanks:", response.error);
                return;
            }
            // Generate dropdown options
            var optionsHtml = '<option value="">Select Tank</option>'; // Default option
            response.forEach(function (tank) {
                // Use the correct property names as shown in the console
                var formattedText = `${tank.tankIdentification}, Frames: ${tank.tankLocation_Frames_From_To}, Capacity: ${tank.volumeCapacity} m³`;
                optionsHtml += `<option value="${tank.tankIdentification}">${formattedText}</option>`;
            });
            // Populate dropdown fields
            $('#EquipmentTransferredFrom, #ReceptionTransferredFrom, #SlopTransferredTo, #SlopTransferredFrom').html(optionsHtml);

            // Check if dropdowns were populated successfully
            console.log("Dropdowns populated with options:", optionsHtml);
        },
        error: function (xhr, status, error) {
            console.error("AJAX Error:", xhr.responseText);
        }
    });
}

// Function to preview attachment before upload
function previewAttachment(input) {
    $('#attachmentPreview').empty();

    if (input.files && input.files[0]) {
        const file = input.files[0];
        const fileName = file.name;
        const fileSize = Math.round(file.size / 1024) + ' KB';
        const fileExt = fileName.split('.').pop().toLowerCase();

        let previewHtml = `
            <div class="card p-2">
                <div class="d-flex align-items-center">
                    <div class="attachment-icon me-2">`;

        // Show different icons based on file type
        if (['jpg', 'jpeg', 'png'].includes(fileExt)) {
            const reader = new FileReader();
            reader.onload = function (e) {
                $('#attachmentPreview .attachment-icon').html(`
                    <img src="${e.target.result}" class="img-thumbnail" style="max-height: 80px;" alt="Preview">
                `);
            };
            reader.readAsDataURL(file);
            previewHtml += `<span class="spinner-border spinner-border-sm" role="status"></span>`;
        } else if (fileExt === 'pdf') {
            previewHtml += `<i class="bi bi-file-earmark-pdf text-danger fs-3"></i>`;
        } else if (['doc', 'docx'].includes(fileExt)) {
            previewHtml += `<i class="bi bi-file-earmark-word text-primary fs-3"></i>`;
        } else if (['xlsx', 'xlsm'].includes(fileExt)) {
            previewHtml += `<i class="bi bi-file-earmark-excel text-success fs-3"></i>`;
        } else {
            previewHtml += `<i class="bi bi-file-earmark fs-3"></i>`;
        }

        previewHtml += `
                    </div>
                    <div>
                        <p class="mb-0 fw-bold">${fileName}</p>
                        <small class="text-muted">${fileSize}</small>
                    </div>
                </div>
            </div>`;

        $('#attachmentPreview').html(previewHtml);
    }
}
