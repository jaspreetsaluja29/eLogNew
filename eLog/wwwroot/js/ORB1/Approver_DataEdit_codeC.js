$(document).ready(function () {
    initializeFormState();
    setupEventListeners();
    setupFormValidation();
});

function initializeFormState() {
    toggleFields();
}

function setupEventListeners() {
    $("#BallastingOrCleaning").change(toggleFields);
    $("#CleanedLastContainedOil").change(toggleWasCleanedFields);
    $("#MethodCleaning").change(toggleChemicalFields);

    // Handle cancel button click
    $("#cancelButton").click(function () {
        if (confirm("Are you sure you want to cancel? Any unsaved changes will be lost.")) {
            const { pageNumber, pageSize } = getQueryParams();
            window.location.href = `/ORB1/CodeA/GetCodeAData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
        }
    });

    // Handle form submission based on button clicked (Approve or Reject)
    $("#approveButton, #rejectButton").click(function (e) {
        // Set the action type based on which button was clicked
        $("#actionType").val($(this).val());

        // Ensure the Comments field exists and validate it
        var comments = $("#Comments").val();
        if (typeof comments === "undefined" || comments === null || !comments.trim()) {
            e.preventDefault();
            alert("Please provide a comment before " + $(this).val().toLowerCase() + "ing.");
            $("#Comments").focus();
            return false;
        }

        if (!confirm("Are you sure you want to " + $(this).val().toLowerCase() + " this entry?")) {
            e.preventDefault();
            return false;
        }
    });


    // Main form submission
    $("#EditCodeAForm").submit(function (event) {
        event.preventDefault();
        if ($(this).valid()) {
            submitForm();
        }
    });
}

function toggleFields() {
    let selection = $("#BallastingOrCleaning").val();
    $("#cleaningFields").toggle(selection === "Cleaning");
    $("#ballastingFields").toggle(selection === "Ballasting");
    toggleWasCleanedFields();
    toggleChemicalFields();
}

function toggleWasCleanedFields() {
    let cleaned = $("#CleanedLastContainedOil").val();
    $("#wasCleanedFields").toggle(cleaned === "No");
}

function toggleChemicalFields() {
    let method = $("#MethodCleaning").val();
    $("#ChemicalFields").toggle(method === "Chemicals");
}

function setupFormValidation() {
    $.validator.addMethod("validPosition", function (value) {
        if (!value) return true;
        return /^\d{1,2}\s*DEG\s*\d{1,2}\s*MIN\s*[NS],\s*\d{1,3}\s*DEG\s*\d{1,2}\s*MIN\s*[EW]$/i.test(value);
    }, "Format: XX DEG XX MIN N/S, XX DEG XX MIN E/W");

    $("#EditCodeAForm").validate({
        rules: {
            EntryDate: "required",
            BallastingOrCleaning: "required",
            LastCleaningDate: { required: () => $("#BallastingOrCleaning").val() === "Cleaning" },
            IdentifyTanks: { required: () => $("#BallastingOrCleaning").val() === "Cleaning" },
            PositionStart: { validPosition: true },
            PositionStop: { validPosition: true },
            ChemicalType: { required: () => $("#MethodCleaning").val() === "Chemicals" },
            ChemicalQuantity: { required: () => $("#MethodCleaning").val() === "Chemicals", number: true },
            QuantityBallast: { number: true },
            Comments: "required" // Add validation for the required comment field
        },
        messages: {
            EntryDate: "Please enter a valid date",
            BallastingOrCleaning: "Please select an option",
            ChemicalQuantity: "Please enter a valid number",
            Comments: "Please provide a comment" // Add validation message for comment
        }
    });
}

function submitForm() {
    let actionType = $("#actionType").val() || "Pending"; // Set a default value if empty

    let formData = {
        Id: $('#Id').val(),
        UserId: userId,
        EntryDate: $('#EntryDate').val(),
        BallastingOrCleaning: $('#BallastingOrCleaning').val(),
        LastCleaningDate: $('#LastCleaningDate').val() || null,
        OilCommercialName: $('#OilCommercialName').val() || null,
        DensityViscosity: $('#DensityViscosity').val() || null,
        CleanedLastContainedOil: $('#CleanedLastContainedOil').val() === "Yes",
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
        BallastingPositionCompletion: $('#BallastingPositionCompletion').val() || null,
        Comments: $('#Comments').val(),
        StatusName: actionType // Ensure StatusName always has a value
    };

    $.ajax({
        url: "/ORB1/CodeA/ApproverUpdateCodeA",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(formData),
        success: function (response) {
            $('#EditCodeAForm')[0].reset();
            resetFormDate();
            hideDependentFields();
            alert(response.message);
            const { pageNumber, pageSize } = getQueryParams();
            window.location.href = `/ORB1/CodeA/GetCodeAData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
        },
        error: function (xhr) {
            $('#message').html('<div class="alert alert-danger">Error: ' + xhr.responseText + '</div>');
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

function hideDependentFields() {
    $('#cleaningFields, #wasCleanedFields, #ChemicalFields, #ballastingFields').hide();
}

// Function to get query parameters from the URL
function getQueryParams() {
    const urlParams = new URLSearchParams(window.location.search); // Get the query string from the URL
    const pageNumber = urlParams.get('pageNumber') || 1; // Default to 1 if not provided
    const pageSize = urlParams.get('pageSize') || 10; // Default to 10 if not provided

    return { pageNumber, pageSize };
}