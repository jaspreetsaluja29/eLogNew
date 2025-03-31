$(document).ready(function () {
    initializeFormState();
    setupEventListeners();
    setupFormValidation();
});

function initializeFormState() {
    // Get the current collection type
    let collectionType = $('#CollectionType').val();

    // Log for debugging
    console.log("Collection Type on load:", collectionType);

    // If no collection type is selected or it's an empty string, default to WeeklyInventory
    if (!collectionType || collectionType === "") {
        $('#CollectionType').val("WeeklyInventory");
        collectionType = "WeeklyInventory";
    }

    // Hide all field sections first
    $("#WeeklyInventoryFields, #CollectionFields, #TransferFields, #IncineratorFields, #DisposalShipFields, #DisposalShoreFields").hide();

    // Show only the relevant section based on collection type
    if (collectionType === "WeeklyInventory") {
        $("#WeeklyInventoryFields").show();
    } else if (collectionType === "Collection") {
        $("#CollectionFields").show();
    } else if (collectionType === "Transfer") {
        $("#TransferFields").show();
    } else if (collectionType === "Incinerator") {
        $("#IncineratorFields").show();
    } else if (collectionType === "DisposalShip") {
        $("#DisposalShipFields").show();
    } else if (collectionType === "DisposalShore") {
        $("#DisposalShoreFields").show();
    }
}

