﻿$(document).ready(function () {
    // Set default date in the format YYYY-MM-DD for relevant date fields
    let today = new Date();
    let formattedDate = today.getFullYear() + '-' +
        String(today.getMonth() + 1).padStart(2, '0') + '-' +
        String(today.getDate()).padStart(2, '0');

    // Set default dates for relevant fields
    $('#pilotProjectStartDate').val(formattedDate);
    $('#lastEntryDate').val(formattedDate);
    $('#lastApprovedDate').val(formattedDate);
    $('#subscriptionStartDate').val(formattedDate);

    // Save form data via AJAX
    $('#saveButton').click(function (event) {
        event.preventDefault();

        var basePath = document.querySelector('base')?.getAttribute('href') || ''; // Get base path

        var formData = {
            CompanyName: $('#companyName').val(),
            ManagerName: $('#managerName').val(),
            OwnerName: $('#ownerName').val(),
            Address: $('#address').val(),
            Email: $('#email').val(),
            ContactNumber: $('#contactNumber').val(),
            PICDetails: $('#picDetails').val(),
            PilotProjectStartDate: $('#pilotProjectStartDate').val(),
            VesselName: $('#vesselName').val() || null,
            IMONumber: parseInt($('#imoNumber').val()) || null,
            ActiveId: parseInt($('#activeId').val()) || null,
            Flag: $('#flag').val() || null,
            LastEntryDate: $('#lastEntryDate').val() || null,
            LastApprovedDate: $('#lastApprovedDate').val() || null,
            SubscriptionStartDate: $('#subscriptionStartDate').val() || null,
            TotalActiveeLog: parseInt($('#totalActiveeLog').val()) || null
        };

        $.ajax({
            url: basePath + "/ISMCompanyDetails/Create",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(formData),
            success: function (response) {
                $('#message').html('<div class="alert alert-success">' + response.message + '</div>');

                // Reset form fields after successful save
                $('#DataEntryISMCompanyDetailsForm')[0].reset();

                // Reset date fields with today's date
                let today = new Date();
                let formattedDate = today.getFullYear() + '-' +
                    String(today.getMonth() + 1).padStart(2, '0') + '-' +
                    String(today.getDate()).padStart(2, '0');

                $('#pilotProjectStartDate').val(formattedDate);
                $('#lastEntryDate').val(formattedDate);
                $('#lastApprovedDate').val(formattedDate);
                $('#subscriptionStartDate').val(formattedDate);
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", xhr.responseText);
                $('#message').html('<div class="alert alert-danger">Error: ' +
                    (xhr.responseJSON?.message || xhr.responseText || 'An error occurred') + '</div>');
            }
        });
    });

    //// Reset form fields
    //$('#resetButton').click(function () {
    //    if (confirm('Are you sure you want to reset the form? All entered data will be lost.')) {
    //        $('#DataEntryISMCompanyDetailsForm')[0].reset();

    //        // Reset date fields with today's date
    //        let today = new Date();
    //        let formattedDate = today.getFullYear() + '-' +
    //            String(today.getMonth() + 1).padStart(2, '0') + '-' +
    //            String(today.getDate()).padStart(2, '0');

    //        $('#pilotProjectStartDate').val(formattedDate);
    //        $('#lastEntryDate').val(formattedDate);
    //        $('#lastApprovedDate').val(formattedDate);
    //        $('#subscriptionStartDate').val(formattedDate);

    //        $('#message').html('');
    //    }
    //});

    // Reset form fields
    $('#resetButton').click(function () {
        $('#DataEntryISMCompanyDetailsForm')[0].reset();

        setTimeout(() => {
            // Reset date fields with today's date
            let today = new Date();
            let formattedDate = today.getFullYear() + '-' +
                String(today.getMonth() + 1).padStart(2, '0') + '-' +
                String(today.getDate()).padStart(2, '0');

            $('#pilotProjectStartDate').val(formattedDate);
            $('#lastEntryDate').val(formattedDate);
            $('#lastApprovedDate').val(formattedDate);
            $('#subscriptionStartDate').val(formattedDate);

            $('#message').html('');
        }, 100); // Delay to let reset() take effect first
    });



    // Cancel button - Redirect to another page
    $("#cancelButton").click(function () {
        const { pageNumber, pageSize } = getQueryParams();
        // Redirect only after the user clicks "OK"
        var basePath = document.querySelector('base')?.getAttribute('href') || '/';
        window.location.href = `${basePath}/ISMCompanyDetails/GetISMCompanyDetails?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    });
});

// Function to get query parameters from the URL (if needed for pagination)
function getQueryParams() {
    const urlParams = new URLSearchParams(window.location.search);
    const pageNumber = urlParams.get('pageNumber') || 1;
    const pageSize = urlParams.get('pageSize') || 10;

    return { pageNumber, pageSize };
}