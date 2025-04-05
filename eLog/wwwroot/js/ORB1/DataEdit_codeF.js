$(document).ready(function () {
    setupEventListeners();
});

function setupEventListeners() {

    $("#cancelButton").click(function () {
        const { pageNumber, pageSize } = getQueryParams();
        var basePath = document.querySelector('base')?.getAttribute('href') || '/';
        window.location.href = `${basePath}/ORB1/CodeF/GetCodeFData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    });

    $("#EditCodeFForm").submit(function (event) {
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
        TimeFailure: $('#TimeFailure').val() || null,
        TimeOperational: $('#TimeOperational').val() || null,
        ReasonFailure: $('#ReasonFailure').val() || null
    };

    $.ajax({
        url: basePath + "/ORB1/CodeF/UpdateCodeF",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(formData),
        success: function (response) {
            $('#EditCodeFForm')[0].reset();
            resetFormDate();

            alert(response.message);

            const { pageNumber, pageSize } = getQueryParams();
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

function getQueryParams() {
    const urlParams = new URLSearchParams(window.location.search);
    const pageNumber = urlParams.get('pageNumber') || 1;
    const pageSize = urlParams.get('pageSize') || 10;
    return { pageNumber, pageSize };
}