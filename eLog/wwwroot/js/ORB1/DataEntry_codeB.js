$(document).ready(function () {
    // Set default date in the format YYYY-MM-DD
    let today = new Date();
    let formattedDate = today.getFullYear() + '-' +
        String(today.getMonth() + 1).padStart(2, '0') + '-' +
        String(today.getDate()).padStart(2, '0');

    // Set the formatted date as the value of the input
    $('#EntryDate').val(formattedDate);

    // Show respective fields based on selection
    $('#MethodOfDischarge').change(function () {
        var selectedValue = $(this).val();
        if (selectedValue === 'To Reception Facilities') {
            $('#ReceptionFields').show();
        } else {
            $('#ReceptionFields').hide();
        }
    });

    // Add this to your existing document.ready function
    $('#ReceptionAttachment').change(function () {
        previewAttachment(this);
    });

    var basePath = document.querySelector('base')?.getAttribute('href') || ''; // Get base path
    basePath = basePath.endsWith('/') ? basePath : basePath + '/'; // Ensure trailing slash for basePath

    // Save form data via AJAX
    $('#saveButton').click(function (event) {
        event.preventDefault();

        var formData = new FormData(); // Use FormData to handle both text and file data

        // Append text fields
        formData.append("UserId", userId);
        formData.append("EntryDate", $('#EntryDate').val());
        formData.append("IdentityOfTanks", $('#IdentityOfTanks').val() || "");
        formData.append("PositionOfShipStart", $('#PositionOfShipStart').val() || "");
        formData.append("PositionOfShipCompletion", $('#PositionOfShipCompletion').val() || "");
        formData.append("ShipSpeedDischarge", $('#ShipSpeedDischarge').val() || "");
        formData.append("MethodOfDischarge", $('#MethodOfDischarge').val() || "");
        formData.append("ReceiptNo", $('#ReceiptNo').val() || "");
        formData.append("QuantityDischarged", parseFloat($('#QuantityDischarged').val()) || 0); // Changed to 0 if empty

        // Get the file input and append the file if available
        var fileInput = $('#ReceptionAttachment')[0].files[0];
        if (fileInput) {
            formData.append("ReceptionAttachment", fileInput);
        }

        $.ajax({
            url: basePath + "ORB1/CodeB/CreateCodeB", // Corrected URL
            type: "POST",
            processData: false, // Prevent jQuery from processing data
            contentType: false, // Let the browser set Content-Type to multipart/form-data
            data: formData,
            success: function (response) {
                $('#message').html('<div class="alert alert-success">' + response.message + '</div>');

                // Reset form fields after successful save
                $('#DataEntryCodeBForm')[0].reset();

                // Reset date field with today's date
                let today = new Date();
                let formattedDate = today.getFullYear() + '-' +
                    String(today.getMonth() + 1).padStart(2, '0') + '-' +
                    String(today.getDate()).padStart(2, '0');
                $('#EntryDate').val(formattedDate);
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", xhr.responseText);
                $('#message').html('<div class="alert alert-danger">Error: ' + xhr.responseText + '</div>');
            }
        });
    });

    // Reset form fields
    $('#resetButton').click(function () {
        $('#DataEntryCodeBForm')[0].reset();
        $('#message').html('');
    });

    // Cancel button - Redirect to another page
    $("#cancelButton").click(function () {
        const { pageNumber, pageSize } = getQueryParams();
        // Redirect only after the user clicks "OK"
        window.location.href = `${basePath}ORB1/CodeB/GetCodeBData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    });
});

// Function to get query parameters from the URL
function getQueryParams() {
    const urlParams = new URLSearchParams(window.location.search); // Get the query string from the URL
    const pageNumber = urlParams.get('pageNumber') || 1; // Default to 1 if not provided
    const pageSize = urlParams.get('pageSize') || 10; // Default to 10 if not provided

    return { pageNumber, pageSize };
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