$(document).ready(function () {
    initializeFormState();
    setupEventListeners();
    setupFormValidation();
    loadTanks(); // Call the loadTanks function when the page loads
});

function initializeFormState() {
    toggleOperationFields();
}

function setupEventListeners() {
    $("#OperationType").change(toggleOperationFields);
    $("#TankCleaned").change(toggleTankCleaningFields);
    $("#DisposalMethod").change(toggleDisposalFields);

    // Handle change event to update capacity when selecting a tank
    $("#WeeklyIdentityOfTanks").change(function () {
        let selectedCapacity = $(this).find(":selected").data("capacity");
        $("#WeeklyCapacityOfTanks").val(selectedCapacity || "");
    });

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

function loadTanks() {
    // Get the record ID if we're on an edit page
    const recordId = $("#Id").val();
    const isEditMode = recordId && recordId !== "0";

    $.ajax({
        url: "/ORB1/CodeC/GetTanks",
        type: "GET",
        dataType: "json",
        success: function (data) {
            if (!data || data.length === 0) {
                return;
            }

            let weeklySelect = $("#WeeklyIdentityOfTanks");
            if (weeklySelect.length === 0) {
                return;
            }

            // Clear existing options
            weeklySelect.empty();

            // Add a default first option
            weeklySelect.append('<option value="">Select Tank</option>');

            // Add tanks to the dropdown
            data.forEach(tank => {
                const tankId = tank.tankIdentification || "";
                const capacity = tank.volumeCapacity || "";
                if (!tankId) {
                    return;
                }
                let option = $("<option>")
                    .val(tankId)
                    .text(tankId)
                    .attr("data-capacity", capacity);
                weeklySelect.append(option);
            });

            // If this is an edit page, load existing data from the database
            if (isEditMode) {
                loadExistingTankData(recordId);
            } else {
                // Get the selected tank ID from Razor ViewBag for new records
                let selectedTankId = "@ViewBag.TankId";
                // Set the selected tank
                if (selectedTankId) {
                    weeklySelect.val(selectedTankId);
                    let selectedCapacity = weeklySelect.find(":selected").data("capacity");
                    $("#WeeklyCapacityOfTanks").val(selectedCapacity);
                }
                // Trigger change event to ensure capacity fields are populated
                weeklySelect.trigger('change');
            }
        },
        error: function (xhr, status, error) {
            // Silently fail
        }
    });
}

function loadExistingTankData(recordId) {
    $.ajax({
        url: `/ORB1/CodeC/GetCodeCById/${recordId}`,
        type: "GET",
        dataType: "json",
        success: function (data) {
            if (!data) {
                return;
            }

            // Set the tank dropdown to the existing value
            if (data.weeklyIdentityOfTanks) {
                $("#WeeklyIdentityOfTanks").val(data.weeklyIdentityOfTanks);

                // If the tank is found in the dropdown, get its capacity
                let selectedOption = $("#WeeklyIdentityOfTanks option:selected");
                if (selectedOption.length > 0) {
                    let capacity = selectedOption.data("capacity");
                    $("#WeeklyCapacityOfTanks").val(capacity || data.weeklyCapacityOfTanks || "");
                } else {
                    // If the tank isn't in the dropdown (maybe it was deleted), keep the existing capacity
                    $("#WeeklyCapacityOfTanks").val(data.weeklyCapacityOfTanks || "");
                }
            }

            // Trigger change event to ensure any dependent fields are updated
            $("#WeeklyIdentityOfTanks").trigger('change');
        },
        error: function (xhr, status, error) {
            console.error("Error loading existing tank data:", error);
        }
    });
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
            },
            WeeklyIdentityOfTanks: "required",
            WeeklyCapacityOfTanks: { required: true, number: true }
        },
        messages: {
            EntryDate: "Please enter a valid date",
            OperationType: "Please select an operation type",
            OilResidueQuantity: "Please enter a valid number",
            ShipSpeed: "Please enter a valid speed",
            DistanceFromShore: "Please enter a valid distance",
            WeeklyIdentityOfTanks: "Please select a tank",
            WeeklyCapacityOfTanks: "Please enter a valid capacity"
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
        TransferDestination: $('#TransferDestination').val() || null,
        WeeklyIdentityOfTanks: $('#WeeklyIdentityOfTanks').val() || null,
        WeeklyCapacityOfTanks: parseFloat($('#WeeklyCapacityOfTanks').val()) || null
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