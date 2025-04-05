$(document).ready(function () {
    setupEventListeners();
});


function setupEventListeners() {
    // Handle cancel button click
    $("#cancelButton").click(function () {
        if (confirm("Are you sure you want to cancel? Any unsaved changes will be lost.")) {
            const { pageNumber, pageSize } = getQueryParams();
            var basePath = document.querySelector('base')?.getAttribute('href') || '/';
            window.location.href = `${basePath}/ORB1/CodeF/GetCodeFData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
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
    $("#EditCodeFForm").submit(function (event) {
        event.preventDefault();
        if ($(this).valid()) {
            submitForm();
        }
    });
}

var basePath = document.querySelector('base')?.getAttribute('href') || '/';
function submitForm() {
    let actionType = $("#actionType").val() || "Pending"; // Set a default value if empty

    let formData = {
        Id: $('#Id').val(),
        UserId: userId,
        EntryDate: $('#EntryDate').val(),
        TimeFailure: $('#TimeFailure').val() || null,
        TimeOperational: $('#TimeOperational').val() || null,
        ReasonFailure: $('#ReasonFailure').val() || null,
        Comments: $('#Comments').val(),
        StatusName: actionType // Ensure StatusName always has a value
    };

    $.ajax({
        url: basePath + "/ORB1/CodeF/ApproverUpdateCodeF",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(formData),
        success: function (response) {
            $('#EditCodeFForm')[0].reset();
            resetFormDate();
            alert(response.message);
            const { pageNumber, pageSize } = getQueryParams();
            var basePath = document.querySelector('base')?.getAttribute('href') || '/';
            window.location.href = `${basePath}/ORB1/CodeF/GetCodeFData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
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

// Function to get query parameters from the URL
function getQueryParams() {
    const urlParams = new URLSearchParams(window.location.search); // Get the query string from the URL
    const pageNumber = urlParams.get('pageNumber') || 1; // Default to 1 if not provided
    const pageSize = urlParams.get('pageSize') || 10; // Default to 10 if not provided

    return { pageNumber, pageSize };
}