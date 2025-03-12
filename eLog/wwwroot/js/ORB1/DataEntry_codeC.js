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
        
        if ($(this).val() === 'WeeklyInventory') {
            fetchLastWeeklyRetention();
        }
    });

    // Fetch the last weekly retention value when the form loads
    fetchLastWeeklyRetention();

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
    $('#saveButton').click(function (event) {
        event.preventDefault();

        // Get the selected collection type
        var collectionType = $('#CollectionType').val();

        // Create a FormData object instead of a regular object
        var formData = new FormData();

        // Add common fields
        formData.append("UserId", userId); // Assuming you have a hidden UserId field
        formData.append("EntryDate", $('#EntryDate').val());
        formData.append("CollectionType", collectionType);

        // Add fields based on which section is visible
        switch (collectionType) {
            case 'WeeklyInventory':
                formData.append("WeeklyIdentityOfTanks", $('#WeeklyIdentityOfTanks').val());
                formData.append("WeeklyCapacityOfTanks", $('#WeeklyCapacityOfTanks').val());
                formData.append("WeeklyTotalQuantityOfRetention", $('#WeeklyTotalQuantityOfRetention').val());
                break;

            case 'Collection':
                formData.append("CollectionIdentityOfTanks", $('#CollectionIdentityOfTanks').val());
                formData.append("CollectionCapacityOfTanks", $('#CollectionCapacityOfTanks').val());
                formData.append("CollectionTotalQuantityOfRetention", $('#CollectionTotalQuantityOfRetention').val());
                formData.append("CollectionManualResidueQuantity", $('#CollectionManualResidueQuantity').val());
                formData.append("CollectionCollectedFromTank", $('#CollectionCollectedFromTank').val());
                break;

            case 'Transfer':
                formData.append("TransferOperationType", $('#TransferOperationType').val());
                formData.append("TransferQuantity", $('#TransferQuantity').val());
                formData.append("TransferTanksFrom", $('#TransferTanksFrom').val());
                formData.append("TransferRetainedIn", $('#TransferRetainedIn').val());
                formData.append("TransferTanksTo", $('#TransferTanksTo').val());
                break;

            case 'Incinerator':
                formData.append("IncineratorOperationType", $('#IncineratorOperationType').val());
                formData.append("IncineratorQuantity", $('#IncineratorQuantity').val());
                formData.append("IncineratorTanksFrom", $('#IncineratorTanksFrom').val());
                formData.append("IncineratorRetainedIn", $('#IncineratorRetainedIn').val());
                formData.append("IncineratorTotalRetainedContent", $('#IncineratorTotalRetainedContent').val());
                formData.append("IncineratorStartTime", $('#IncineratorStartTime').val());
                formData.append("IncineratorStopTime", $('#IncineratorStopTime').val());
                formData.append("IncineratorTotalOperationTime", $('#IncineratorTotalOperationTime').val());
                break;

            case 'DisposalShip':
                formData.append("DisposalShipQuantity", $('#DisposalShipQuantity').val());
                formData.append("DisposalShipTanksFrom", $('#DisposalShipTanksFrom').val());
                formData.append("DisposalShipRetainedIn", $('#DisposalShipRetainedIn').val());
                formData.append("DisposalShipTanksTo", $('#DisposalShipTanksTo').val());
                formData.append("DisposalShipRetainedTo", $('#DisposalShipRetainedTo').val());
                break;

            case 'DisposalShore':
                formData.append("DisposalShoreQuantity", $('#DisposalShoreQuantity').val());
                formData.append("DisposalShoreTanksFrom", $('#DisposalShoreTanksFrom').val());
                formData.append("DisposalShoreRetainedInDischargeTanks", $('#DisposalShoreRetainedInDischargeTanks').val());
                formData.append("DisposalShoreBargeName", $('#DisposalShoreBargeName').val());
                formData.append("DisposalShoreReceptionFacility", $('#DisposalShoreReceptionFacility').val());
                formData.append("DisposalShoreReceiptNo", $('#DisposalShoreReceiptNo').val());

                // Get the file input
                var fileInput = $('#DisposalShoreAttachment')[0].files[0];
                if (fileInput) {
                    formData.append("DisposalShoreAttachment", fileInput);
                }
                break;
        }

        $.ajax({
            url: "/ORB1/CodeC/CreateCodeC",
            type: "POST",
            data: formData,
            processData: false,  // Prevent jQuery from converting the FormData object to a string
            contentType: false,  // Let the browser set the Content-Type, including multipart/form-data for files
            success: function (response) {
                $('#message').html('<div class="alert alert-success">' + response.message + '</div>');
                // Reset form fields after successful save
                $('#DataEntryCodeCForm')[0].reset();
                // Reset date field with today's date
                let today = new Date();
                let formattedDate = today.getFullYear() + '-' +
                    String(today.getMonth() + 1).padStart(2, '0') + '-' +
                    String(today.getDate()).padStart(2, '0');
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

    function fetchLastWeeklyRetention() {
        $.ajax({
            url: "/ORB1/CodeC/GetLastWeeklyRetention",
            type: "GET",
            success: function (response) {
                if (response && response.length > 0) {
                    const data = response[0];
                    const retentionValue = data.WeeklyTotalQuantityOfRetention;

                    if (retentionValue) {
                        $('#lastWeeklyRetention').text(`Last Week content: ${retentionValue} m³`);
                    } else {
                        // Fallback to iterate through properties if direct access fails
                        let foundValue = false;
                        for (const key in data) {
                            if (key === 'WeeklyTotalQuantityOfRetention' || key.includes('Retention')) {
                                $('#lastWeeklyRetention').text(`Last Week content: ${data[key]} m³`);
                                foundValue = true;
                                break;
                            }
                        }

                        if (!foundValue) {
                            $('#lastWeeklyRetention').text('Data format not as expected');
                        }
                    }
                } else {
                    $('#lastWeeklyRetention').text('No previous data available');
                }
            },
            error: function (xhr, status, error) {
                $('#lastWeeklyRetention').text('Error loading previous data');
            }
        });
    }

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