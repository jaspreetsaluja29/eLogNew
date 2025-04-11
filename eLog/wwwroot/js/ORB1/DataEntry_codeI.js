$(document).ready(function () {
    // Set default date in the format YYYY-MM-DD
    let today = new Date();
    let formattedDate = today.getFullYear() + '-' +
        String(today.getMonth() + 1).padStart(2, '0') + '-' +
        String(today.getDate()).padStart(2, '0');

    // Set the formatted date as the value of the input
    $('#EntryDate').val(formattedDate);

    // Hide all fields except Date & Ballasting or Cleaning at the start
    $('#DebunkeringFields, #SealingFields, #BreakingFields').hide();

    // Show respective fields based on selection
    $('#SelectType').change(function () {
        var selectedValue = $(this).val();
        if (selectedValue === 'Debunkering') {
            $('#DebunkeringFields').show();
            $('#SealingFields').hide();
            $('#BreakingFields').hide();
        } else if (selectedValue === 'Sealing of Valve') {
            $('#DebunkeringFields').hide();
            $('#SealingFields').show();
            $('#BreakingFields').hide();
        } else if (selectedValue === 'Breaking of Seal') {
            $('#DebunkeringFields').hide();
            $('#SealingFields').hide();
            $('#BreakingFields').show();
        } else {
            $('#DebunkeringFields, #SealingFields, #BreakingFields').hide();
        }
    });


    // Save form data via AJAX
    $('#saveButton').click(function (event) {
        event.preventDefault();

        var basePath = document.querySelector('base')?.getAttribute('href') || ''; // Get base path

        var formData = {
            UserId: userId, // Include the UserID
            EntryDate: $('#EntryDate').val(),
            SelectType: $('#SelectType').val() || null,

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
            url: basePath + "/ORB1/CodeI/CreateCodeI", // Corrected URL string
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

                $('#DebunkeringFields, #SealingFields, #BreakingFields').hide();
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
        $('#DebunkeringFields, #SealingFields, #BreakingFields').hide();
        $('#message').html('');
    });

    // Cancel button - Redirect to another page
    $("#cancelButton").click(function () {
        const { pageNumber, pageSize } = getQueryParams();
        // Redirect only after the user clicks "OK"
        var basePath = document.querySelector('base')?.getAttribute('href') || '/';
        window.location.href = `${basePath}/ORB1/CodeI/GetCodeIData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    });
});

// Function to get query parameters from the URL
function getQueryParams() {
    const urlParams = new URLSearchParams(window.location.search); // Get the query string from the URL
    const pageNumber = urlParams.get('pageNumber') || 1; // Default to 1 if not provided
    const pageSize = urlParams.get('pageSize') || 10; // Default to 10 if not provided

    return { pageNumber, pageSize };
}
