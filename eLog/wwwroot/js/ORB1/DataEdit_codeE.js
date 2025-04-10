$(document).ready(function () {
    initializeFormState();
    setupEventListeners();
});

function initializeFormState() {
    // Show appropriate fields based on initial values
    const initialAction = $("#AutomaticDischargeType").val();
    if (initialAction === "Overboard") {
        $('#OverboardFields').show();
        $('#TransferFields').hide();
    } else if (initialAction === "Transfer") {
        $('#OverboardFields').hide();
        $('#TransferFields').show();
    } else {
        $('#OverboardFields').hide();
        $('#TransferFields').hide();
    }
}

function setupEventListeners() {

    $("#cancelButton").click(function () {
        const { pageNumber, pageSize } = getQueryParams();
        var basePath = document.querySelector('base')?.getAttribute('href') || '/';
        window.location.href = `${basePath}/ORB1/CodeE/GetCodeEData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    });

    $("#EditCodeEForm").submit(function (event) {
        event.preventDefault();
        if ($(this).valid()) {
            submitForm();
        }
    });
}

var basePath = document.querySelector('base')?.getAttribute('href') || '';

function submitForm() {
    let formData = {
        Id: $('#Id').val(),
        UserId: userId,
        EntryDate: $('#EntryDate').val(),
        AutomaticDischargeType: $('#AutomaticDischargeType').val() || null,
        OverboardPositionShipStart: $('#OverboardPositionShipStart').val() || null,
        OverboardTimeSwitching: $('#OverboardTimeSwitching').val() || null,
        TransferTimeSwitching: $('#TransferTimeSwitching').val() || null,
        TransferTankfrom: $('#TransferTankfrom').val() || null,
        TransferTankTo: $('#TransferTankTo').val() || null,
        TimeBackToManual: $('#TimeBackToManual').val() || null
    };

    $.ajax({
        url: basePath + "/ORB1/CodeE/UpdateCodeE",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(formData),
        success: function (response) {
            $('#EditCodeEForm')[0].reset();
            resetFormDate();
            // Hide dependent fields
            $('#OverboardFields, #TransferFields').hide();
            alert(response.message);

            const { pageNumber, pageSize } = getQueryParams();
            window.location.href = `${basePath}/ORB1/CodeE/GetCodeEData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
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

function getQueryParams() {
    const urlParams = new URLSearchParams(window.location.search);
    const pageNumber = urlParams.get('pageNumber') || 1;
    const pageSize = urlParams.get('pageSize') || 10;
    return { pageNumber, pageSize };
}