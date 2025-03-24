$(document).ready(function () {
    // Set default date in the format YYYY-MM-DD
    let today = new Date();
    let formattedDate = today.getFullYear() + '-' +
        String(today.getMonth() + 1).padStart(2, '0') + '-' +
        String(today.getDate()).padStart(2, '0');

    // Set the formatted date as the value of the input
    $('#EntryDate').val(formattedDate);

    // Hide all fields except Date & Ballasting or Cleaning at the start
    $('#cleaningFields, #ballastingFields').hide();

    // Show respective fields based on selection
    $('#ballastingOrCleaning').change(function () {
        var selectedValue = $(this).val();
        if (selectedValue === 'Cleaning') {
            $('#cleaningFields').show();
            $('#ballastingFields').hide();
        } else if (selectedValue === 'Ballasting') {
            $('#ballastingFields').show();
            $('#cleaningFields').hide();
        } else {
            $('#cleaningFields, #ballastingFields').hide();
        }
    });

    // Save form data via AJAX
    $('#saveButton').click(function (event) {
        event.preventDefault();

        var basePath = document.querySelector('base')?.getAttribute('href') || ''; // Get base path

        var formData = {
            UserId: userId, // Include the UserID
            EntryDate: $('#EntryDate').val(),
            BallastingOrCleaning: $('#ballastingOrCleaning').val(),
            LastCleaningDate: $('#dateLastCleaning').val() || null,
            OilCommercialName: $('#oilCommercialName').val() || null,
            DensityViscosity: $('#densityViscosity').val() || null,
            CleanedLastContainedOil: $('#wasCleaned').val() === "Yes",
            PreviousOilType: $('#PreviousOilType').val() || null,
            QuantityBallast: parseFloat($('#QuantityBallast').val()) || null,
            IdentityOfTanksBallasted: $('#IdentityOfTanksBallasted').val() || null,
            StartCleaningTime: $('#StartCleaningTime').val() || null,
            PositionStart: $('#PositionStart').val() || null,
            StopCleaningTime: $('#StopCleaningTime').val() || null,
            PositionStop: $('#PositionStop').val() || null,
            IdentifyTanks: $('#IdentifyTanks').val() || null,
            MethodCleaning: $('#MethodCleaning').val() || null,
            ChemicalType: $('#ChemicalType').val() || null,
            ChemicalQuantity: parseFloat($('#ChemicalQuantity').val()) || null,
            StartBallastingTime: $('#StartBallastingTime').val() || null,
            BallastingPositionStart: $('#BallastingPositionStart').val() || null,
            CompletionBallastingTime: $('#CompletionBallastingTime').val() || null,
            BallastingPositionCompletion: $('#BallastingPositionCompletion').val() || null
        };

        $.ajax({
            url: basePath + "/ORB1/CodeA/CreateCodeA", // Corrected URL string
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(formData),
            success: function (response) {
                $('#message').html('<div class="alert alert-success">' + response.message + '</div>');

                // Reset form fields after successful save
                $('#DataEntryCodeAForm')[0].reset();

                // Reset date field with today's date
                let today = new Date();
                let formattedDate = today.getFullYear() + '-' +
                    String(today.getMonth() + 1).padStart(2, '0') + '-' +
                    String(today.getDate()).padStart(2, '0');
                $('#EntryDate').val(formattedDate);

                // Hide dependent fields
                $('#cleaningFields, #wasCleanedFields, #ChemicalFields, #ballastingFields').hide();
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", xhr.responseText);
                $('#message').html('<div class="alert alert-danger">Error: ' + xhr.responseText + '</div>');
            }
        });
    });


    // Reset form fields
    $('#resetButton').click(function () {
        $('#DataEntryCodeAForm')[0].reset();
        $('#cleaningFields, #wasCleanedFields, #ChemicalFields').hide();
        $('#message').html('');
    });

    // Cancel button - Redirect to another page
    $("#cancelButton").click(function () {
        const { pageNumber, pageSize } = getQueryParams();
        // Redirect only after the user clicks "OK"
        var basePath = document.querySelector('base')?.getAttribute('href') || '/';
        window.location.href = `${basePath}/ORB1/CodeA/GetCodeAData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    });

    // Toggle fields based on selection
    $('#ballastingOrCleaning').change(function () {
        var selectedValue = $(this).val();
        $('#cleaningFields').toggle(selectedValue === 'Cleaning');
        $('#ballastingFields').toggle(selectedValue !== 'Cleaning');
    });

    $('#wasCleaned').change(function () {
        $('#wasCleanedFields').toggle($(this).val() === 'Yes');
    });
    $('#wasCleaned').change(function () {
        $('#wasCleanedFieldsNo').toggle($(this).val() === 'No');
    });

    $('#MethodCleaning').change(function () {
        $('#ChemicalFields').toggle($(this).val() === 'Chemicals');
    });
});

// Function to get query parameters from the URL
function getQueryParams() {
    const urlParams = new URLSearchParams(window.location.search); // Get the query string from the URL
    const pageNumber = urlParams.get('pageNumber') || 1; // Default to 1 if not provided
    const pageSize = urlParams.get('pageSize') || 10; // Default to 10 if not provided

    return { pageNumber, pageSize };
}
