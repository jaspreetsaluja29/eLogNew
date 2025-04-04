$(document).ready(function () {
    // Set default date in the format YYYY-MM-DD
    let today = new Date();
    let formattedDate = today.getFullYear() + '-' +
        String(today.getMonth() + 1).padStart(2, '0') + '-' +
        String(today.getDate()).padStart(2, '0');

    // Set the formatted date as the value of the input
    $('#EntryDate').val(formattedDate);

    // Save form data via AJAX
    $('#saveButton').click(function (event) {
        event.preventDefault();

        var basePath = document.querySelector('base')?.getAttribute('href') || ''; // Get base path

        var formData = {
            UserId: userId, // Include the UserID
            EntryDate: $('#EntryDate').val(),
            TimeofOccurrence: $('#TimeofOccurrence').val() || null,
            PositionofShip: $('#PositionofShip').val() || null,
            ApproxQuantity: $('#ApproxQuantity').val() || null,
            TypeofOil: $('#TypeofOil').val() || null,
            Reasons: $('#Reasons').val() || null
        };

        $.ajax({
            url: basePath + "/ORB1/CodeG/CreateCodeG", // Corrected URL string
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(formData),
            success: function (response) {
                $('#message').html('<div class="alert alert-success">' + response.message + '</div>');

                // Reset form fields after successful save
                $('#DataEntryCodeGForm')[0].reset();

                // Reset date field with today's date
                let today = new Date();
                let formattedDate = today.getFullYear() + '-' +
                    String(today.getMonth() + 1).padStart(2, '0') + '-' +
                    String(today.getDate()).padStart(2, '0');
                $('#EntryDate').val(formattedDate);
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", xhr.responseText);
                $('#message').html('<div class="alert alert-danger">Error: ' + xhr.responseText + '</div>');
            }
        });
    });


    // Reset form fields
    $('#resetButton').click(function () {
        $('#DataEntryCodeGForm')[0].reset();
        $('#message').html('');
    });

    // Cancel button - Redirect to another page
    $("#cancelButton").click(function () {
        const { pageNumber, pageSize } = getQueryParams();
        // Redirect only after the user clicks "OK"
        var basePath = document.querySelector('base')?.getAttribute('href') || '/';
        window.location.href = `${basePath}/ORB1/CodeG/GetCodeGData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    });
});

// Function to get query parameters from the URL
function getQueryParams() {
    const urlParams = new URLSearchParams(window.location.search); // Get the query string from the URL
    const pageNumber = urlParams.get('pageNumber') || 1; // Default to 1 if not provided
    const pageSize = urlParams.get('pageSize') || 10; // Default to 10 if not provided

    return { pageNumber, pageSize };
}
