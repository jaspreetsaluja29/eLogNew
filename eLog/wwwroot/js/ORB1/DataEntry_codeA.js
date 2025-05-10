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
            // Reset ballasting fields when switching to cleaning
            resetBallastingFields();
        } else if (selectedValue === 'Ballasting') {
            $('#ballastingFields').show();
            $('#cleaningFields').hide();
            // Reset cleaning fields when switching to ballasting
            resetCleaningFields();
        } else {
            $('#cleaningFields, #ballastingFields').hide();
        }
    });

    // Regex for validating position format: (XX DEG XX MIN N/S, XX DEG XX MIN E/W)
    const positionRegex = /^\d{1,2} DEG \d{1,2} MIN [NS], \d{1,3} DEG \d{1,2} MIN [EW]$/;

    // Function to check if the position format is valid
    function isValidPositionFormat(value) {
        return positionRegex.test(value.trim());
    }

    // Form validation function
    function validateForm() {
        let isValid = true;
        let errorMessage = '';

        // Validate common fields
        if (!$('#EntryDate').val()) {
            errorMessage += "Date is required.\n";
            highlightField('#EntryDate');
            isValid = false;
        }

        if ($('#ballastingOrCleaning').val() === 'Select') {
            errorMessage += "Please select Ballasting or Cleaning.\n";
            highlightField('#ballastingOrCleaning');
            isValid = false;
        }

        // Validate based on the selected operation
        const operation = $('#ballastingOrCleaning').val();

        if (operation === 'Cleaning') {
            // Validate cleaning fields
            if ($('#wasCleaned').val() === 'Select' || !$('#wasCleaned').val()) {
                errorMessage += "Please indicate whether tank was cleaned since last containing oil.\n";
                highlightField('#wasCleaned');
                isValid = false;
            }

            if ($('#wasCleaned').val() === 'Yes') {
                if (!$('#dateLastCleaning').val()) {
                    errorMessage += "Date of Last Cleaning is required.\n";
                    highlightField('#dateLastCleaning');
                    isValid = false;
                }

                if (!$('#oilCommercialName').val()) {
                    errorMessage += "Oil Commercial Name is required.\n";
                    highlightField('#oilCommercialName');
                    isValid = false;
                }

                if (!$('#densityViscosity').val()) {
                    errorMessage += "Density and/or Viscosity is required.\n";
                    highlightField('#densityViscosity');
                    isValid = false;
                }
            }

            if ($('#wasCleaned').val() === 'No' && !$('#PreviousOilType').val()) {
                errorMessage += "Type of oil previously carried is required.\n";
                highlightField('#PreviousOilType');
                isValid = false;
            }

            if (!$('#StartCleaningTime').val()) {
                errorMessage += "Start cleaning Time is required.\n";
                highlightField('#StartCleaningTime');
                isValid = false;
            }

            if (!$('#PositionStart').val()) {
                errorMessage += "Position at Start is required.\n";
                highlightField('#PositionStart');
                isValid = false;
            }

            if (!$('#StopCleaningTime').val()) {
                errorMessage += "Stop cleaning Time is required.\n";
                highlightField('#StopCleaningTime');
                isValid = false;
            }

            if (!$('#PositionStop').val()) {
                errorMessage += "Position at Stop is required.\n";
                highlightField('#PositionStop');
                isValid = false;
            }

            if (!$('#IdentifyTanks').val()) {
                errorMessage += "Identify tanks(s) is required.\n";
                highlightField('#IdentifyTanks');
                isValid = false;
            }

            if ($('#MethodCleaning').val() === 'Select' || !$('#MethodCleaning').val()) {
                errorMessage += "Method of cleaning is required.\n";
                highlightField('#MethodCleaning');
                isValid = false;
            }

            if ($('#MethodCleaning').val() === 'Chemicals') {
                if (!$('#ChemicalType').val()) {
                    errorMessage += "Type of chemical is required.\n";
                    highlightField('#ChemicalType');
                    isValid = false;
                }

                if (!$('#ChemicalQuantity').val()) {
                    errorMessage += "Quantity of chemicals is required.\n";
                    highlightField('#ChemicalQuantity');
                    isValid = false;
                } else if (isNaN(parseFloat($('#ChemicalQuantity').val()))) {
                    errorMessage += "Quantity of chemicals must be a number.\n";
                    highlightField('#ChemicalQuantity');
                    isValid = false;
                }
            }
        } else if (operation === 'Ballasting') {
            // Validate ballasting fields
            if (!$('#QuantityBallast').val()) {
                errorMessage += "Quantity of ballast is required.\n";
                highlightField('#QuantityBallast');
                isValid = false;
            } else if (isNaN(parseFloat($('#QuantityBallast').val()))) {
                errorMessage += "Quantity of ballast must be a number.\n";
                highlightField('#QuantityBallast');
                isValid = false;
            }

            if (!$('#IdentityOfTanksBallasted').val()) {
                errorMessage += "Identity of tank(s) ballasted is required.\n";
                highlightField('#IdentityOfTanksBallasted');
                isValid = false;
            }

            if (!$('#StartBallastingTime').val()) {
                errorMessage += "Time at start of Ballasting is required.\n";
                highlightField('#StartBallastingTime');
                isValid = false;
            }

            if (!$('#BallastingPositionStart').val()) {
                errorMessage += "Position of ship at start of Ballasting is required.\n";
                highlightField('#BallastingPositionStart');
                isValid = false;
            } else if (!isValidPositionFormat($('#BallastingPositionStart').val())) {
                errorMessage += "Invalid format for 'Position of ship at start of Ballasting'.\nUse: XX DEG XX MIN N/S, XX DEG XX MIN E/W\n";
                highlightField('#BallastingPositionStart');
                isValid = false;
            }

            if (!$('#CompletionBallastingTime').val()) {
                errorMessage += "Time on completion of Ballasting is required.\n";
                highlightField('#CompletionBallastingTime');
                isValid = false;
            }

            if (!$('#BallastingPositionCompletion').val()) {
                errorMessage += "Position of ship on completion of Ballasting is required.\n";
                highlightField('#BallastingPositionCompletion');
                isValid = false;
            } else if (!isValidPositionFormat($('#BallastingPositionCompletion').val())) {
                errorMessage += "Invalid format for 'Position of ship on completion of Ballasting'.\nUse: XX DEG XX MIN N/S, XX DEG XX MIN E/W\n";
                highlightField('#BallastingPositionCompletion');
                isValid = false;
            }
        }

        // Display error message if validation fails
        if (!isValid) {
            alert("Please correct the following errors:\n" + errorMessage);
        }

        return isValid;
    }

    // Function to highlight invalid fields
    function highlightField(selector) {
        $(selector).addClass('is-invalid');

        // Remove the highlight when the field is edited
        $(selector).one('input change', function () {
            $(this).removeClass('is-invalid');
        });
    }

    // Function to reset ballasting fields
    function resetBallastingFields() {
        $('#QuantityBallast, #IdentityOfTanksBallasted, #StartBallastingTime, #CompletionBallastingTime').val('');
        $('#BallastingPositionStart, #BallastingPositionCompletion').val('XX DEG XX MIN N/S, XX DEG XX MIN E/W');
    }

    // Function to reset cleaning fields
    function resetCleaningFields() {
        $('#wasCleaned').val('Select');
        $('#dateLastCleaning, #oilCommercialName, #densityViscosity, #PreviousOilType').val('');
        $('#StartCleaningTime, #StopCleaningTime, #PositionStart, #PositionStop, #IdentifyTanks').val('');
        $('#MethodCleaning').val('Select');
        $('#ChemicalType, #ChemicalQuantity').val('');
        $('#wasCleanedFields, #wasCleanedFieldsNo, #ChemicalFields').hide();
    }

    // Save form data via AJAX - modified to include validation
    $('#saveButton').click(function (event) {
        event.preventDefault();

        // Validate form before submission
        if (!validateForm()) {
            return;
        }

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
                $('#cleaningFields, #wasCleanedFields, #wasCleanedFieldsNo, #ChemicalFields, #ballastingFields').hide();

                // Reset selectbox
                $('#ballastingOrCleaning').val('Select');
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
        $('#cleaningFields, #wasCleanedFields, #wasCleanedFieldsNo, #ChemicalFields, #ballastingFields').hide();
        $('#message').html('');

        // Reset date field with today's date
        $('#EntryDate').val(formattedDate);

        // Remove validation highlights
        $('.is-invalid').removeClass('is-invalid');
    });

    // Cancel button - Redirect to another page
    $("#cancelButton").click(function () {
        const { pageNumber, pageSize } = getQueryParams();
        // Redirect only after the user clicks "OK"
        var basePath = document.querySelector('base')?.getAttribute('href') || '/';
        window.location.href = `${basePath}/ORB1/CodeA/GetCodeAData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    });

    // Toggle fields based on selection
    $('#wasCleaned').change(function () {
        var selectedValue = $(this).val();
        $('#wasCleanedFields').toggle(selectedValue === 'Yes');
        $('#wasCleanedFieldsNo').toggle(selectedValue === 'No');
    });

    $('#MethodCleaning').change(function () {
        $('#ChemicalFields').toggle($(this).val() === 'Chemicals');
    });

    // Add CSS for validation
    $('<style>')
        .prop('type', 'text/css')
        .html(`
            .is-invalid {
                border-color: #dc3545 !important;
                padding-right: calc(1.5em + 0.75rem) !important;
                background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 12 12' width='12' height='12' fill='none' stroke='%23dc3545'%3e%3ccircle cx='6' cy='6' r='4.5'/%3e%3cpath stroke-linejoin='round' d='M5.8 3.6h.4L6 6.5z'/%3e%3ccircle cx='6' cy='8.2' r='.6' fill='%23dc3545' stroke='none'/%3e%3c/svg%3e") !important;
                background-repeat: no-repeat !important;
                background-position: right calc(0.375em + 0.1875rem) center !important;
                background-size: calc(0.75em + 0.375rem) calc(0.75em + 0.375rem) !important;
            }
        `)
        .appendTo('head');
});

// Function to get query parameters from the URL
function getQueryParams() {
    const urlParams = new URLSearchParams(window.location.search); // Get the query string from the URL
    const pageNumber = urlParams.get('pageNumber') || 1; // Default to 1 if not provided
    const pageSize = urlParams.get('pageSize') || 10; // Default to 10 if not provided

    return { pageNumber, pageSize };
}