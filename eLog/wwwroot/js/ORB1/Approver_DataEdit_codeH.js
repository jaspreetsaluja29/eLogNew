$(document).ready(function () {
    setupEventListeners();
    setupPortLocationHandling();
});

function setupEventListeners() {
    // Handle cancel button click
    $("#cancelButton").click(function () {
        if (confirm("Are you sure you want to cancel? Any unsaved changes will be lost.")) {
            const { pageNumber, pageSize } = getQueryParams();
            var basePath = document.querySelector('base')?.getAttribute('href') || '/';
            window.location.href = `${basePath}/ORB1/CodeH/GetCodeHData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
        }
    });

    // Handle form submission based on button clicked (Approve or Reject)
    $("#approveButton, #rejectButton").click(function (e) {
        e.preventDefault(); // Prevent the default form submission

        // Set the action type based on which button was clicked
        $("#actionType").val($(this).val());

        // Ensure the Comments field exists and validate it
        var comments = $("#Comments").val();
        if (typeof comments === "undefined" || comments === null || !comments.trim()) {
            alert("Please provide a comment before " + $(this).val().toLowerCase() + "ing.");
            $("#Comments").focus();
            return false;
        }

        if (!confirm("Are you sure you want to " + $(this).val().toLowerCase() + " this entry?")) {
            return false;
        }

        // Enable all disabled fields before submission so their values are included
        $("#EditCodeHForm input:disabled").prop("disabled", false);

        // Submit the form manually
        submitForm();
    });

    // Main form validation
    $("#EditCodeHForm").submit(function (event) {
        event.preventDefault();
        if ($(this).valid()) {
            // Enable all disabled fields before submission
            $("#EditCodeHForm input:disabled").prop("disabled", false);
            submitForm();
        }
    });
}

function setupPortLocationHandling() {
    // Initial check on page load - if Port has a value, disable Location
    if ($('#Port').val().trim() !== '') {
        $('#Location').prop('disabled', true).val('');
    }

    // Port field handling
    $('#Port').on('input', function () {
        const portValue = $(this).val().trim();

        if (portValue !== '') {
            // If Port has a value, disable and clear Location
            $('#Location').prop('disabled', true).val('');
        } else {
            // If Port is empty, enable Location
            $('#Location').prop('disabled', false);
        }
    });
}

var basePath = document.querySelector('base')?.getAttribute('href') || '/';
function submitForm() {
    let actionType = $("#actionType").val() || "Pending"; // Set a default value if empty

    // Enable all disabled fields temporarily to collect their values
    $("#EditCodeHForm input:disabled").prop("disabled", false);

    let formData = {
        Id: $('#Id').val(),
        UserId: userId, // Assuming userId is defined globally
        EntryDate: $('#EntryDate').val(),
        Port: $('#Port').val() || null,
        Location: $('#Location').val() || null,
        StartDateTime: $('#StartDateTime').val() || null,
        StopDateTime: $('#StopDateTime').val() || null,
        Quantity: parseFloat($('#Quantity').val()) || 0,
        Grade: $('#Grade').val() || null,
        SulphurContent: $('#SulphurContent').val() || null,
        TankLoaded1: $('#TankLoaded1').val() || null,
        TankRetained1: $('#TankRetained1').val() || null,
        TankLoaded2: $('#TankLoaded2').val() || null,
        TankRetained2: $('#TankRetained2').val() || null,
        TankLoaded3: $('#TankLoaded3').val() || null,
        TankRetained3: $('#TankRetained3').val() || null,
        TankLoaded4: $('#TankLoaded4').val() || null,
        TankRetained4: $('#TankRetained4').val() || null,
        Comments: $('#Comments').val(),
        StatusName: actionType // Ensure StatusName always has a value
    };

    // Before submitting, check if Port has a value and clear Location if needed
    if ($('#Port').val().trim() !== '') {
        formData.Location = null;
    }

    $.ajax({
        url: basePath + "/ORB1/CodeH/ApproverUpdateCodeH",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(formData),
        success: function (response) {
            $('#EditCodeHForm')[0].reset();
            resetFormDate();
            alert(response.message);
            const { pageNumber, pageSize } = getQueryParams();
            var basePath = document.querySelector('base')?.getAttribute('href') || '/';
            window.location.href = `${basePath}/ORB1/CodeH/GetCodeHData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
        },
        error: function (xhr) {
            $('#message').html('<div class="alert alert-danger">Error: ' + xhr.responseText + '</div>');
            // Re-apply the disabled state to Location if Port has a value
            setupPortLocationHandling();
        }
    });
}

function resetFormDate() {
    let today = new Date();
    let formattedDate = today.getFullYear() + '-' +
        String(today.getMonth() + 1).padStart(2, '0') + '-' +
        String(today.getDate()).padStart(2, '0');
    $('#EntryDate').val(formattedDate);
}

// Function to get query parameters from the URL
function getQueryParams() {
    const urlParams = new URLSearchParams(window.location.search); // Get the query string from the URL
    const pageNumber = urlParams.get('pageNumber') || 1; // Default to 1 if not provided
    const pageSize = urlParams.get('pageSize') || 10; // Default to 10 if not provided

    return { pageNumber, pageSize };
}