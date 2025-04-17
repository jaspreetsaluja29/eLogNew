$(document).ready(function () {
    initializeFormState();
    setupEventListeners();
});

function initializeFormState() {
    // Show appropriate fields based on initial values
    const initialAction = $("#SelectType").val();
    if (initialAction === "Debunkering") {
        $('#DebunkeringFields').show();
        $('#SealingFields').hide();
        $('#BreakingFields').hide();
        $('#WeeklyInventoryFields').hide();
    } else if (initialAction === "Sealing of Valve") {
        $('#DebunkeringFields').hide();
        $('#SealingFields').show();
        $('#BreakingFields').hide();
        $('#WeeklyInventoryFields').hide();
    } else if (initialAction === "Breaking of Seal") {
        $('#DebunkeringFields').hide();
        $('#SealingFields').hide();
        $('#BreakingFields').show();
        $('#WeeklyInventoryFields').hide();
    } else if (initialAction === "Weekly Inventory of IOPP Tank 3.3") {
        $('#DebunkeringFields').hide();
        $('#SealingFields').hide();
        $('#BreakingFields').hide();
        $('#WeeklyInventoryFields').show();
    } else {
        $('#WeeklyInventoryFields, #DebunkeringFields, #SealingFields, #BreakingFields').hide();
    }
}

function setupEventListeners() {

    $("#cancelButton").click(function () {
        const { pageNumber, pageSize } = getQueryParams();
        var basePath = document.querySelector('base')?.getAttribute('href') || '/';
        window.location.href = `${basePath}/ORB1/CodeI/GetCodeIData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    });

    $("#EditCodeIForm").submit(function (event) {
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
        url: basePath + "/ORB1/CodeI/UpdateCodeI",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(formData),
        success: function (response) {
            $('#EditCodeIForm')[0].reset();
            resetFormDate();
            $('#WeeklyInventoryFields, #DebunkeringFields, #SealingFields, #BreakingFields').hide();

            alert(response.message);

            const { pageNumber, pageSize } = getQueryParams();
            window.location.href = `${basePath}/ORB1/CodeI/GetCodeIData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
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