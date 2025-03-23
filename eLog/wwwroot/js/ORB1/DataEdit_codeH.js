$(document).ready(function () {
    setupEventListeners();
    setupPortLocationHandling();
    // No custom validation since both fields can be empty
});

function setupEventListeners() {
    $("#cancelButton").click(function () {
        const { pageNumber, pageSize } = getQueryParams();
        // Redirect only after the user clicks "OK"
        var basePath = document.querySelector('base')?.getAttribute('href') || '/';
        window.location.href = `${basePath}/ORB1/CodeH/GetCodeHData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    });

    $("#EditCodeHForm").submit(function (event) {
        event.preventDefault();
        submitForm();
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

    // We'll modify the approach for enabling disabled fields
    // No event handler needed here anymore for the update button
}

var basePath = document.querySelector('base')?.getAttribute('href') || ''; // Get base path

function submitForm() {
    // Create a copy of the form data before enabling disabled fields
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
        TankRetained4: $('#TankRetained4').val() || null
    };

    // Before submitting, check if Port has a value and clear Location if needed
    if ($('#Port').val().trim() !== '') {
        formData.Location = null;
    }

    // No need to enable disabled fields anymore since we're working with our own formData object

    $.ajax({
        url: basePath + "/ORB1/CodeH/UpdateCodeH",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(formData),
        success: function (response) {
            // Reset the form and apply UI changes before showing the alert
            $('#EditCodeHForm')[0].reset();
            resetFormDate();
            // Show message in an alert popup and redirect when the user clicks "OK"
            alert(response.message);
            const { pageNumber, pageSize } = getQueryParams();
            // Redirect only after the user clicks "OK"
            window.location.href = `${basePath}/ORB1/CodeH/GetCodeHData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
        },
        error: function (xhr) {
            $('#message').html('<div class="alert alert-danger">Error: ' + xhr.responseText + '</div>');
            // Re-apply the disabled state to Location if Port has a value
            if ($('#Port').val().trim() !== '') {
                $('#Location').prop('disabled', true);
            }
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