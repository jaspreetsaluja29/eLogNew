$(document).ready(function () {
    initializeFormState();
    setupEventListeners();
    setupFormValidation();
});

function initializeFormState() {
    toggleOperationFields();
}

function setupEventListeners() {
    $("#OperationType").change(toggleOperationFields);
    $("#TankCleaned").change(toggleTankCleaningFields);
    $("#DisposalMethod").change(toggleDisposalFields);

    $("#cancelButton").click(function () {
        const { pageNumber, pageSize } = getQueryParams();
        // Redirect only after the user clicks "OK"
        window.location.href = `/ORB1/CodeC/GetCodeCData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    });

    $("#EditCodeCForm").submit(function (event) {
        event.preventDefault();
        if ($(this).valid()) {
            submitForm();
        }
    });
}

function toggleOperationFields() {
    let operation = $("#OperationType").val();
    $("#disposalFields").toggle(operation === "Disposal");
    $("#retentionFields").toggle(operation === "Retention");
    $("#transferFields").toggle(operation === "Transfer");
    toggleTankCleaningFields();
    toggleDisposalFields();
}

function toggleTankCleaningFields() {
    let cleaned = $("#TankCleaned").val();
    $("#tankCleaningFields").toggle(cleaned === "Yes");
}

function toggleDisposalFields() {
    let method = $("#DisposalMethod").val();
    $("#shoreDisposalFields").toggle(method === "Shore");
    $("#seaDisposalFields").toggle(method === "Sea");
}

function setupFormValidation() {
    $.validator.addMethod("validPosition", function (value) {
        if (!value) return true;
        return /^\d{1,2}\s*DEG\s*\d{1,2}\s*MIN\s*[NS],\s*\d{1,3}\s*DEG\s*\d{1,2}\s*MIN\s*[EW]$/i.test(value);
    }, "Format: XX DEG XX MIN N/S, XXX DEG XX MIN E/W");

    $("#EditCodeCForm").validate({
        rules: {
            EntryDate: "required",
            OperationType: "required",
            IdentityOfTanks: "required",
            OilResidueQuantity: { required: true, number: true },
            PositionOfVesselDisposal: {
                required: () => $("#DisposalMethod").val() === "Sea",
                validPosition: true
            },
            ShipSpeed: {
                required: () => $("#DisposalMethod").val() === "Sea",
                number: true
            },
            DistanceFromShore: {
                required: () => $("#DisposalMethod").val() === "Sea",
                number: true
            },
            PortName: {
                required: () => $("#DisposalMethod").val() === "Shore"
            },
            CleaningMethod: {
                required: () => $("#TankCleaned").val() === "Yes"
            },
            DisposalFacilityName: {
                required: () => $("#DisposalMethod").val() === "Shore"
            }
        },
        messages: {
            EntryDate: "Please enter a valid date",
            OperationType: "Please select an operation type",
            OilResidueQuantity: "Please enter a valid number",
            ShipSpeed: "Please enter a valid speed",
            DistanceFromShore: "Please enter a valid distance"
        }
    });
}

function submitForm() {
    let formData = {
        Id: $('#Id').val(),
        UserId: userId,
        EntryDate: $('#EntryDate').val(),
        OperationType: $('#OperationType').val(),
        TankCleaned: $('#TankCleaned').val() === "Yes",
        IdentityOfTanks: $('#IdentityOfTanks').val(),
        OilResidueQuantity: parseFloat($('#OilResidueQuantity').val()) || 0,
        DisposalMethod: $('#DisposalMethod').val() || null,
        DisposalStartTime: $('#DisposalStartTime').val() || null,
        DisposalEndTime: $('#DisposalEndTime').val() || null,
        PositionOfVesselDisposal: $('#PositionOfVesselDisposal').val() || null,
        ShipSpeed: parseFloat($('#ShipSpeed').val()) || null,
        DistanceFromShore: parseFloat($('#DistanceFromShore').val()) || null,
        PortName: $('#PortName').val() || null,
        DisposalFacilityName: $('#DisposalFacilityName').val() || null,
        CleaningMethod: $('#CleaningMethod').val() || null,
        CleaningStartTime: $('#CleaningStartTime').val() || null,
        CleaningEndTime: $('#CleaningEndTime').val() || null,
        OilType: $('#OilType').val() || null,
        RetentionStartTime: $('#RetentionStartTime').val() || null,
        RetentionEndTime: $('#RetentionEndTime').val() || null,
        TransferStartTime: $('#TransferStartTime').val() || null,
        TransferEndTime: $('#TransferEndTime').val() || null,
        TransferDestination: $('#TransferDestination').val() || null
    };

    $.ajax({
        url: "/ORB1/CodeC/UpdateCodeC",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(formData),
        success: function (response) {
            // Reset the form and apply UI changes before showing the alert
            $('#EditCodeCForm')[0].reset();
            resetFormDate();
            hideOperationFields();

            // Show message in an alert popup and redirect when the user clicks "OK"
            alert(response.message);

            const { pageNumber, pageSize } = getQueryParams();
            // Redirect only after the user clicks "OK"
            window.location.href = `/ORB1/CodeC/GetCodeCData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
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

function hideOperationFields() {
    $('#disposalFields, #retentionFields, #transferFields, #tankCleaningFields, #shoreDisposalFields, #seaDisposalFields').hide();
}

// Function to get query parameters from the URL
function getQueryParams() {
    const urlParams = new URLSearchParams(window.location.search);
    const pageNumber = urlParams.get('pageNumber') || 1;
    const pageSize = urlParams.get('pageSize') || 10;

    return { pageNumber, pageSize };
}