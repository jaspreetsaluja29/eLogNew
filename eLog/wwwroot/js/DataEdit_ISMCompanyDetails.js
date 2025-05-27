$(document).ready(function () {
    // Get from localStorage
    $('#userId').val(localStorage.getItem('userId'));
    $('#userName').val(localStorage.getItem('userName'));
    $('#userRoleName').val(localStorage.getItem('userRoleName'));
    $('#jobRank').val(localStorage.getItem('jobRank'));
    setupEventListeners();
});

function setupEventListeners() {
    // Attach click event to the update button, not the form
    $('#updateButton').click(function (event) {
        event.preventDefault();
        submitForm();
    });

    // Also handle form submission
    $('#EditISMCompanyDetailsForm').submit(function (event) {
        event.preventDefault();
        submitForm();
    });

    // Cancel button - Redirect to another page
    $("#cancelButton").click(function () {
        const { pageNumber, pageSize } = getQueryParams();
        var basePath = document.querySelector('base')?.getAttribute('href') || '/';
        window.location.href = `${basePath}/ISMCompanyDetails/GetISMCompanyDetails?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    });
}

var basePath = document.querySelector('base')?.getAttribute('href') || '';

function submitForm() {
    // Match the case of HTML element IDs exactly - using ASP.NET tag helpers
    // convert attribute names to proper case (e.g., asp-for="CompanyName" becomes id="CompanyName")
    var formData = {
        // Corrected CompanyId (was Companyid)
        CompanyId: $('#CompanyId').val(),
        CompanyName: $('#CompanyName').val(),
        ManagerName: $('#ManagerName').val(),
        OwnerName: $('#OwnerName').val(),
        Address: $('#Address').val(),
        Email: $('#Email').val(),
        ContactNumber: $('#ContactNumber').val(),
        PICDetails: $('#PICDetails').val(),
        PilotProjectStartDate: $('#PilotProjectStartDate').val(),
        VesselName: $('#VesselName').val() || null,
        IMONumber: $('#IMONumber').val() ? parseInt($('#IMONumber').val()) : null,
        ActiveId: $('#ActiveId').val() ? parseInt($('#ActiveId').val()) : null,
        Flag: $('#Flag').val() || null,
        LastEntryDate: $('#LastEntryDate').val() || null,
        LastApprovedDate: $('#LastApprovedDate').val() || null,
        SubscriptionStartDate: $('#SubscriptionStartDate').val() || null,
        TotalActiveeLog: $('#TotalActiveeLog').val() ? parseInt($('#TotalActiveeLog').val()) : null,
        // Add user information
        UserId: $('#userId').val() || localStorage.getItem('userId'),
        UserName: $('#userName').val() || localStorage.getItem('userName'),
        UserRoleName: $('#userRoleName').val() || localStorage.getItem('userRoleName')
        JobRank: $('#jobRank').val() || localStorage.getItem('jobRank')
    };

    // Debug information
    console.log("Sending data:", JSON.stringify(formData));

    $.ajax({
        url: basePath + "/ISMCompanyDetails/Update",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(formData),
        success: function (response) {
            // Show message in an alert popup
            alert(response.message);
            const { pageNumber, pageSize } = getQueryParams();
            // Redirect after the user clicks "OK"
            var basePath = document.querySelector('base')?.getAttribute('href') || '/';
            window.location.href = `${basePath}/ISMCompanyDetails/GetISMCompanyDetails?pageNumber=${pageNumber}&pageSize=${pageSize}`;
        },
        error: function (xhr, status, error) {
            console.error("AJAX Error:", status, error);
            console.error("Response:", xhr.responseText);
            // Show error message
            $('#message').html('<div class="alert alert-danger">Error: ' + xhr.responseText + '</div>');
        }
    });
}

// Function to get query parameters from the URL for pagination
function getQueryParams() {
    const urlParams = new URLSearchParams(window.location.search);
    const pageNumber = urlParams.get('pageNumber') || 1;
    const pageSize = urlParams.get('pageSize') || 10;
    return { pageNumber, pageSize };
}