function setupEventListeners() {
    // Handle collection type change
    $("#CollectionType").change(function () {
        initializeFormState(); // Re-initialize based on new collection type
    });

    $("#OperationType").change(toggleOperationFields);
    $("#TankCleaned").change(toggleTankCleaningFields);
    $("#DisposalMethod").change(toggleDisposalFields);

    // Handle change event to update capacity when selecting a tank
    $("#WeeklyIdentityOfTanks").change(function () {
        let selectedCapacity = $(this).find(":selected").data("capacity");
        $("#WeeklyCapacityOfTanks").val(selectedCapacity || "");
    });

    // Same for collection fields
    $("#CollectionIdentityOfTanks").change(function () {
        let selectedCapacity = $(this).find(":selected").data("capacity");
        $("#CollectionCapacityOfTanks").val(selectedCapacity || "");
    });

    $("#cancelButton").click(function () {
        const { pageNumber, pageSize } = getQueryParams();
        var basePath = document.querySelector('base')?.getAttribute('href') || '/';
        window.location.href = `${basePath}/ORB1/CodeC/GetCodeCData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    });

    $("#EditCodeCForm").submit(function (event) {
        event.preventDefault();
        if ($(this).valid()) {
            submitForm();
        }
    });
}

function toggleOperationFields() {
    let operation = $("#OperationType").val();
    $("#disposalFields").toggle(operation === "Disposal");
    $("#retentionFields").toggle(operation === "Retention");
    $("#transferFields").toggle(operation === "Transfer");
    toggleTankCleaningFields();
    toggleDisposalFields();
}

function toggleTankCleaningFields() {
    let cleaned = $("#TankCleaned").val();
    $("#tankCleaningFields").toggle(cleaned === "Yes");
}

function toggleDisposalFields() {
    let method = $("#DisposalMethod").val();
    $("#shoreDisposalFields").toggle(method === "Shore");
    $("#seaDisposalFields").toggle(method === "Sea");
}

function setupFormValidation() {
    $.validator.addMethod("validPosition", function (value) {
        if (!value) return true;
        return /^\d{1,2}\s*DEG\s*\d{1,2}\s*MIN\s*[NS],\s*\d{1,3}\s*DEG\s*\d{1,2}\s*MIN\s*[EW]$/i.test(value);
    }, "Format: XX DEG XX MIN N/S, XXX DEG XX MIN E/W");

    $("#EditCodeCForm").validate({
        rules: {
            EntryDate: "required",
            CollectionType: "required",
            // Weekly Inventory validation
            WeeklyTotalQuantityOfRetention: {
                required: function () {
                    return $("#CollectionType").val() === "WeeklyInventory";
                },
                number: true
            },
            // Collection validation
            CollectionIdentityOfTanks: {
                required: function () {
                    return $("#CollectionType").val() === "Collection";
                }
            },
            CollectionTotalQuantityOfRetention: {
                required: function () {
                    return $("#CollectionType").val() === "Collection";
                },
                number: true
            },
            CollectionManualResidueQuantity: {
                required: function () {
                    return $("#CollectionType").val() === "Collection";
                },
                number: true
            },
            CollectionCollectedFromTank: {
                required: function () {
                    return $("#CollectionType").val() === "Collection";
                }
            },
            // Transfer validation
            TransferOperationType: {
                required: function () {
                    return $("#CollectionType").val() === "Transfer";
                }
            },
            TransferQuantity: {
                required: function () {
                    return $("#CollectionType").val() === "Transfer";
                },
                number: true
            },
            TransferTanksFrom: {
                required: function () {
                    return $("#CollectionType").val() === "Transfer";
                }
            },
            TransferRetainedIn: {
                required: function () {
                    return $("#CollectionType").val() === "Transfer";
                }
            },
            TransferTanksTo: {
                required: function () {
                    return $("#CollectionType").val() === "Transfer";
                }
            },
            // Incinerator validation
            IncineratorOperationType: {
                required: function () {
                    return $("#CollectionType").val() === "Incinerator";
                }
            },
            IncineratorTanksFrom: {
                required: function () {
                    return $("#CollectionType").val() === "Incinerator";
                }
            },
            IncineratorTotalRetainedContent: {
                required: function () {
                    return $("#CollectionType").val() === "Incinerator";
                },
                number: true
            },
            IncineratorStartTime: {
                required: function () {
                    return $("#CollectionType").val() === "Incinerator";
                }
            },
            IncineratorStopTime: {
                required: function () {
                    return $("#CollectionType").val() === "Incinerator";
                }
            },
            IncineratorTotalOperationTime: {
                required: function () {
                    return $("#CollectionType").val() === "Incinerator";
                },
                number: true
            },
            // DisposalShip validation
            DisposalShipQuantity: {
                required: function () {
                    return $("#CollectionType").val() === "DisposalShip";
                },
                number: true
            },
            DisposalShipTanksFrom: {
                required: function () {
                    return $("#CollectionType").val() === "DisposalShip";
                }
            },
            DisposalShipRetainedIn: {
                required: function () {
                    return $("#CollectionType").val() === "DisposalShip";
                }
            },
            DisposalShipTanksTo: {
                required: function () {
                    return $("#CollectionType").val() === "DisposalShip";
                }
            },
            DisposalShipRetainedTo: {
                required: function () {
                    return $("#CollectionType").val() === "DisposalShip";
                }
            },
            // DisposalShore validation
            DisposalShoreQuantity: {
                required: function () {
                    return $("#CollectionType").val() === "DisposalShore";
                },
                number: true
            },
            DisposalShoreTanksFrom: {
                required: function () {
                    return $("#CollectionType").val() === "DisposalShore";
                }
            },
            DisposalShoreRetainedInDischargeTanks: {
                required: function () {
                    return $("#CollectionType").val() === "DisposalShore";
                }
            },
            DisposalShoreBargeName: {
                required: function () {
                    return $("#CollectionType").val() === "DisposalShore";
                }
            },
            DisposalShoreReceptionFacility: {
                required: function () {
                    return $("#CollectionType").val() === "DisposalShore";
                }
            },
            DisposalShoreReceiptNo: {
                required: function () {
                    return $("#CollectionType").val() === "DisposalShore";
                }
            }
        },
        messages: {
            EntryDate: "Please enter a valid date",
            CollectionType: "Please select an operation type"
        }
    });
}

function submitForm() {

    var basePath = document.querySelector('base')?.getAttribute('href') || ''; // Get base path

    let formData = {
        Id: $('#Id').val(),
        UserId: userId,
        EntryDate: $('#EntryDate').val(),
        CollectionType: $('#CollectionType').val(),

        // Weekly Inventory Fields
        WeeklyIdentityOfTanks: $('#WeeklyIdentityOfTanks').val() || null,
        WeeklyCapacityOfTanks: parseFloat($('#WeeklyCapacityOfTanks').val()) || null,
        WeeklyTotalQuantityOfRetention: parseFloat($('#WeeklyTotalQuantityOfRetention').val()) || null,

        // Collection Fields
        CollectionIdentityOfTanks: $('#CollectionIdentityOfTanks').val() || null,
        CollectionCapacityOfTanks: parseFloat($('#CollectionCapacityOfTanks').val()) || null,
        CollectionTotalQuantityOfRetention: parseFloat($('#CollectionTotalQuantityOfRetention').val()) || null,
        CollectionManualResidueQuantity: parseFloat($('#CollectionManualResidueQuantity').val()) || null,
        CollectionCollectedFromTank: $('#CollectionCollectedFromTank').val() || null,

        // Transfer Fields
        TransferOperationType: $('#TransferOperationType').val() || null,
        TransferQuantity: parseFloat($('#TransferQuantity').val()) || null,
        TransferTanksFrom: $('#TransferTanksFrom').val() || null,
        TransferRetainedIn: $('#TransferRetainedIn').val() || null,
        TransferTanksTo: $('#TransferTanksTo').val() || null,

        // Incinerator Fields
        IncineratorOperationType: $('#IncineratorOperationType').val() || null,
        IncineratorQuantity: parseFloat($('#IncineratorQuantity').val()) || null,
        IncineratorTanksFrom: $('#IncineratorTanksFrom').val() || null,
        IncineratorTotalRetainedContent: parseFloat($('#IncineratorTotalRetainedContent').val()) || null,
        IncineratorStartTime: $('#IncineratorStartTime').val() || null,
        IncineratorStopTime: $('#IncineratorStopTime').val() || null,
        IncineratorTotalOperationTime: parseFloat($('#IncineratorTotalOperationTime').val()) || null,

        // Disposal Ship Fields
        DisposalShipQuantity: parseFloat($('#DisposalShipQuantity').val()) || null,
        DisposalShipTanksFrom: $('#DisposalShipTanksFrom').val() || null,
        DisposalShipRetainedIn: $('#DisposalShipRetainedIn').val() || null,
        DisposalShipTanksTo: $('#DisposalShipTanksTo').val() || null,
        DisposalShipRetainedTo: $('#DisposalShipRetainedTo').val() || null,

        // Disposal Shore Fields
        DisposalShoreQuantity: parseFloat($('#DisposalShoreQuantity').val()) || null,
        DisposalShoreTanksFrom: $('#DisposalShoreTanksFrom').val() || null,
        DisposalShoreRetainedInDischargeTanks: $('#DisposalShoreRetainedInDischargeTanks').val() || null,
        DisposalShoreBargeName: $('#DisposalShoreBargeName').val() || null,
        DisposalShoreReceptionFacility: $('#DisposalShoreReceptionFacility').val() || null,
        DisposalShoreReceiptNo: $('#DisposalShoreReceiptNo').val() || null
    };

    $.ajax({
        url: basePath + "/ORB1/CodeC/Update",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(formData),
        success: function (response) {
            // Show message in an alert popup
            alert(response.message);

            const { pageNumber, pageSize } = getQueryParams();
            // Redirect after the user clicks "OK"
            var basePath = document.querySelector('base')?.getAttribute('href') || '/';
            window.location.href = `${basePath}/ORB1/CodeC/GetCodeCData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
        },
        error: function (xhr) {
            // Show error message
            $('#message').html('<div class="alert alert-danger">Error: ' + xhr.responseText + '</div>');
        }
    });
}

// Function to get query parameters from the URL
function getQueryParams() {
    const urlParams = new URLSearchParams(window.location.search);
    const pageNumber = urlParams.get('pageNumber') || 1;
    const pageSize = urlParams.get('pageSize') || 10;

    return { pageNumber, pageSize };
}