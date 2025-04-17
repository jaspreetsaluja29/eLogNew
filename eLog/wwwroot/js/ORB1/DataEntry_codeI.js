// Define base path globally
var basePath = document.querySelector('base')?.getAttribute('href') || '';

$(document).ready(function () {
    // Set default date in the format YYYY-MM-DD
    let today = new Date();
    let formattedDate = today.getFullYear() + '-' +
        String(today.getMonth() + 1).padStart(2, '0') + '-' +
        String(today.getDate()).padStart(2, '0');

    // Set the formatted date as the value of the input
    $('#EntryDate').val(formattedDate);

    // Hide all fields except Date & Ballasting or Cleaning at the start
    $('#WeeklyInventoryFields, #DebunkeringFields, #SealingFields, #BreakingFields').hide();

    // Show respective fields based on selection
    $('#SelectType').change(function () {
        var selectedValue = $(this).val();
        setFieldVisibility(selectedValue);
    });

    // Populate capacity when tank is selected in Weekly Inventory
    $('#WeeklyInventoryTanks').change(function () {
        var capacity = $(this).find('option:selected').data('capacity');
        $('#WeeklyInventoryCapacity').val(capacity ? capacity : '');
    });

    // Save form data via AJAX
    $('#saveButton').click(function (event) {
        event.preventDefault();

        var formData = {
            UserId: userId, // Include the UserID
            EntryDate: $('#EntryDate').val(),
            SelectType: $('#SelectType').val() || null,

            // Weekly Inventory
            WeeklyInventoryTanks: $('#WeeklyInventoryTanks').val() || null,
            WeeklyInventoryCapacity: parseFloat($('#WeeklyInventoryCapacity').val()) || null,
            WeeklyInventoryRetained: parseFloat($('#WeeklyInventoryRetained').val()) || null,

            // Debunkering
            DebunkeringQuantity: parseFloat($('#DebunkeringQuantity').val()) || null,
            DebunkeringGrade: $('#DebunkeringGrade').val() || null,
            DebunkeringSulphurContent: $('#DebunkeringSulphurContent').val() || null,
            DebunkeringFrom: $('#DebunkeringFrom').val() || null,
            DebunkeringQuantityRetained: parseFloat($('#DebunkeringQuantityRetained').val()) || null,
            DebunkeringTo: $('#DebunkeringTo').val() || null,
            DebunkeringPortFacility: $('#DebunkeringPortFacility').val() || null,
            DebunkeringStartDateTime: $('#DebunkeringStartDateTime').val() || null,
            DebunkeringStopDateTime: $('#DebunkeringStopDateTime').val() || null,

            // Sealing of Valve
            ValveName: $('#ValveName').val() || null,
            ValveNo: $('#ValveNo').val() || null,
            ValveAssociatedEquipment: $('#ValveAssociatedEquipment').val() || null,
            ValveSealNo: $('#ValveSealNo').val() || null,

            // Breaking of Seal
            BreakingValveName: $('#BreakingValveName').val() || null,
            BreakingValveNo: $('#BreakingValveNo').val() || null,
            BreakingAssociatedEquipment: $('#BreakingAssociatedEquipment').val() || null,
            BreakingReason: $('#BreakingReason').val() || null,
            BreakingSealNo: $('#BreakingSealNo').val() || null
        };

        $.ajax({
            url: basePath + "/ORB1/CodeI/CreateCodeI",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(formData),
            success: function (response) {
                $('#message').html('<div class="alert alert-success">' + response.message + '</div>');

                // Reset form fields after successful save
                $('#DataEntryCodeIForm')[0].reset();

                // Reset date field with today's date
                let today = new Date();
                let formattedDate = today.getFullYear() + '-' +
                    String(today.getMonth() + 1).padStart(2, '0') + '-' +
                    String(today.getDate()).padStart(2, '0');
                $('#EntryDate').val(formattedDate);

                $('#WeeklyInventoryFields, #DebunkeringFields, #SealingFields, #BreakingFields').hide();
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", xhr.responseText);
                $('#message').html('<div class="alert alert-danger">Error: ' + xhr.responseText + '</div>');
            }
        });
    });

    // Reset form fields
    $('#resetButton').click(function () {
        $('#DataEntryCodeIForm')[0].reset();
        $('#WeeklyInventoryFields, #DebunkeringFields, #SealingFields, #BreakingFields').hide();
        $('#message').html('');

        // Reset date field with today's date
        $('#EntryDate').val(formattedDate);
    });

    // Cancel button - Redirect to another page
    $("#cancelButton").click(function () {
        const { pageNumber, pageSize } = getQueryParams();
        window.location.href = `${basePath}/ORB1/CodeI/GetCodeIData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    });

    // Load tanks from the server
    loadTanks();
});

// Function to get query parameters from the URL
function getQueryParams() {
    const urlParams = new URLSearchParams(window.location.search);
    const pageNumber = urlParams.get('pageNumber') || 1; // Default to 1 if not provided
    const pageSize = urlParams.get('pageSize') || 10; // Default to 10 if not provided

    return { pageNumber, pageSize };
}

// Helper function for field visibility
function setFieldVisibility(selectedValue) {
    // Hide all sections first
    $('#WeeklyInventoryFields, #DebunkeringFields, #SealingFields, #BreakingFields').hide();

    // Show only the selected section
    switch (selectedValue) {
        case 'Weekly Inventory of IOPP Tank 3.3':
            $('#WeeklyInventoryFields').show();
            break;
        case 'Debunkering':
            $('#DebunkeringFields').show();
            break;
        case 'Sealing of Valve':
            $('#SealingFields').show();
            break;
        case 'Breaking of Seal':
            $('#BreakingFields').show();
            break;
        default:
            // Don't show any fields by default
            break;
    }
}

// Function to load tanks from the server
function loadTanks() {
    $.ajax({
        url: basePath + "/ORB1/CodeI/GetTanks",
        type: "GET",
        dataType: "json",
        success: function (data) {
            if (!data || data.length === 0) {
                return;
            }

            // Get references to all tank select elements
            let weeklyInventoryTanks = $("#WeeklyInventoryTanks");

            // Check if the main element exists
            if (weeklyInventoryTanks.length === 0) {
                return;
            }

            // Clear existing options
            weeklyInventoryTanks.empty();

            // Add a default first option
            weeklyInventoryTanks.append('<option value="">Select Tank</option>');

            // Add tanks to all dropdowns
            data.forEach(tank => {
                const tankId = tank.tankIdentification || "";
                const capacity = tank.volumeCapacity || "";

                if (!tankId) {
                    return;
                }

                let option1 = $("<option>")
                    .val(tankId)
                    .text(tankId)
                    .attr("data-capacity", capacity);

                weeklyInventoryTanks.append(option1.clone());
            });

            // Trigger change event to ensure capacity fields are populated if a default is selected
            weeklyInventoryTanks.trigger('change');

            // Set the field visibility based on current selection
            setFieldVisibility($('#SelectType').val());
        },
        error: function (xhr, status, error) {
            console.error("Failed to load tanks:", error);
        }
    });
}