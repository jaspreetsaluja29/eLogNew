$(document).ready(function () {
    // Set default date in the format YYYY-MM-DD
    let today = new Date();
    let formattedDate = today.getFullYear() + '-' +
        String(today.getMonth() + 1).padStart(2, '0') + '-' +
        String(today.getDate()).padStart(2, '0');

    // Set the formatted date as the value of the input
    $('#EntryDate').val(formattedDate);

    // Initial state setup and visibility settings
    $('#WeeklyInventoryFields').show(); // Show Weekly Inventory by default
    $('#CollectionFields, #TransferFields, #IncineratorFields, #DisposalShipFields, #DisposalShoreFields').hide();


    // Change event handler
    $('#CollectionType').change(function () {
        var selectedValue = $(this).val();
        setFieldVisibility(selectedValue);
    });

    // Function to handle visibility
    function setFieldVisibility(selectedValue) {
        // Hide all sections first
        $('#WeeklyInventoryFields, #CollectionFields, #TransferFields, #IncineratorFields, #DisposalShipFields, #DisposalShoreFields').hide();

        // Show only the selected section
        switch (selectedValue) {
            case 'WeeklyInventory':
                $('#WeeklyInventoryFields').show();
                break;
            case 'Collection':
                $('#CollectionFields').show();
                break;
            case 'Transfer':
                $('#TransferFields').show();
                break;
            case 'Incinerator':
                $('#IncineratorFields').show();
                break;
            case 'DisposalShip':
                $('#DisposalShipFields').show();
                break;
            case 'DisposalShore':
                $('#DisposalShoreFields').show();
                break;
            default:
                $('#WeeklyInventoryFields').show(); // Default to WeeklyInventory
                break;
        }
    }

    // Populate capacity when tank is selected in Weekly section
    $('#WeeklyIdentityOfTanks').change(function () {
        var capacity = $(this).find('option:selected').data('capacity');
        $('#WeeklyCapacityOfTanks').val(capacity ? capacity : '');
    });

    // Populate capacity when tank is selected in Collection section
    $('#CollectionIdentityOfTanks').change(function () {
        var capacity = $(this).find('option:selected').data('capacity');
        $('#CollectionCapacityOfTanks').val(capacity ? capacity : '');
    });

    // Calculate incinerator operation time
    $('#IncineratorStartTime, #IncineratorStopTime').change(function () {
        var startTime = $('#IncineratorStartTime').val();
        var stopTime = $('#IncineratorStopTime').val();

        if (startTime && stopTime) {
            var start = new Date('2000-01-01T' + startTime + ':00');
            var stop = new Date('2000-01-01T' + stopTime + ':00');

            // If stop time is earlier than start time, assume it's the next day
            if (stop < start) {
                stop.setDate(stop.getDate() + 1);
            }

            var diff = (stop - start) / (1000 * 60 * 60); // Convert ms to hours
            $('#IncineratorTotalOperationTime').val(diff.toFixed(2));
        }
    });

    // Save form data via AJAX
    $('#DataEntryCodeCForm').submit(function (event) {
        event.preventDefault();

        // Get the selected collection type
        var collectionType = $('#CollectionType').val();

        // Create a base data object with common fields
        var formData = {
            EntryDate: $('#EntryDate').val(),
            CollectionType: collectionType
        };

        // Add fields based on which section is visible
        switch (collectionType) {
            case 'WeeklyInventory':
                formData.IdentityOfTanks = $('#WeeklyIdentityOfTanks').val();
                formData.CapacityOfTanks = $('#WeeklyCapacityOfTanks').val();
                formData.TotalQuantityOfRetention = $('#WeeklyTotalQuantityOfRetention').val();
                break;

            case 'Collection':
                formData.IdentityOfTanks = $('#CollectionIdentityOfTanks').val();
                formData.CapacityOfTanks = $('#CollectionCapacityOfTanks').val();
                formData.TotalQuantityOfRetention = $('#CollectionTotalQuantityOfRetention').val();
                formData.ManualResidueQuantity = $('#CollectionManualResidueQuantity').val();
                formData.CollectedFromTank = $('#CollectionCollectedFromTank').val();
                break;

            case 'Transfer':
                formData.OperationType = $('#TransferOperationType').val();
                formData.Quantity = $('#TransferQuantity').val();
                formData.TanksFrom = $('#TransferTanksFrom').val();
                formData.RetainedIn = $('#TransferRetainedIn').val();
                formData.TanksTo = $('#TransferTanksTo').val();
                break;

            case 'Incinerator':
                formData.OperationType = $('#IncineratorOperationType').val();
                formData.Quantity = $('#IncineratorQuantity').val();
                formData.TanksFrom = $('#IncineratorTanksFrom').val();
                formData.RetainedIn = $('#IncineratorRetainedIn').val();
                formData.TotalRetainedContent = $('#IncineratorTotalRetainedContent').val();
                formData.StartTime = $('#IncineratorStartTime').val();
                formData.StopTime = $('#IncineratorStopTime').val();
                formData.TotalOperationTime = $('#IncineratorTotalOperationTime').val();
                break;

            case 'DisposalShip':
                formData.Quantity = $('#DisposalShipQuantity').val();
                formData.TanksFrom = $('#DisposalShipTanksFrom').val();
                formData.RetainedIn = $('#DisposalShipRetainedIn').val();
                formData.TanksTo = $('#DisposalShipTanksTo').val();
                formData.RetainedTo = $('#DisposalShipRetainedTo').val();
                break;

            case 'DisposalShore':
                formData.Quantity = $('#DisposalShoreQuantity').val();
                formData.TanksFrom = $('#DisposalShoreTanksFrom').val();
                formData.RetainedInDischargeTanks = $('#DisposalShoreRetainedInDischargeTanks').val();
                formData.BargeName = $('#DisposalShoreBargeName').val();
                formData.ReceptionFacility = $('#DisposalShoreReceptionFacility').val();
                formData.ReceiptNo = $('#DisposalShoreReceiptNo').val();
                break;
        }

        $.ajax({
            url: "/ORB1/CodeC/CreateCodeC",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(formData),
            success: function (response) {
                $('#message').html('<div class="alert alert-success">' + response.message + '</div>');

                // Reset form fields after successful save
                $('#DataEntryCodeCForm')[0].reset();

                // Reset date field with today's date
                $('#EntryDate').val(formattedDate);

                // Set default CollectionType
                $('#CollectionType').val('WeeklyInventory');

                // Show default fields
                setFieldVisibility('WeeklyInventory');
            },
            error: function (xhr, status, error) {
                $('#message').html('<div class="alert alert-danger">Error: ' + xhr.responseText + '</div>');
            }
        });
    });

    // Reset button
    $('#resetButton').click(function () {
        $('#DataEntryCodeCForm')[0].reset();
        $('#EntryDate').val(formattedDate);
        $('#CollectionType').val('WeeklyInventory');
        setFieldVisibility('WeeklyInventory');
        $('#message').html('');
    });

    // Cancel button
    $('#cancelButton').click(function () {
        window.location.href = '/ORB1/CodeC/GetCodeCData';
    });

    // Clean loadTanks function
    function loadTanks() {
        $.ajax({
            url: "/ORB1/CodeC/GetTanks",
            type: "GET",
            dataType: "json",
            success: function (data) {
                if (!data || data.length === 0) {
                    return;
                }

                let weeklySelect = $("#WeeklyIdentityOfTanks");
                let collectionSelect = $("#CollectionIdentityOfTanks");

                if (weeklySelect.length === 0 || collectionSelect.length === 0) {
                    return;
                }

                // Clear existing options
                weeklySelect.empty();
                collectionSelect.empty();

                // Add a default first option
                weeklySelect.append('<option value="">Select Tank</option>');
                collectionSelect.append('<option value="">Select Tank</option>');

                // Add tanks to both dropdowns
                data.forEach(tank => {
                    const tankId = tank.tankIdentification || "";
                    const capacity = tank.volumeCapacity || "";

                    if (!tankId) {
                        return;
                    }

                    let option1 = $("<option>")
                        .val(tankId)
                        .text(tankId)
                        .attr("data-capacity", capacity);

                    let option2 = $("<option>")
                        .val(tankId)
                        .text(tankId)
                        .attr("data-capacity", capacity);

                    weeklySelect.append(option1);
                    collectionSelect.append(option2);
                });

                // Trigger change event to ensure capacity fields are populated if a default is selected
                weeklySelect.trigger('change');
                collectionSelect.trigger('change');

                // Set the first select field to visible to match CollectionType default
                setFieldVisibility($('#CollectionType').val());
            },
            error: function (xhr, status, error) {
                // Silently fail
            }
        });
    }

    // Call the loadTanks function when the page loads
    loadTanks();
});