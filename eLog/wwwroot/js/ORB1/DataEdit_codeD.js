﻿$(document).ready(function () {
    initializeFormState();
    setupEventListeners();
    setupFormValidation();
});

function initializeFormState() {
    // Show appropriate fields based on initial values
    const initialMethod = $("#MethodDischargeTransferDisposal").val();
    if (initialMethod === "Through 15 PPM Equipment") {
        $("#EquipmentFields").show();
        $("#ReceptionFields, #SlopFields").hide();
    } else if (initialMethod === "To Reception facilities") {
        $("#ReceptionFields").show();
        $("#EquipmentFields, #SlopFields").hide();
    } else if (initialMethod === "To Slop, holding or other tank") {
        $("#SlopFields").show();
        $("#EquipmentFields, #ReceptionFields").hide();
    }

    const initialAction = $("#SlopTransferredFrom").val();
    if (initialAction === "E/RM Bilge Wells") {
        $('#SlopTransferredFromField').hide();
    } else {
        $('#SlopTransferredFromField').show();
    }
}

function setupEventListeners() {
    $("#MethodDischargeTransferDisposal").change(toggleFields);

    $("#cancelButton").click(function () {
        const { pageNumber, pageSize } = getQueryParams();
        var basePath = document.querySelector('base')?.getAttribute('href') || '/';
        window.location.href = `${basePath}/ORB1/CodeD/GetCodeDData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    });

    $("#EditCodeDForm").submit(function (event) {
        event.preventDefault();
        if ($(this).valid()) {
            submitForm();
        }
    });
}

function toggleFields() {
    let selection = $("#MethodDischargeTransferDisposal").val();
    $("#EquipmentFields").toggle(selection === "Through 15 PPM Equipment");
    $("#ReceptionFields").toggle(selection === "To Reception facilities");
    $("#SlopFields").toggle(selection === "To Slop, holding or other tank");
}

function setupFormValidation() {
    $.validator.addMethod("validPosition", function (value) {
        if (!value) return true;
        return /^\d{1,2}\s*DEG\s*\d{1,2}\s*MIN\s*[NS],\s*\d{1,3}\s*DEG\s*\d{1,2}\s*MIN\s*[EW]$/i.test(value);
    }, "Format: XX DEG XX MIN N/S, XX DEG XX MIN E/W");

    $("#EditCodeDForm").validate({
        rules: {
            EntryDate: "required",
            MethodDischargeTransferDisposal: "required",
            EquipmentQuantity: {
                required: () => $("#MethodDischargeTransferDisposal").val() === "Through 15 PPM Equipment",
                number: true
            },
            EquipmentPositionStart: {
                required: () => $("#MethodDischargeTransferDisposal").val() === "Through 15 PPM Equipment",
                validPosition: true
            },
            EquipmentPositionStop: {
                required: () => $("#MethodDischargeTransferDisposal").val() === "Through 15 PPM Equipment",
                validPosition: true
            },
            ReceptionQuantity: {
                required: () => $("#MethodDischargeTransferDisposal").val() === "To Reception facilities",
                number: true
            },
            SlopQuantity: {
                required: () => $("#MethodDischargeTransferDisposal").val() === "To Slop, holding or other tank",
                number: true
            }
        },
        messages: {
            EntryDate: "Please enter a valid date",
            MethodDischargeTransferDisposal: "Please select an option",
            EquipmentQuantity: "Please enter a valid number",
            ReceptionQuantity: "Please enter a valid number",
            SlopQuantity: "Please enter a valid number"
        }
    });
}

var basePath = document.querySelector('base')?.getAttribute('href') || '';

function submitForm() {
    let formData = {
        Id: $('#Id').val(),
        UserId: userId,
        EntryDate: $('#EntryDate').val(),
        MethodDischargeTransferDisposal: $('#MethodDischargeTransferDisposal').val(),

        // Equipment Fields
        EquipmentQuantity: parseFloat($('#EquipmentQuantity').val()) || null,
        EquipmentResidue: $('#EquipmentResidue').val() || null,
        EquipmentTransferredFrom: $('#EquipmentTransferredFrom').val() || null,
        EquipmentQuantityRetained: parseFloat($('#EquipmentQuantityRetained').val()) || null,
        EquipmentStartTime: $('#EquipmentStartTime').val() || null,
        EquipmentPositionStart: $('#EquipmentPositionStart').val() || null,
        EquipmentStopTime: $('#EquipmentStopTime').val() || null,
        EquipmentPositionStop: $('#EquipmentPositionStop').val() || null,

        // Reception Fields
        ReceptionQuantity: parseFloat($('#ReceptionQuantity').val()) || null,
        ReceptionResidue: $('#ReceptionResidue').val() || null,
        ReceptionTransferredFrom: $('#ReceptionTransferredFrom').val() || null,
        ReceptionQuantityRetained: parseFloat($('#ReceptionQuantityRetained').val()) || null,
        ReceptionStartTime: $('#ReceptionStartTime').val() || null,
        ReceptionStopTime: $('#ReceptionStopTime').val() || null,
        ReceptionPortFacilities: $('#ReceptionPortFacilities').val() || null,
        ReceptionReceiptNo: $('#ReceptionReceiptNo').val() || null,

        // Slop Fields
        SlopTransferredTo: $('#SlopTransferredTo').val() || null,
        SlopQuantity: parseFloat($('#SlopQuantity').val()) || null,
        SlopResidue: $('#SlopResidue').val() || null,
        SlopTransferredFrom: $('#SlopTransferredFrom').val() || null,
        SlopQuantityRetainedFrom: parseFloat($('#SlopQuantityRetainedFrom').val()) || null,
        SlopStartTime: $('#SlopStartTime').val() || null,
        SlopStopTime: $('#SlopStopTime').val() || null,
        SlopQuantityRetainedTo: parseFloat($('#SlopQuantityRetainedTo').val()) || null
    };

    $.ajax({
        url: basePath + "/ORB1/CodeD/UpdateCodeD",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(formData),
        success: function (response) {
            $('#EditCodeDForm')[0].reset();
            resetFormDate();
            hideDependentFields();

            alert(response.message);

            const { pageNumber, pageSize } = getQueryParams();
            window.location.href = `${basePath}/ORB1/CodeD/GetCodeDData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
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

function hideDependentFields() {
    $('#EquipmentFields, #ReceptionFields, #SlopFields').hide();
}

function getQueryParams() {
    const urlParams = new URLSearchParams(window.location.search);
    const pageNumber = urlParams.get('pageNumber') || 1;
    const pageSize = urlParams.get('pageSize') || 10;
    return { pageNumber, pageSize };
}