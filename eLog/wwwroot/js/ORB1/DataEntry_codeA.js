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

        var formData = {
            UserId: userId,  // Include the UserID
            EntryDate: $('#EntryDate').val(),
            BallastingOrCleaning: $('#ballastingOrCleaning').val(),
            LastCleaningDate: $('#dateLastCleaning').val() || null,
            OilCommercialName: $('#oilCommercialName').val() || null,
            DensityViscosity: $('#densityViscosity').val() || null,
            CleanedLastContainedOil: $('#wasCleaned').val() === "Yes" ? true : false,
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
            url: "/ORB1/CodeA/CreateCodeA",
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
                console.log(xhr.responseText);
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
    $('#cancelButton').click(function () {
        window.location.href = '/ORB1/CodeA/GetCodeAData'; // Change to the appropriate redirection URL
    });

    // Toggle fields based on selection
    $('#ballastingOrCleaning').change(function () {
        var selectedValue = $(this).val();
        $('#cleaningFields').toggle(selectedValue === 'Cleaning');
        $('#ballastingFields').toggle(selectedValue !== 'Cleaning');
    });

    $('#wasCleaned').change(function () {
        $('#wasCleanedFields').toggle($(this).val() === 'No');
    });

    $('#MethodCleaning').change(function () {
        $('#ChemicalFields').toggle($(this).val() === 'Chemicals');
    });
});