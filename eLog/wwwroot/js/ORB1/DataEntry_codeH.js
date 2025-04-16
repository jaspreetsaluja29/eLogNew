// Define base path globally
var basePath = document.querySelector('base')?.getAttribute('href') || '';

$(document).ready(function () {
    // Set default date in the format YYYY-MM-DD
    function setTodayDate() {
        let today = new Date();
        let formattedDate = today.getFullYear() + '-' +
            String(today.getMonth() + 1).padStart(2, '0') + '-' +
            String(today.getDate()).padStart(2, '0');
        $('#EntryDate').val(formattedDate);
    }

    // Set today's date on page load
    setTodayDate();

    // Port and Location field handling
    $('#Port').on('input', function () {
        if ($(this).val().trim() !== '') {
            $('#Location').prop('disabled', true).val(''); // Disable and clear Location
        } else {
            $('#Location').prop('disabled', false);
        }
    });

    // Track changes in Location when Port is blank
    $('#Location').on('input', function () {
        if ($('#Port').val().trim() === '') {
            $(this).data('edited', true); // Mark as edited
        }
    });

    // When Port is updated, clear Location if it was edited
    $('#Port').on('input', function () {
        if ($(this).val().trim() !== '' && $('#Location').data('edited')) {
            $('#Location').val('').data('edited', false); // Clear Location and reset flag
        }
    });

    // Populate capacity when tank is selected in Weekly section
    //$('#TankLoaded1').change(function () {
    //    var capacity = $(this).find('option:selected').data('capacity');
    //    $('#TankRetained1').val(capacity ? capacity : '');
    //});

    // Populate capacity when tank is selected in Collection section
    //$('#TankLoaded2').change(function () {
    //    var capacity = $(this).find('option:selected').data('capacity');
    //    $('#TankRetained2').val(capacity ? capacity : '');
    //});

    // Populate capacity when tank is selected in Collection section
    //$('#TankLoaded3').change(function () {
    //    var capacity = $(this).find('option:selected').data('capacity');
    //    $('#TankRetained3').val(capacity ? capacity : '');
    //});

    // Populate capacity when tank is selected in Collection section
    //$('#TankLoaded4').change(function () {
    //    var capacity = $(this).find('option:selected').data('capacity');
    //    $('#TankRetained4').val(capacity ? capacity : '');
    //});

    // Hide all fields except Date & Ballasting or Cleaning at the start
    $('#BunkeringLubricatingOilField').hide();

    // Show respective fields based on selection
    $('#SelectType').change(function () {
        var selectedValue = $(this).val();
        if (selectedValue === 'Bunkering of Fuel') {
            $('#BunkeringLubricatingOilField').show();
        } else {
            $('#BunkeringLubricatingOilField').hide();
        }
    });

    // Save form data via AJAX
    $('#saveButton').click(function (event) {
        event.preventDefault();

        var formData = {
            UserId: userId, // Assuming userId is defined globally
            EntryDate: $('#EntryDate').val(),
            SelectType: $('#SelectType').val() || null,
            Port: $('#Port').val() || null,
            Location: $('#Location').val() || null,
            StartDateTime: $('#StartDateTime').val() || null,
            StopDateTime: $('#StopDateTime').val() || null,
            Quantity: parseFloat($('#Quantity').val()) || 0,
            Grade: $('#Grade').val() || null,
            SulphurContent: $('#SulphurContent').val() || null,
            TankLoaded1: $('#TankLoaded1').val() || null,
            TankRetained1: $('#TankRetained1').val() || null,
            TankLoaded2: $('#TankLoaded2').val() || null,
            TankRetained2: $('#TankRetained2').val() || null,
            TankLoaded3: $('#TankLoaded3').val() || null,
            TankRetained3: $('#TankRetained3').val() || null,
            TankLoaded4: $('#TankLoaded4').val() || null,
            TankRetained4: $('#TankRetained4').val() || null
        };

        $.ajax({
            url: basePath + "/ORB1/CodeH/CreateCodeH",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(formData),
            success: function (response) {
                $('#message').html('<div class="alert alert-success">' + response.message + '</div>');

                // Reset form fields after successful save
                $('#DataEntryCodeHForm')[0].reset();

                // Reset date field with today's date
                setTodayDate();

                // Re-enable Location field after reset
                $('#Location').prop('disabled', false);

                // Reset data flags
                $('#Location').data('edited', false);
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", xhr.responseText);
                $('#message').html('<div class="alert alert-danger">Error: ' + xhr.responseText + '</div>');
            }
        });
    });

    // Reset form fields
    $('#resetButton').click(function () {
        $('#DataEntryCodeHForm')[0].reset();
        $('#message').html('');

        // Re-enable Location field after reset
        $('#Location').prop('disabled', false);

        // Reset data flags
        $('#Location').data('edited', false);

        // Reset date to today
        setTodayDate();
        $('#BunkeringLubricatingOilField').hide();

        // Reload tanks to ensure dropdowns are populated correctly
        loadTanks();
    });

    // Cancel button - Redirect to another page
    $("#cancelButton").click(function () {
        const { pageNumber, pageSize } = getQueryParams();
        window.location.href = `${basePath}/ORB1/CodeH/GetCodeHData?pageNumber=${pageNumber}&pageSize=${pageSize}`;
    });

    // Load tanks from the server
    loadTanks();
});

// Function to load tanks from the server
function loadTanks() {
    $.ajax({
        url: basePath + "/ORB1/CodeH/GetTanks",
        type: "GET",
        dataType: "json",
        success: function (data) {
            if (!data || data.length === 0) {
                return;
            }

            let TankLoaded1 = $("#TankLoaded1");
            let TankLoaded2 = $("#TankLoaded2");
            let TankLoaded3 = $("#TankLoaded3");
            let TankLoaded4 = $("#TankLoaded4");

            if (TankLoaded1.length === 0 || TankLoaded2.length === 0 || TankLoaded3.length === 0 || TankLoaded4.length === 0) {
                return;
            }

            // Clear existing options
            TankLoaded1.empty();
            TankLoaded2.empty();
            TankLoaded3.empty();
            TankLoaded4.empty();

            // Add a default first option
            TankLoaded1.append('<option value="">Select Tank</option>');
            TankLoaded2.append('<option value="">Select Tank</option>');
            TankLoaded3.append('<option value="">Select Tank</option>');
            TankLoaded4.append('<option value="">Select Tank</option>');

            // Add tanks to both dropdowns
            data.forEach(tank => {
                const tankId = tank.tankIdentification || "";
                //const capacity = tank.volumeCapacity || "";

                if (!tankId) {
                    return;
                }

                let option1 = $("<option>")
                    .val(tankId)
                    .text(tankId);
                    //.attr("data-capacity", capacity);

                let option2 = $("<option>")
                    .val(tankId)
                    .text(tankId);
                    //.attr("data-capacity", capacity);

                let option3 = $("<option>")
                    .val(tankId)
                    .text(tankId);
                    //.attr("data-capacity", capacity);

                let option4 = $("<option>")
                    .val(tankId)
                    .text(tankId);
                    //.attr("data-capacity", capacity);

                TankLoaded1.append(option1);
                TankLoaded2.append(option2);
                TankLoaded3.append(option3);
                TankLoaded4.append(option4);
            });

            // Trigger change event to ensure capacity fields are populated if a default is selected
            TankLoaded1.trigger('change');
            TankLoaded2.trigger('change');
            TankLoaded3.trigger('change');
            TankLoaded4.trigger('change');


        },
        error: function (xhr, status, error) {
            // Silently fail
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